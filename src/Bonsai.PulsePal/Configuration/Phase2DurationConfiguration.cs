using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents configuration parameters specifying the duration for the second
    /// phase of each pulse.
    /// </summary>
    [DisplayName(nameof(ParameterCode.Phase2Duration))]
    [Description("Specifies the duration for the second phase of each pulse.")]
    public class Phase2DurationConfiguration : OutputChannelParameterConfiguration
    {
        /// <summary>
        /// Gets or sets the duration of the second phase of the pulse, in the range
        /// [0.0001, 3600] seconds.
        /// </summary>
        [Range(MinTimePeriod, MaxTimePeriod)]
        [Precision(TimeDecimalPlaces, MinTimePeriod)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The duration of the second phase of the pulse, in seconds.")]
        public double Phase2Duration { get; set; } = MinTimePeriod;

        /// <inheritdoc/>
        public override void Configure(PulsePalDevice device)
        {
            device.SetPhase2Duration(Channel, Phase2Duration);
        }
    }
}
