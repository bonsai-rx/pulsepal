using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents configuration parameters specifying the duration for the first
    /// phase of each pulse.
    /// </summary>
    [DisplayName(nameof(ParameterCode.Phase1Duration))]
    [Description("Specifies the duration for the first phase of each pulse.")]
    public class Phase1DurationConfiguration : OutputChannelParameterConfiguration
    {
        /// <summary>
        /// Gets or sets the duration of the first phase of the pulse, in the range
        /// [0.0001, 3600] seconds.
        /// </summary>
        [Range(MinTimePeriod, MaxTimePeriod)]
        [Precision(TimeDecimalPlaces, MinTimePeriod)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The duration of the first phase of the pulse, in seconds.")]
        public double Phase1Duration { get; set; } = MinTimePeriod;

        /// <inheritdoc/>
        public override void Configure(PulsePalDevice device)
        {
            device.SetPhase1Duration(Channel, Phase1Duration);
        }
    }
}
