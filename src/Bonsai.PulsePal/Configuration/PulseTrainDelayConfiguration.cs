using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents configuration parameters specifying the delay between the arrival of
    /// a trigger and when the channel begins its pulse train.
    /// </summary>
    [DisplayName(nameof(ParameterCode.PulseTrainDelay))]
    [Description("Specifies the delay between the arrival of a trigger and when the channel begins its pulse train.")]
    public class PulseTrainDelayConfiguration : OutputChannelParameterConfiguration
    {
        /// <summary>
        /// Gets or sets the delay to start the pulse train, in the range
        /// [0.0001, 3600] seconds.
        /// </summary>
        [Range(MinTimePeriod, MaxTimePeriod)]
        [Precision(TimeDecimalPlaces, MinTimePeriod)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The delay to start the pulse train, in seconds.")]
        public double PulseTrainDelay { get; set; } = MinTimePeriod;

        /// <inheritdoc/>
        public override void Configure(PulsePalDevice device)
        {
            device.SetPulseTrainDelay(Channel, PulseTrainDelay);
        }
    }
}
