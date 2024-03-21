using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents configuration parameters specifying the behavior of a trigger channel.
    /// </summary>
    [DisplayName(nameof(ParameterCode.TriggerMode))]
    [Description("Specifies the behavior of a trigger channel.")]
    public class TriggerModeConfiguration : TriggerChannelParameterConfiguration
    {
        /// <summary>
        /// Gets or sets a value specifying the behavior of a trigger channel.
        /// </summary>
        [Description("Specifies the behavior of a trigger channel.")]
        public TriggerMode TriggerMode { get; set; }

        /// <inheritdoc/>
        public override void Configure(PulsePalDevice device)
        {
            device.SetTriggerMode(Channel, TriggerMode);
        }
    }
}
