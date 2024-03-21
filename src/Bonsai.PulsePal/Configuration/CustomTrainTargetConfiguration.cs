using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents configuration parameters specifying the interpretation of pulse times
    /// in the custom train configured on this output channel.
    /// </summary>
    [DisplayName(nameof(ParameterCode.CustomTrainTarget))]
    [Description("Specifies the interpretation of pulse times in the channel custom pulse train.")]
    public class CustomTrainTargetConfiguration : OutputChannelParameterConfiguration
    {
        /// <summary>
        /// Gets or sets a value specifying the interpretation of pulse times in the
        /// custom pulse train.
        /// </summary>
        [Description("Specifies the interpretation of pulse times in the custom pulse train.")]
        public CustomTrainTarget CustomTrainTarget { get; set; }

        /// <inheritdoc/>
        public override void Configure(PulsePalDevice device)
        {
            device.SetCustomTrainTarget(Channel, CustomTrainTarget);
        }
    }
}
