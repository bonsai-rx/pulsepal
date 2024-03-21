using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents configuration parameters specifying the duration of a pulse
    /// burst when using burst mode.
    /// </summary>
    [DisplayName(nameof(ParameterCode.BurstDuration))]
    [Description("Specifies the duration of a pulse burst when using burst mode.")]
    public class BurstDurationConfiguration : OutputChannelParameterConfiguration
    {
        /// <summary>
        /// Gets or sets the duration of a pulse burst, in the range
        /// [0.0001, 3600] seconds.
        /// </summary>
        [Range(MinTimePeriod, MaxTimePeriod)]
        [Precision(TimeDecimalPlaces, MinTimePeriod)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The duration of a pulse burst, in seconds.")]
        public double BurstDuration { get; set; } = MinTimePeriod;

        /// <inheritdoc/>
        public override void Configure(PulsePalDevice device)
        {
            device.SetBurstDuration(Channel, BurstDuration);
        }
    }
}
