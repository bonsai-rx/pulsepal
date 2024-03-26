using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents configuration parameters for a trigger channel on a Pulse Pal device.
    /// </summary>
    public class TriggerChannelConfiguration : TriggerChannelParameterConfiguration
    {
        /// <summary>
        /// Gets or sets a value specifying the behavior of the trigger channel.
        /// </summary>
        [Category(ChannelCategory)]
        [Description("Specifies the behavior of the trigger channel.")]
        public TriggerMode TriggerMode { get; set; }

        /// <inheritdoc/>
        public override void Configure(PulsePalDevice pulsePal)
        {
            var channel = Channel;
            pulsePal.SetTriggerMode(channel, TriggerMode);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Channel == 0 ? nameof(TriggerChannelConfiguration) : $"{Channel}";
        }
    }
}
