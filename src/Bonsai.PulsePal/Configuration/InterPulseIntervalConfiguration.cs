using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents configuration parameters specifying the interval between pulses.
    /// </summary>
    [DisplayName(nameof(ParameterCode.InterPulseInterval))]
    [Description("Specifies the interval between pulses.")]
    public class InterPulseIntervalConfiguration : OutputChannelParameterConfiguration
    {
        /// <summary>
        /// Gets or sets the interval between pulses, in the range [0.0001, 3600] seconds.
        /// </summary>
        [Range(MinTimePeriod, MaxTimePeriod)]
        [Precision(TimeDecimalPlaces, MinTimePeriod)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The interval between pulses, in seconds.")]
        public double InterPulseInterval { get; set; } = MinTimePeriod;

        /// <inheritdoc/>
        public override void Configure(PulsePalDevice device)
        {
            device.SetInterPulseInterval(Channel, InterPulseInterval);
        }
    }
}
