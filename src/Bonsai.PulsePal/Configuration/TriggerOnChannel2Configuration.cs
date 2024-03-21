using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents configuration parameters specifying whether trigger channel 2 can
    /// trigger the output channel.
    /// </summary>
    [DisplayName(nameof(ParameterCode.TriggerOnChannel2))]
    [Description("Specifies whether trigger channel 2 can trigger the output channel.")]
    public class TriggerOnChannel2Configuration : OutputChannelParameterConfiguration
    {
        /// <summary>
        /// Gets or sets a value specifying whether trigger channel 2 can trigger
        /// this output channel.
        /// </summary>
        [Description("Specifies whether trigger channel 2 can trigger this output channel.")]
        public bool TriggerOnChannel2 { get; set; }

        /// <inheritdoc/>
        public override void Configure(PulsePalDevice device)
        {
            device.SetTriggerOnChannel2(Channel, TriggerOnChannel2);
        }
    }
}
