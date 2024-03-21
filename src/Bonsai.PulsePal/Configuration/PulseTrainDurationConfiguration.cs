using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents configuration parameters specifying the duration of the
    /// entire pulse train.
    /// </summary>
    [DisplayName(nameof(ParameterCode.PulseTrainDuration))]
    [Description("Specifies the duration of the entire pulse train.")]
    public class PulseTrainDurationConfiguration : OutputChannelParameterConfiguration
    {
        /// <summary>
        /// Gets or sets the duration of the pulse train, in the range
        /// [0.0001, 3600] seconds.
        /// </summary>
        [Range(MinTimePeriod, MaxTimePeriod)]
        [Precision(TimeDecimalPlaces, MinTimePeriod)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The duration of the pulse train, in seconds.")]
        public double PulseTrainDuration { get; set; } = MinTimePeriod;

        /// <inheritdoc/>
        public override void Configure(PulsePalDevice device)
        {
            device.SetPulseTrainDuration(Channel, PulseTrainDuration);
        }
    }
}
