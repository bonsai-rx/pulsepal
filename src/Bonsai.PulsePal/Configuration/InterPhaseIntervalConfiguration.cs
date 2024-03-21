using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents configuration parameters specifying the interval between the first
    /// and second phases of biphasic pulses.
    /// </summary>
    [DisplayName(nameof(ParameterCode.InterPhaseInterval))]
    [Description("Specifies the interval between the first and second phases of biphasic pulses.")]
    public class InterPhaseIntervalConfiguration : OutputChannelParameterConfiguration
    {
        /// <summary>
        /// Gets or sets the interval between the first and second phase of a biphasic pulse,
        /// in the range [0.0001, 3600] seconds.
        /// </summary>
        [Range(MinTimePeriod, MaxTimePeriod)]
        [Precision(TimeDecimalPlaces, MinTimePeriod)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The interval between the first and second phase of a biphasic pulse, in seconds.")]
        public double InterPhaseInterval { get; set; } = MinTimePeriod;

        /// <inheritdoc/>
        public override void Configure(PulsePalDevice device)
        {
            device.SetInterPhaseInterval(Channel, InterPhaseInterval);
        }
    }
}
