using System;
using System.IO.Ports;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace Bonsai.PulsePal
{
    public sealed class PulsePal : IDisposable
    {
        public const int BaudRate = 115200;
        const int MaxDataBytes = 35;

        const byte Acknowledge           = 0x4B;
        const byte OpMenu                = 0xD5;
        const byte HandshakeCommand      = 0x48;
        const byte ProgramParamCommand   = 0x4A;
        const byte PulseTrain1Command    = 0x4B;
        const byte PulseTrain2Command    = 0x4C;
        const byte TriggerCommand        = 0x4D;
        const byte SetDisplayCommand     = 0x4E;
        const byte SetVoltageCommand     = 0x4F;
        const byte AbortCommand          = 0x50;
        const byte DisconnectCommand     = 0x51;
        const byte LoopCommand           = 0x52;
        const byte ClientIdCommand       = 0x59;
        const byte LineBreak             = 0xFE;

        bool disposed;
        bool initialized;
        readonly SerialPort serialPort;
        readonly byte[] responseBuffer;
        readonly byte[] commandBuffer;
        readonly byte[] readBuffer;

        public PulsePal(string portName)
        {
            serialPort = new SerialPort(portName);
            serialPort.BaudRate = BaudRate;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.Parity = Parity.None;
            serialPort.DtrEnable = false;
            serialPort.RtsEnable = true;

            responseBuffer = new byte[4];
            commandBuffer = new byte[MaxDataBytes];
            readBuffer = new byte[serialPort.ReadBufferSize];
        }

        public int MajorVersion { get; private set; }

        public int MinorVersion { get; private set; }

        public bool IsOpen
        {
            get { return serialPort.IsOpen; }
        }

        Task RunAsync(CancellationToken cancellationToken)
        {
            serialPort.Open();
            serialPort.ReadExisting();
            Connect();

            return Task.Factory.StartNew(() =>
            {
                using var cancellation = cancellationToken.Register(serialPort.Dispose);
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var bytesToRead = serialPort.BytesToRead;
                        if (bytesToRead == 0)
                        {
                            var nextByte = serialPort.ReadByte();
                            if (nextByte < 0) break;
                            ProcessInput((byte)nextByte);
                        }
                        else
                        {
                            while (bytesToRead > 0)
                            {
                                var bytesRead = serialPort.Read(readBuffer, 0, Math.Min(bytesToRead, readBuffer.Length));
                                for (int i = 0; i < bytesRead; i++)
                                {
                                    ProcessInput(readBuffer[i]);
                                }
                                bytesToRead -= bytesRead;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        if (!cancellationToken.IsCancellationRequested)
                        {
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
        }

        void Connect()
        {
            commandBuffer[0] = OpMenu;
            commandBuffer[1] = HandshakeCommand;
            serialPort.Write(commandBuffer, 0, 2);
        }

        public int HandshakeForVersion()
        {
            Connect();
            serialPort.Read(readBuffer, 0, 5); // Read 5 bytes after handshake, ack character followed by 
            int firmwareBuild = BitConverter.ToInt32(readBuffer, 1);
            return firmwareBuild;
        }

        void Disconnect()
        {
            commandBuffer[0] = OpMenu;
            commandBuffer[1] = DisconnectCommand;
            serialPort.Write(commandBuffer, 0, 2);
        }

        void WriteInt(BinaryWriter writer, int value)
        {
            writer.Write((byte)value);
            writer.Write((byte)(value >> 8));
            writer.Write((byte)(value >> 16));
            writer.Write((byte)(value >> 24));
        }

        public void ProgramParameter(int channel, ParameterCode parameter, int value)
        {
            using (var stream = new MemoryStream(commandBuffer))
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(OpMenu);
                writer.Write(ProgramParamCommand);
                writer.Write((byte)parameter);
                writer.Write((byte)channel);
                if (parameter >= ParameterCode.Phase1Duration
                    && parameter < ParameterCode.PulseTrainDelay
                    || parameter == ParameterCode.RestingVoltage)
                {
                    WriteInt(writer, value);
                }
                else writer.Write((byte)value);
                serialPort.Write(commandBuffer, 0, (int)stream.Length);
            }
        }

        public void SendCustomPulseTrain(int id, int[] pulseTimes, byte[] pulseVoltages)
        {
            if (id < 1 || id > 2)
            {
                throw new ArgumentException("Pulse train id must be either 1 or 2.", "id");
            }

            if (pulseTimes == null)
            {
                throw new ArgumentNullException("pulseTimes");
            }

            if (pulseVoltages == null)
            {
                throw new ArgumentNullException("pulseVoltages");
            }

            var nPulses = pulseTimes.Length;
            if (nPulses > 1000)
            {
                throw new ArgumentException("Exceeded the maximum allowed number of pulses.", "pulseTimes");
            }

            if (pulseTimes.Length != pulseVoltages.Length)
            {
                throw new ArgumentException("Pulse voltages array must be of same length as pulse times.", "pulseVoltages");
            }

            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(OpMenu);
                writer.Write(id == 1 ? PulseTrain1Command : PulseTrain2Command);
                writer.Write((byte)0);
                WriteInt(writer, nPulses);

                for (int i = 0; i < pulseTimes.Length; i++)
                {
                    WriteInt(writer, pulseTimes[i]);
                }

                for (int i = 0; i < pulseVoltages.Length; i++)
                {
                    writer.Write(pulseVoltages[i]);
                }

                var command = stream.GetBuffer();
                serialPort.Write(command, 0, (int)stream.Length);
            }
        }

        public void TriggerOutputChannels(byte channels)
        {
            commandBuffer[0] = OpMenu;
            commandBuffer[1] = TriggerCommand;
            commandBuffer[2] = channels;
            serialPort.Write(commandBuffer, 0, 3);
        }

        int WriteText(string text, int index)
        {
            var i = 0;
            for (; i < text.Length && i < 16; i++)
            {
                commandBuffer[i + index] = (byte)text[i];
            }

            return index + i;
        }

        public void SetDisplay(string text)
        {
            SetDisplay(text, string.Empty);
        }

        public void SetDisplay(string row1, string row2)
        {
            var index = 0;
            commandBuffer[index++] = OpMenu;
            commandBuffer[index++] = SetDisplayCommand;
            index = WriteText(row1, index);
            if (!string.IsNullOrEmpty(row2))
            {
                commandBuffer[index++] = LineBreak;
                WriteText(row2, index);
            }
        }

        public void SetFixedVoltage(byte channel, byte voltage)
        {
            commandBuffer[0] = OpMenu;
            commandBuffer[1] = SetVoltageCommand;
            commandBuffer[2] = channel;
            commandBuffer[3] = voltage;
            serialPort.Write(commandBuffer, 0, 4);
        }

        public void AbortPulseTrains()
        {
            commandBuffer[0] = OpMenu;
            commandBuffer[1] = AbortCommand;
            serialPort.Write(commandBuffer, 0, 2);
        }

        public void SetContinuousLoop(byte channel, bool loop)
        {
            commandBuffer[0] = OpMenu;
            commandBuffer[1] = LoopCommand;
            commandBuffer[2] = channel;
            commandBuffer[3] = (byte)(loop ? 1 : 0);
            serialPort.Write(commandBuffer, 0, 4);
        }

        public void SetClientId(string id)
        {
            commandBuffer[0] = OpMenu;
            commandBuffer[1] = ClientIdCommand;
            for (int i = 0; i < 6; i++)
            {
                commandBuffer[i + 2] = i < id.Length ? (byte)id[i] : (byte)' ';
            }
            serialPort.Write(commandBuffer, 0, 8);
        }

        void SetVersion(int majorVersion, int minorVersion)
        {
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
        }

        // TODO - Process input could be extended to allow for byte[], e.g. for handshake returns like K20 etc.
        void ProcessInput(byte inputData)
        {
            if (!initialized && inputData != Acknowledge)
            {
                throw new InvalidOperationException("Unexpected return value from PulsePal.");
            }

            switch (inputData)
            {
                case Acknowledge:
                    initialized = true;
                    break;
                default:
                    break;
            }
        }

        public void Close()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PulsePal()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Disconnect();
                    serialPort.Close();
                    disposed = true;
                }
            }
        }

        void IDisposable.Dispose()
        {
            Close();
        }
    }
}
