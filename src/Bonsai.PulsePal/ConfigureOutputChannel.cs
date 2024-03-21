using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents an operator that configures output channel parameters on a
    /// Pulse Pal device.
    /// </summary>
    [Description("Configures output channel parameters on a Pulse Pal device.")]
    public class ConfigureOutputChannel : Sink, INamedElement
    {
        const string ChannelCategory = "Channel";
        const string VoltageCategory = "Pulse Voltage";
        const string TimingCategory = "Pulse Timing";
        const string CustomTrainCategory = "Custom Train";
        const string TriggerCategory = "Pulse Trigger";
        const double MinVoltage = PulsePalDevice.MinVoltage;
        const double MaxVoltage = PulsePalDevice.MaxVoltage;
        const double MinTimePeriod = PulsePalDevice.MinTimePeriod;
        const double MaxTimePeriod = PulsePalDevice.MaxTimePeriod;
        const int VoltageDecimalPlaces = OutputChannelParameterConfiguration.VoltageDecimalPlaces;
        const double VoltageIncrement = OutputChannelParameterConfiguration.VoltageIncrement;
        const int TimeDecimalPlaces = OutputChannelParameterConfiguration.TimeDecimalPlaces;

        string INamedElement.Name => Channel == 0
            ? nameof(ConfigureOutputChannel)
            : $"ConfigureOutput{Channel}";

        /// <summary>
        /// Gets or sets the name of the Pulse Pal device.
        /// </summary>
        [Category(ChannelCategory)]
        [TypeConverter(typeof(DeviceNameConverter))]
        [Description("The name of the Pulse Pal device.")]
        public string DeviceName { get; set; } = nameof(PulsePal);

        /// <summary>
        /// Gets or sets the output channel to configure.
        /// </summary>
        [Category(ChannelCategory)]
        [Description("The output channel to configure.")]
        public OutputChannel Channel { get; set; }

        /// <summary>
        /// Gets or sets a value specifying whether to use biphasic or
        /// monophasic pulses.
        /// </summary>
        [Category(VoltageCategory)]
        [Description("Specifies whether to use biphasic or monophasic pulses.")]
        public bool Biphasic { get; set; }

        /// <summary>
        /// Gets or sets the voltage for the first phase of each pulse.
        /// </summary>
        [Category(VoltageCategory)]
        [Range(MinVoltage, MaxVoltage)]
        [Precision(VoltageDecimalPlaces, VoltageIncrement)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The voltage for the first phase of each pulse.")]
        public double Phase1Voltage { get; set; }

        /// <summary>
        /// Gets or sets the voltage for the second phase of each pulse.
        /// </summary>
        [Category(VoltageCategory)]
        [Range(MinVoltage, MaxVoltage)]
        [Precision(VoltageDecimalPlaces, VoltageIncrement)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The voltage for the second phase of each pulse.")]
        public double Phase2Voltage { get; set; }

        /// <summary>
        /// Gets or sets the resting voltage, in the range [-10, 10] volts.
        /// </summary>
        [Category(VoltageCategory)]
        [Range(MinVoltage, MaxVoltage)]
        [Precision(VoltageDecimalPlaces, VoltageIncrement)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The resting voltage.")]
        public double RestingVoltage { get; set; }

        /// <summary>
        /// Gets or sets the duration of the first phase of the pulse, in the range
        /// [0.0001, 3600] seconds.
        /// </summary>
        [Category(TimingCategory)]
        [Range(MinTimePeriod, MaxTimePeriod)]
        [Precision(TimeDecimalPlaces, MinTimePeriod)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The duration of the first phase of the pulse, in seconds.")]
        public double Phase1Duration { get; set; } = MinTimePeriod;

        /// <summary>
        /// Gets or sets the interval between the first and second phase of a biphasic pulse,
        /// in the range [0, 3600] seconds.
        /// </summary>
        [Category(TimingCategory)]
        [Range(0, MaxTimePeriod)]
        [Precision(TimeDecimalPlaces, MinTimePeriod)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The interval between the first and second phase of a biphasic pulse, in seconds.")]
        public double InterPhaseInterval { get; set; }

        /// <summary>
        /// Gets or sets the duration of the second phase of the pulse, in the range
        /// [0.0001, 3600] seconds.
        /// </summary>
        [Category(TimingCategory)]
        [Range(MinTimePeriod, MaxTimePeriod)]
        [Precision(TimeDecimalPlaces, MinTimePeriod)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The duration of the second phase of the pulse, in seconds.")]
        public double Phase2Duration { get; set; } = MinTimePeriod;

        /// <summary>
        /// Gets or sets the interval between pulses, in the range [0.0001, 3600] seconds.
        /// </summary>
        [Category(TimingCategory)]
        [Range(MinTimePeriod, MaxTimePeriod)]
        [Precision(TimeDecimalPlaces, MinTimePeriod)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The interval between pulses, in seconds.")]
        public double InterPulseInterval { get; set; } = MinTimePeriod;

        /// <summary>
        /// Gets or sets the duration of a pulse burst, in the range
        /// [0, 3600] seconds. If set to zero, bursts are disabled.
        /// </summary>
        [Category(TimingCategory)]
        [Range(0, MaxTimePeriod)]
        [Precision(TimeDecimalPlaces, MinTimePeriod)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The duration of a pulse burst, in seconds. If set to zero, bursts are disabled.")]
        public double BurstDuration { get; set; }

        /// <summary>
        /// Gets or sets the duration of the off-time between bursts, in the range
        /// [0.0001, 3600] seconds.
        /// </summary>
        [Category(TimingCategory)]
        [Range(MinTimePeriod, MaxTimePeriod)]
        [Precision(TimeDecimalPlaces, MinTimePeriod)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The duration of the off-time between bursts, in seconds.")]
        public double InterBurstInterval { get; set; } = MinTimePeriod;

        /// <summary>
        /// Gets or sets the duration of the pulse train, in the range
        /// [0.0001, 3600] seconds.
        /// </summary>
        [Category(TimingCategory)]
        [Range(MinTimePeriod, MaxTimePeriod)]
        [Precision(TimeDecimalPlaces, MinTimePeriod)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The duration of the pulse train, in seconds.")]
        public double PulseTrainDuration { get; set; } = MinTimePeriod;

        /// <summary>
        /// Gets or sets the delay to start the pulse train, in the range
        /// [0.0001, 3600] seconds.
        /// </summary>
        [Category(TimingCategory)]
        [Range(MinTimePeriod, MaxTimePeriod)]
        [Precision(TimeDecimalPlaces, MinTimePeriod)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The delay to start the pulse train, in seconds.")]
        public double PulseTrainDelay { get; set; } = MinTimePeriod;

        /// <summary>
        /// Gets or sets a value specifying the identity of the custom pulse train
        /// to use on this output channel.
        /// </summary>
        [Category(CustomTrainCategory)]
        [Description("Specifies the identity of the custom pulse train to use on this output channel.")]
        public CustomTrainId CustomTrainIdentity { get; set; }

        /// <summary>
        /// Gets or sets a value specifying the interpretation of pulse times in the
        /// custom pulse train.
        /// </summary>
        [Category(CustomTrainCategory)]
        [Description("Specifies the interpretation of pulse times in the custom pulse train.")]
        public CustomTrainTarget CustomTrainTarget { get; set; }

        /// <summary>
        /// Gets or sets a value specifying whether the output channel
        /// will loop its custom pulse train.
        /// </summary>
        [Category(CustomTrainCategory)]
        [Description("Specifies whether the output channel will loop its custom pulse train.")]
        public bool CustomTrainLoop { get; set; }

        /// <summary>
        /// Gets or sets a value specifying whether trigger channel 1 can trigger
        /// this output channel.
        /// </summary>
        [Category(TriggerCategory)]
        [Description("Specifies whether trigger channel 1 can trigger this output channel.")]
        public bool TriggerOnChannel1 { get; set; }

        /// <summary>
        /// Gets or sets a value specifying whether trigger channel 2 can trigger
        /// this output channel.
        /// </summary>
        [Category(TriggerCategory)]
        [Description("Specifies whether trigger channel 2 can trigger this output channel.")]
        public bool TriggerOnChannel2 { get; set; }

        /// <summary>
        /// Gets or sets a value specifying whether to set the output channel in
        /// continuous loop mode.
        /// </summary>
        [Category(TriggerCategory)]
        [Description("Specifies whether to set the output channel in continuous loop mode.")]
        public bool ContinuousLoop { get; set; }

        /// <summary>
        /// Configures the output channel parameters on the Pulse Pal device whenever
        /// an observable sequence emits a notification.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the <paramref name="source"/> sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence containing the notifications used to trigger configuration
        /// of the Pulse Pal output channel.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of configuring the
        /// output channel parameters on the Pulse Pal device whenever the sequence
        /// emits a notification.
        /// </returns>
        public override IObservable<TSource> Process<TSource>(IObservable<TSource> source)
        {
            return Observable.Using(
                () => PulsePalManager.ReserveConnection(DeviceName),
                connection => source.Do(input =>
                {
                    lock (connection.PulsePal)
                    {
                        Configure(connection.PulsePal);
                    }
                }));
        }

        /// <summary>
        /// Configures the output channel parameters on every Pulse Pal device
        /// in the observable sequence.
        /// </summary>
        /// <param name="source">
        /// The sequence of Pulse Pal devices to configure.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of configuring the
        /// output channel parameters on each Pulse Pal device.
        /// </returns>
        public IObservable<PulsePalDevice> Process(IObservable<PulsePalDevice> source)
        {
            return source.Do(Configure);
        }

        internal void Configure(PulsePalDevice pulsePal)
        {
            var channel = Channel;
            pulsePal.SetBiphasic(channel, Biphasic);
            pulsePal.SetPhase1Voltage(channel, Phase1Voltage);
            pulsePal.SetPhase2Voltage(channel, Phase2Voltage);
            pulsePal.SetPhase1Duration(channel, Phase1Duration);
            pulsePal.SetInterPhaseInterval(channel, InterPhaseInterval);
            pulsePal.SetPhase2Duration(channel, Phase2Duration);
            pulsePal.SetInterPulseInterval(channel, InterPulseInterval);
            pulsePal.SetBurstDuration(channel, BurstDuration);
            pulsePal.SetInterBurstInterval(channel, InterBurstInterval);
            pulsePal.SetPulseTrainDuration(channel, PulseTrainDuration);
            pulsePal.SetPulseTrainDelay(channel, PulseTrainDelay);
            pulsePal.SetTriggerOnChannel1(channel, TriggerOnChannel1);
            pulsePal.SetTriggerOnChannel2(channel, TriggerOnChannel2);
            pulsePal.SetCustomTrainIdentity(channel, CustomTrainIdentity);
            pulsePal.SetCustomTrainTarget(channel, CustomTrainTarget);
            pulsePal.SetCustomTrainLoop(channel, CustomTrainLoop);
            pulsePal.SetRestingVoltage(channel, RestingVoltage);
            pulsePal.SetContinuousLoop(channel, ContinuousLoop);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Channel == 0 ? nameof(ConfigureOutputChannel) : $"{Channel}";
        }
    }
}
