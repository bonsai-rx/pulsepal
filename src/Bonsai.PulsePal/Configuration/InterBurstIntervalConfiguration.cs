using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents configuration parameters specifying the duration of the
    /// off-time between bursts.
    /// </summary>
    [DisplayName(nameof(ParameterCode.InterBurstInterval))]
    [Description("Specifies the duration of the off-time between bursts.")]
    public class InterBurstIntervalConfiguration : OutputChannelParameterConfiguration
    {
        /// <summary>
        /// Gets or sets the duration of the off-time between bursts, in the range
        /// [0.0001, 3600] seconds.
        /// </summary>
        [Range(MinTimePeriod, MaxTimePeriod)]
        [Precision(TimeDecimalPlaces, MinTimePeriod)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The duration of the off-time between bursts, in seconds.")]
        public double InterBurstInterval { get; set; } = MinTimePeriod;

        /// <inheritdoc/>
        public override void Configure(PulsePalDevice device)
        {
            device.SetInterBurstInterval(Channel, InterBurstInterval);
        }
    }
}
