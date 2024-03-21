using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents configuration parameters specifying whether the output channel
    /// will loop its custom pulse train.
    /// </summary>
    [DisplayName(nameof(ParameterCode.CustomTrainLoop))]
    [Description("Specifies whether the output channel will loop its custom pulse train.")]
    public class CustomTrainLoopConfiguration : OutputChannelParameterConfiguration
    {
        /// <summary>
        /// Gets or sets a value specifying whether the output channel
        /// will loop its custom pulse train.
        /// </summary>
        [Description("Specifies whether the output channel will loop its custom pulse train.")]
        public bool CustomTrainLoop { get; set; }

        /// <inheritdoc/>
        public override void Configure(PulsePalDevice device)
        {
            device.SetCustomTrainLoop(Channel, CustomTrainLoop);
        }
    }
}
