using System;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Threading;
using System.Reactive.Subjects;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents a Pulse Pal device.
    /// </summary>
    public sealed class PulsePalDevice : IDisposable
    {
        internal const double MinVoltage = -10;
        internal const double MaxVoltage = 10;
        internal const double MinTimePeriod = 0.0001;
        internal const double MaxTimePeriod = 3600;

        const int BaudRate = 12000000;
        const int CycleFrequency = 20000;
        const int MaxCyclePeriod = 3600 * CycleFrequency;
        const int MaxPulseLength = 1000;
        const int MaxDataBytes = 8192;

        const byte OpMenu                = 213;
        const byte Handshake             = 72;
        const byte Acknowledge           = 75;

        const byte ProgramParam          = 74;
        const byte ProgramPulseTrain1    = 75;
        const byte ProgramPulseTrain2    = 76;
        const byte TriggerCommand        = 77;
        const byte UpdateDisplayCommand  = 78;
        const byte SetVoltageCommand     = 79;
        const byte AbortCommand          = 80;
        const byte DisconnectCommand     = 81;
        const byte LoopCommand           = 82;
        const byte ClientIdCommand       = 89;
        const byte LineBreak             = 254;

        bool disposed;
        int firmwareVersion;
        int dacMaxValue;
        readonly AsyncSubject<bool> initialized;
        readonly SerialPort serialPort;
        readonly byte[] commandBuffer;
        readonly byte[] readBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PulsePalDevice"/> class using
        /// the specified port name.
        /// </summary>
        /// <param name="portName">
        /// The name of the serial port used to communicate with the Pulse Pal device.
        /// </param>
        public PulsePalDevice(string portName)
        {
            serialPort = new SerialPort(portName);
            serialPort.BaudRate = BaudRate;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.Parity = Parity.None;
            serialPort.DtrEnable = false;
            serialPort.RtsEnable = true;

            firmwareVersion = -1;
            initialized = new();
            serialPort.WriteBufferSize = MaxDataBytes;
            commandBuffer = new byte[serialPort.WriteBufferSize];
            readBuffer = new byte[serialPort.ReadBufferSize];
        }

        /// <summary>
        /// Gets the version of the firmware used by the Pulse Pal device.
        /// </summary>
        public int FirmwareVersion
        {
            get { return firmwareVersion; }
        }

        /// <summary>
        /// Gets a value indicating the open or closed status of the <see cref="PulsePalDevice"/> object.
        /// </summary>
        public bool IsOpen
        {
            get { return serialPort.IsOpen && initialized.IsCompleted; }
        }

        Task RunAsync(CancellationToken cancellationToken)
        {
            serialPort.Open();
            serialPort.ReadExisting();
            Connect();

            return Task.Factory.StartNew(() =>
            {
                var offset = 0;
                using var cancellation = cancellationToken.Register(serialPort.Dispose);
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var bytesToRead = serialPort.BytesToRead;
                        if (bytesToRead == 0)
                        {
                            var nextByte = (byte)serialPort.ReadByte();
                            if (nextByte < 0) break;
                            readBuffer[offset++] = nextByte;
                            offset -= ProcessResponse(offset);
                        }
                        else
                        {
                            while (bytesToRead > 0)
                            {
                                var bytesRead = serialPort.Read(readBuffer, offset, Math.Min(bytesToRead, readBuffer.Length - offset));
                                bytesToRead -= bytesRead;
                                offset += bytesRead;
                                offset -= ProcessResponse(offset);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            if (!initialized.IsCompleted)
                            {
                                initialized.OnError(ex);
                            }
                            throw;
                        }
                        break;
                    }
                }
            },
            cancellationToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
        }

        /// <summary>
        /// Opens a new serial port connection to the Pulse Pal device.
        /// </summary>
        /// <param name="cancellationToken">
        /// A <see cref="CancellationToken"/> which can be used to cancel the operation.
        /// </param>
        public void Open(CancellationToken cancellationToken = default)
        {
            RunAsync(cancellationToken);
            initialized.GetResult();
        }

        void Connect()
        {
            using var writer = new CommandWriter(this);
            writer.Write(OpMenu);
            writer.Write(Handshake);
        }

        int ProcessResponse(int count)
        {
            if (!initialized.IsCompleted)
            {
                const int ResponseLength = 5;
                if (count < ResponseLength) return 0;
                if (readBuffer[0] != Acknowledge)
                {
                    throw new InvalidOperationException("Unexpected return value from Pulse Pal.");
                }

                firmwareVersion = BitConverter.ToInt32(readBuffer, 1);
                dacMaxValue = firmwareVersion switch
                {
                    < 20 => byte.MaxValue,
                    < 40 => ushort.MaxValue,
                    _ => throw new InvalidOperationException($"Unknown Pulse Pal firmware version {firmwareVersion}.")
                };
                initialized.OnNext(true);
                initialized.OnCompleted();
                return ResponseLength;
            }
            else return count;
        }

        void Disconnect()
        {
            using var writer = new CommandWriter(this);
            writer.Write(OpMenu);
            writer.Write(DisconnectCommand);
        }

        /// <summary>
        /// Sets the specified output channel to produce either monophasic
        /// or biphasic square pulses.
        /// </summary>
        /// <param name="channel">The output channel to configure.</param>
        /// <param name="isBiphasic">
        /// <see langword="true"/> to produce biphasic pulses;
        /// <see langword="false"/> to produce monophasic pulses.
        /// </param>
        public void SetBiphasic(OutputChannel channel, bool isBiphasic)
        {
            ProgramParameter(channel, ParameterCode.Biphasic, isBiphasic);
        }

        /// <summary>
        /// Sets the voltage for the first phase of each pulse on a specified
        /// output channel.
        /// </summary>
        /// <param name="channel">The output channel to configure.</param>
        /// <param name="volts">
        /// The voltage of the first phase of the pulse, in the range [-10, 10] volts.
        /// </param>
        public void SetPhase1Voltage(OutputChannel channel, double volts)
        {
            ProgramParameterVoltage(channel, ParameterCode.Phase1Voltage, volts);
        }

        /// <summary>
        /// Sets the voltage for the second phase of each pulse on a specified
        /// output channel.
        /// </summary>
        /// <param name="channel">The output channel to configure.</param>
        /// <param name="volts">
        /// The voltage of the second phase of the pulse, in the range [-10, 10] volts.
        /// </param>
        public void SetPhase2Voltage(OutputChannel channel, double volts)
        {
            ProgramParameterVoltage(channel, ParameterCode.Phase2Voltage, volts);
        }

        /// <summary>
        /// Sets the duration for the first phase of each pulse on a specified
        /// output channel.
        /// </summary>
        /// <param name="channel">The output channel to configure.</param>
        /// <param name="seconds">
        /// The duration of the first phase of the pulse, in the range
        /// [0.0001, 3600] seconds.
        /// </param>
        public void SetPhase1Duration(OutputChannel channel, double seconds)
        {
            ProgramParameterTime(channel, ParameterCode.Phase1Duration, seconds);
        }

        /// <summary>
        /// Sets the interval between the first and second phases of biphasic pulses
        /// on a specified output channel.
        /// </summary>
        /// <param name="channel">The output channel to configure.</param>
        /// <param name="seconds">
        /// The interval between the first and second phase of a biphasic pulse,
        /// in the range [0.0001, 3600] seconds.
        /// </param>
        public void SetInterPhaseInterval(OutputChannel channel, double seconds)
        {
            ProgramParameterTime(channel, ParameterCode.InterPhaseInterval, seconds);
        }

        /// <summary>
        /// Sets the duration for the second phase of each pulse on a specified
        /// output channel.
        /// </summary>
        /// <param name="channel">The output channel to configure.</param>
        /// <param name="seconds">
        /// The duration of the second phase of the pulse, in the range
        /// [0.0001, 3600] seconds.
        /// </param>
        public void SetPhase2Duration(OutputChannel channel, double seconds)
        {
            ProgramParameterTime(channel, ParameterCode.Phase2Duration, seconds);
        }

        /// <summary>
        /// Sets the interval between pulses on a specified output channel.
        /// </summary>
        /// <param name="channel">The output channel to configure.</param>
        /// <param name="seconds">
        /// The interval between pulses, in the range [0.0001, 3600] seconds.
        /// </param>
        public void SetInterPulseInterval(OutputChannel channel, double seconds)
        {
            ProgramParameterTime(channel, ParameterCode.InterPulseInterval, seconds);
        }

        /// <summary>
        /// Sets the duration of a pulse burst when using burst mode.
        /// </summary>
        /// <param name="channel">The output channel to configure.</param>
        /// <param name="seconds">
        /// The duration of a pulse burst, in the range [0.0001, 3600] seconds.
        /// Burst mode is automatically disabled if this value is set to zero.
        /// </param>
        public void SetBurstDuration(OutputChannel channel, double seconds)
        {
            ProgramParameterTime(channel, ParameterCode.BurstDuration, seconds);
        }

        /// <summary>
        /// Sets the duration of the off-time between bursts.
        /// </summary>
        /// <param name="channel">The output channel to configure.</param>
        /// <param name="seconds">
        /// The duration of the off-time between bursts, in the range
        /// [0.0001, 3600] seconds.
        /// </param>
        public void SetInterBurstInterval(OutputChannel channel, double seconds)
        {
            ProgramParameterTime(channel, ParameterCode.InterBurstInterval, seconds);
        }

        /// <summary>
        /// Sets the duration of the entire pulse train.
        /// </summary>
        /// <param name="channel">The output channel to configure.</param>
        /// <param name="seconds">
        /// The duration of the pulse train, in the range [0.0001, 3600] seconds.
        /// </param>
        public void SetPulseTrainDuration(OutputChannel channel, double seconds)
        {
            ProgramParameterTime(channel, ParameterCode.PulseTrainDuration, seconds);
        }

        /// <summary>
        /// Sets a delay between the arrival of a trigger and when the channel
        /// begins its pulse train.
        /// </summary>
        /// <param name="channel">The output channel to configure.</param>
        /// <param name="seconds">
        /// The delay to start the pulse train, in the range [0.0001, 3600] seconds.
        /// </param>
        public void SetPulseTrainDelay(OutputChannel channel, double seconds)
        {
            ProgramParameterTime(channel, ParameterCode.PulseTrainDelay, seconds);
        }

        /// <summary>
        /// Links or unlinks an output channel to trigger channel 1.
        /// </summary>
        /// <param name="channel">The output channel to link or unlink.</param>
        /// <param name="enabled">
        /// <see langword="true"/> if trigger channel 1 can trigger this output channel;
        /// <see langword="false"/> otherwise.
        /// </param>
        public void SetTriggerOnChannel1(OutputChannel channel, bool enabled)
        {
            ProgramParameter(channel, ParameterCode.TriggerOnChannel1, enabled);
        }

        /// <summary>
        /// Links or unlinks an output channel to trigger channel 2.
        /// </summary>
        /// <param name="channel">The output channel to link or unlink.</param>
        /// <param name="enabled">
        /// <see langword="true"/> if trigger channel 2 can trigger this output channel;
        /// <see langword="false"/> otherwise.
        /// </param>
        public void SetTriggerOnChannel2(OutputChannel channel, bool enabled)
        {
            ProgramParameter(channel, ParameterCode.TriggerOnChannel2, enabled);
        }

        /// <summary>
        /// Sets the identity of the custom train used to specify pulse times and
        /// voltages on an output channel.
        /// </summary>
        /// <param name="channel">The output channel to configure.</param>
        /// <param name="trainId">
        /// The identity of the custom pulse train to use on the specified
        /// output channel.
        /// </param>
        public void SetCustomTrainIdentity(OutputChannel channel, CustomTrainId trainId)
        {
            ProgramParameter(channel, ParameterCode.CustomTrainIdentity, (byte)trainId);
        }

        /// <summary>
        /// Sets the interpretation of pulse times in the custom train configured
        /// on the specified output channel.
        /// </summary>
        /// <param name="channel">The output channel to configure.</param>
        /// <param name="target">
        /// The interpretation of pulse times in the custom pulse train.
        /// </param>
        public void SetCustomTrainTarget(OutputChannel channel, CustomTrainTarget target)
        {
            ProgramParameter(channel, ParameterCode.CustomTrainTarget, (byte)target);
        }

        /// <summary>
        /// Sets an output channel to loop its custom pulse train.
        /// </summary>
        /// <param name="channel">The output channel to configure.</param>
        /// <param name="loop">
        /// <see langword="true"/> if the output channel should loop its custom pulse train
        /// for the duration specified by <see cref="SetPulseTrainDuration"/>; otherwise,
        /// the pulse train ends after its final pulse.
        /// </param>
        public void SetCustomTrainLoop(OutputChannel channel, bool loop)
        {
            ProgramParameter(channel, ParameterCode.CustomTrainLoop, loop);
        }

        /// <summary>
        /// Sets the resting voltage on a specified output channel, i.e. the voltage
        /// between phases, pulses and pulse trains.
        /// </summary>
        /// <param name="channel">The output channel to configure.</param>
        /// <param name="volts">
        /// The resting voltage, in the range [-10, 10] volts.
        /// </param>
        public void SetRestingVoltage(OutputChannel channel, double volts)
        {
            ProgramParameterVoltage(channel, ParameterCode.RestingVoltage, volts);
        }

        /// <summary>
        /// Sets the behavior of a trigger channel.
        /// </summary>
        /// <param name="channel">The trigger channel to configure.</param>
        /// <param name="triggerMode">Specifies the behavior of the trigger channel.</param>
        public void SetTriggerMode(TriggerChannel channel, TriggerMode triggerMode)
        {
            ProgramParameter((OutputChannel)channel, ParameterCode.TriggerMode, (byte)triggerMode);
        }

        void ProgramParameter(OutputChannel channel, ParameterCode parameter, bool value)
        {
            using var writer = new CommandWriter(this);
            writer.WriteProgramHeader(channel, parameter);
            writer.Write(value);
        }

        void ProgramParameter(OutputChannel channel, ParameterCode parameter, byte value)
        {
            using var writer = new CommandWriter(this);
            writer.WriteProgramHeader(channel, parameter);
            writer.Write(value);
        }

        void ProgramParameterVoltage(OutputChannel channel, ParameterCode parameter, double volts)
        {
            using var writer = new CommandWriter(this);
            writer.WriteProgramHeader(channel, parameter);
            writer.WriteVoltage(volts);
        }

        void ProgramParameterTime(OutputChannel channel, ParameterCode parameter, double seconds)
        {
            using var writer = new CommandWriter(this);
            writer.WriteProgramHeader(channel, parameter);
            writer.WriteTime(seconds);
        }

        /// <summary>
        /// Sends a sequence of onset times and voltages describing a train of pulses.
        /// </summary>
        /// <param name="id">The identity of the custom pulse train to program.</param>
        /// <param name="pulseTimes">
        /// The array of pulse onset times, where each time is specified in seconds
        /// from the start of the pulse train.
        /// </param>
        /// <param name="pulseVoltages">
        /// The array of pulse voltages, with one voltage per pulse.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The specified pulse train <paramref name="id"/> is invalid.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Either <paramref name="pulseTimes"/> or <paramref name="pulseVoltages"/> is
        /// <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The maximum length of 1,000 pulses has been exceeded.
        /// </exception>
        /// <remarks>
        /// Pulses that are continuous or overlapping will merge. If an output channel is set
        /// to produce biphasic pulses, the voltage specified for each pulse is sign-inverted
        /// for the second phase, i.e. if +5 V is used for phase 1, -5 V is used automatically
        /// for phase 2.
        /// </remarks>
        public void SendCustomPulseTrain(CustomTrainId id, double[] pulseTimes, double[] pulseVoltages)
        {
            if (pulseTimes == null)
            {
                throw new ArgumentNullException(nameof(pulseTimes));
            }

            if (pulseVoltages == null)
            {
                throw new ArgumentNullException(nameof(pulseVoltages));
            }

            var nPulses = (uint)pulseTimes.Length;
            if (nPulses > MaxPulseLength)
            {
                throw new ArgumentOutOfRangeException("Exceeded the maximum allowed pulse length.", nameof(pulseTimes));
            }

            if (pulseTimes.Length != pulseVoltages.Length)
            {
                throw new ArgumentException("Array of pulse voltages must be of same length as array of pulse times.", nameof(pulseVoltages));
            }

            using var writer = new CommandWriter(this);
            writer.WriteProgramPulseTrainHeader(id);
            writer.Write(nPulses);
            for (int i = 0; i < pulseTimes.Length; i++)
            {
                writer.WriteMonotonicTime(pulseTimes[i]);
            }

            for (int i = 0; i < pulseVoltages.Length; i++)
            {
                writer.WriteVoltage(pulseVoltages[i]);
            }
        }

        /// <summary>
        /// Sends a sequence of onset times and voltages describing a train of pulses.
        /// </summary>
        /// <param name="id">The identity of the custom pulse train to program.</param>
        /// <param name="pulseTrain">
        /// The array specifying all pulse onset times and voltages, respectively in
        /// seconds and volts.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The specified pulse train <paramref name="id"/> is invalid.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pulseTrain"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The maximum length of 1,000 pulses has been exceeded.
        /// </exception>
        /// <remarks>
        /// Pulses that are continuous or overlapping will merge. If an output channel is set
        /// to produce biphasic pulses, the voltage specified for each pulse is sign-inverted
        /// for the second phase, i.e. if +5 V is used for phase 1, -5 V is used automatically
        /// for phase 2.
        /// </remarks>
        public void SendCustomPulseTrain(CustomTrainId id, PulseOnset[] pulseTrain)
        {
            if (pulseTrain == null)
            {
                throw new ArgumentNullException(nameof(pulseTrain));
            }

            var nPulses = (uint)pulseTrain.Length;
            if (nPulses > MaxPulseLength)
            {
                throw new ArgumentOutOfRangeException("Exceeded the maximum allowed pulse length.", nameof(pulseTrain));
            }

            using var writer = new CommandWriter(this);
            writer.WriteProgramPulseTrainHeader(id);
            writer.Write(nPulses);
            for (int i = 0; i < pulseTrain.Length; i++)
            {
                writer.WriteMonotonicTime(pulseTrain[i].Time);
            }

            for (int i = 0; i < pulseTrain.Length; i++)
            {
                writer.WriteVoltage(pulseTrain[i].Voltage);
            }
        }

        /// <summary>
        /// Sends a sequence of onset times and voltages describing a train of pulses.
        /// </summary>
        /// <param name="id">The identity of the custom pulse train to program.</param>
        /// <param name="pulseTrain">
        /// A rectangular array of pulse times and pulse voltages, where the first row
        /// represents the vector of pulse onset times in seconds, and the second row
        /// the corresponding vector of pulse voltages in volts.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The specified pulse train <paramref name="id"/> is invalid.-or-
        /// <paramref name="pulseTrain"/> does not have exactly two rows.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pulseTrain"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The maximum length of 1,000 pulses has been exceeded.
        /// </exception>
        public void SendCustomPulseTrain(CustomTrainId id, double[,] pulseTrain)
        {
            if (pulseTrain == null)
            {
                throw new ArgumentNullException(nameof(pulseTrain));
            }

            if (pulseTrain.GetLength(0) != 2)
            {
                throw new ArgumentException("Array of pulse times and voltages must be of length two in its first dimension.", nameof(pulseTrain));
            }

            var nPulses = (uint)pulseTrain.GetLength(1);
            if (nPulses > MaxPulseLength)
            {
                throw new ArgumentOutOfRangeException("Exceeded the maximum allowed pulse length.", nameof(pulseTrain));
            }

            using var writer = new CommandWriter(this);
            writer.WriteProgramPulseTrainHeader(id);
            writer.Write(nPulses);
            for (int i = 0; i < nPulses; i++)
            {
                writer.WriteMonotonicTime(pulseTrain[0, i]);
            }

            for (int i = 0; i < nPulses; i++)
            {
                writer.WriteVoltage(pulseTrain[1, i]);
            }
        }

        /// <summary>
        /// Sends a sequence of voltages describing a train of continuous monophasic
        /// pulses, with periodic onset times.
        /// </summary>
        /// <param name="id">The identity of the custom pulse train to program.</param>
        /// <param name="samplingPeriod">
        /// The width of all pulses in the train in the range [0.0001, 3600] seconds.
        /// Pulses are continuous.
        /// </param>
        /// <param name="pulseVoltages">
        /// The array of pulse voltages, with one voltage per pulse.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="samplingPeriod"/> is outside the range [0.0001, 3600]
        /// seconds.-or-
        /// The maximum length of 1,000 pulses has been exceeded.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="pulseVoltages"/> is <see langword="null"/>.
        /// </exception>
        public void SendCustomWaveform(CustomTrainId id, double samplingPeriod, double[] pulseVoltages)
        {
            if (samplingPeriod < MinTimePeriod || samplingPeriod > MaxTimePeriod)
            {
                throw new ArgumentOutOfRangeException(nameof(samplingPeriod));
            }

            if (pulseVoltages == null)
            {
                throw new ArgumentNullException(nameof(pulseVoltages));
            }

            using var writer = new CommandWriter(this);
            writer.WriteProgramPulseTrainHeader(id);
            writer.Write((uint)pulseVoltages.Length);
            for (int i = 0; i < pulseVoltages.Length; i++)
            {
                writer.WriteMonotonicTime(samplingPeriod * i);
            }

            for (int i = 0; i < pulseVoltages.Length; i++)
            {
                writer.WriteVoltage(pulseVoltages[i]);
            }
        }

        /// <summary>
        /// Begins the stimulation pulse train on the specified output channels.
        /// </summary>
        /// <param name="channels">
        /// Specifies which output channels to start.
        /// </param>
        public void TriggerOutputChannels(ChannelTriggers channels)
        {
            using var writer = new CommandWriter(this);
            writer.Write(OpMenu);
            writer.Write(TriggerCommand);
            writer.Write((byte)channels);
        }

        /// <summary>
        /// Writes a text string to the Pulse Pal oLED display.
        /// </summary>
        /// <param name="text">
        /// A text string to display on the top row of the oLED display.
        /// Text must be less than 17 characters in length.
        /// </param>
        public void UpdateDisplay(string text)
        {
            UpdateDisplay(text, string.Empty);
        }

        /// <summary>
        /// Writes text strings to the Pulse Pal oLED display.
        /// </summary>
        /// <param name="row1">
        /// A text string to display on the top row of the oLED display.
        /// Text must be less than 17 characters in length.
        /// </param>
        /// <param name="row2">
        /// An optional text string to display on the bottom row of the oLED display.
        /// Text must be less than 17 characters in length.
        /// </param>
        public void UpdateDisplay(string row1, string row2)
        {
            const int MaxDisplayCharacters = 16;
            var textWriter = new CommandWriter(this);
            textWriter.WriteText(row1, MaxDisplayCharacters);
            if (!string.IsNullOrEmpty(row2))
            {
                textWriter.Write(LineBreak);
                textWriter.WriteText(row2, MaxDisplayCharacters);
            }

            var message = new byte[textWriter.Length];
            Array.Copy(commandBuffer, message, message.Length);

            using var writer = new CommandWriter(this);
            writer.Write(OpMenu);
            writer.Write(UpdateDisplayCommand);
            writer.Write((byte)message.Length);
            writer.Write(message);
        }

        /// <summary>
        /// Sets a constant voltage on an output channel.
        /// </summary>
        /// <param name="channel">The output channel to configure.</param>
        /// <param name="volts">
        /// The voltage to set on the output channel, in the range [-10, 10] volts.
        /// </param>
        public void SetFixedVoltage(OutputChannel channel, double volts)
        {
            using var writer = new CommandWriter(this);
            writer.Write(OpMenu);
            writer.Write(SetVoltageCommand);
            writer.Write((byte)channel);
            writer.WriteVoltage(volts);
        }

        /// <summary>
        /// Terminates all pulse trains currently playing on the device.
        /// </summary>
        public void AbortPulseTrains()
        {
            using var writer = new CommandWriter(this);
            writer.Write(OpMenu);
            writer.Write(AbortCommand);
        }

        /// <summary>
        /// Sets an output channel to play its pulse train indefinitely when triggered,
        /// without needing to be re-triggered.
        /// </summary>
        /// <param name="channel">The output channel to configure.</param>
        /// <param name="loop">
        /// <see langword="true"/> to set the output channel in continuous loop mode;
        /// <see langword="false"/> otherwise.
        /// </param>
        public void SetContinuousLoop(OutputChannel channel, bool loop)
        {
            using var writer = new CommandWriter(this);
            writer.Write(OpMenu);
            writer.Write(LoopCommand);
            writer.Write((byte)channel);
            writer.Write(loop);
        }

        /// <summary>
        /// Sets a 6-character string to indicate the connected application's name,
        /// at the top of the PulsePal's thumb joystick menu tree.
        /// </summary>
        /// <param name="id">
        /// A text string identifying the connected application. The text must be
        /// 6 or less characters in length.
        /// </param>
        public void SetClientId(string id)
        {
            using var writer = new CommandWriter(this);
            writer.Write(OpMenu);
            writer.Write(ClientIdCommand);
            for (int i = 0; i < 6; i++)
            {
                writer.Write(i < id.Length ? (byte)id[i] : (byte)' ');
            }
        }

        /// <summary>
        /// Closes the port connection, sets the <see cref="IsOpen"/>
        /// property to <see langword="false"/> and disposes of the
        /// internal <see cref="SerialPort"/> object.
        /// </summary>
        public void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Disconnect();
                    serialPort.Close();
                    initialized.Dispose();
                    disposed = true;
                }
            }
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        struct CommandWriter : IDisposable
        {
            int offset;
            double previousTime;
            readonly PulsePalDevice device;

            public CommandWriter(PulsePalDevice pulsePal)
            {
                offset = 0;
                previousTime = -MinTimePeriod;
                device = pulsePal;
            }

            public readonly int Length => offset;

            public void Write(byte value)
            {
                device.commandBuffer[offset++] = value;
            }

            public void Write(bool value)
            {
                device.commandBuffer[offset++] = (byte)(value ? 1 : 0);
            }

            public void Write(ushort value)
            {
                device.commandBuffer[offset++] = (byte)value;
                device.commandBuffer[offset++] = (byte)(value >> 8);
            }

            public void Write(uint value)
            {
                device.commandBuffer[offset++] = (byte)value;
                device.commandBuffer[offset++] = (byte)(value >> 8);
                device.commandBuffer[offset++] = (byte)(value >> 16);
                device.commandBuffer[offset++] = (byte)(value >> 24);
            }

            public void Write(byte[] bytes)
            {
                Array.Copy(bytes, 0, device.commandBuffer, offset, bytes.Length);
                offset += bytes.Length;
            }

            public void WriteTime(double seconds)
            {
                var cycles = GetTimeCycles((decimal)seconds);
                ThrowIfCyclesOutOfRange(cycles, nameof(seconds));
                Write(cycles);
            }

            public void WriteMonotonicTime(double seconds)
            {
                if (seconds - previousTime < MinTimePeriod)
                {
                    ThrowAndReset(new ArgumentException(
                        "The array of pulse times must be monotonically increasing.",
                        nameof(seconds)));
                }

                WriteTime(seconds);
                previousTime = seconds;
            }

            public void WriteVoltage(double volts)
            {
                if (volts < MinVoltage || volts > MaxVoltage)
                {
                    ThrowAndReset(new ArgumentOutOfRangeException(nameof(volts)));
                }

                var steps = GetVoltageSteps((decimal)volts);
                if (device.dacMaxValue > byte.MaxValue)
                {
                    Write((ushort)steps);
                }
                else Write((byte)steps);
            }

            public void WriteText(string text, int maxChars)
            {
                for (int i = 0; i < text.Length && i < maxChars; i++)
                {
                    Write((byte)text[i]);
                }
            }

            public void WriteProgramHeader(OutputChannel channel, ParameterCode parameter)
            {
                if (channel == 0)
                {
                    ThrowAndReset(new ArgumentOutOfRangeException(nameof(channel)));
                }

                Write(OpMenu);
                Write(ProgramParam);
                Write((byte)parameter);
                Write((byte)channel);
            }

            public void WriteProgramPulseTrainHeader(CustomTrainId id)
            {
                var command = id switch
                {
                    CustomTrainId.CustomTrain1 => ProgramPulseTrain1,
                    CustomTrainId.CustomTrain2 => ProgramPulseTrain2,
                    _ => ThrowAndReset<byte>(new ArgumentException("Invalid pulse train id.", nameof(id)))
                };

                Write(OpMenu);
                Write(command);
            }

            readonly int GetVoltageSteps(decimal volts)
            {
                return (int)decimal.Ceiling((volts + 10) / 20 * device.dacMaxValue);
            }

            readonly uint GetTimeCycles(decimal seconds)
            {
                return (uint)(seconds * CycleFrequency);
            }

            void ThrowIfCyclesOutOfRange(uint cycles, string paramName)
            {
                if (cycles > MaxCyclePeriod)
                {
                    ThrowAndReset(new ArgumentOutOfRangeException(
                        "The specified value exceeds the maximum allowed Pulse Pal time interval.",
                        paramName));
                }
            }

            void ThrowAndReset(Exception ex)
            {
                offset = 0;
                throw ex;
            }

            TResult ThrowAndReset<TResult>(Exception ex)
            {
                offset = 0;
                throw ex;
            }

            public void Dispose()
            {
                if (offset > 0)
                {
                    device.serialPort.Write(device.commandBuffer, 0, offset);
                    offset = 0;
                }
            }
        }
    }
}
