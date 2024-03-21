using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents configuration parameters specifying whether the channel
    /// will produce either monophasic or biphasic square pulses.
    /// </summary>
    [DisplayName(nameof(ParameterCode.Biphasic))]
    [Description("Specifies whether the channel will generate monophasic or biphasic square pulses.")]
    public class BiphasicConfiguration : OutputChannelParameterConfiguration
    {
        /// <summary>
        /// Gets or sets a value specifying whether to use biphasic or
        /// monophasic pulses.
        /// </summary>
        [Description("Specifies whether to use biphasic or monophasic pulses.")]
        public bool Biphasic { get; set; }

        /// <inheritdoc/>
        public override void Configure(PulsePalDevice device)
        {
            device.SetBiphasic(Channel, Biphasic);
        }
    }
}
