using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents configuration parameters specifying whether trigger channel 1 can
    /// trigger the output channel.
    /// </summary>
    [DisplayName(nameof(ParameterCode.TriggerOnChannel1))]
    [Description("Specifies whether trigger channel 1 can trigger the output channel.")]
    public class TriggerOnChannel1Configuration : OutputChannelParameterConfiguration
    {
        /// <summary>
        /// Gets or sets a value specifying whether trigger channel 1 can trigger
        /// this output channel.
        /// </summary>
        [Description("Specifies whether trigger channel 1 can trigger this output channel.")]
        public bool TriggerOnChannel1 { get; set; }

        /// <inheritdoc/>
        public override void Configure(PulsePalDevice device)
        {
            device.SetTriggerOnChannel1(Channel, TriggerOnChannel1);
        }
    }
}
