using System;
using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents pulse train configuration parameters for an output channel on a
    /// Pulse Pal device.
    /// </summary>
    [TypeConverter(typeof(OutputChannelConfigurationConverter))]
    public class OutputChannelConfiguration : OutputChannelParameterConfiguration
    {
        const string VoltageCategory = "Pulse Voltage";
        const string TimingCategory = "Pulse Timing";
        const string CustomTrainCategory = "Custom Train";
        const string TriggerCategory = "Pulse Trigger";

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

        /// <inheritdoc/>
        public override void Configure(PulsePalDevice pulsePal)
        {
            var channel = Channel;
            pulsePal.SetBiphasic(channel, Biphasic);
            pulsePal.SetPhase1Voltage(channel, Phase1Voltage);
            pulsePal.SetPhase2Voltage(channel, Phase2Voltage);
            pulsePal.SetPulseTrainDelay(channel, PulseTrainDelay);
            pulsePal.SetPhase1Duration(channel, Phase1Duration);
            pulsePal.SetInterPhaseInterval(channel, InterPhaseInterval);
            pulsePal.SetPhase2Duration(channel, Phase2Duration);
            pulsePal.SetInterPulseInterval(channel, InterPulseInterval);
            pulsePal.SetBurstDuration(channel, BurstDuration);
            pulsePal.SetInterBurstInterval(channel, InterBurstInterval);
            pulsePal.SetPulseTrainDuration(channel, PulseTrainDuration);
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
            return Channel == 0 ? nameof(OutputChannelConfiguration) : $"{Channel}";
        }

        class OutputChannelConfigurationConverter : ExpandableObjectConverter
        {
            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                return base.GetProperties(context, value, attributes)
                           .Sort(new[]
                           {
                               nameof(PulseTrainDelay),
                               nameof(Phase1Duration),
                               nameof(InterPhaseInterval),
                               nameof(Phase2Duration),
                               nameof(InterPulseInterval),
                               nameof(BurstDuration),
                               nameof(InterBurstInterval),
                               nameof(PulseTrainDuration)
                           });
            }
        }
    }
}
