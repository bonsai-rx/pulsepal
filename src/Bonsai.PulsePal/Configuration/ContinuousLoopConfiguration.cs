using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents configuration parameters specifying whether to play this output channel
    /// pulse train indefinitely when triggered, without needing to be re-triggered.
    /// </summary>
    [DisplayName(nameof(ContinuousLoop))]
    [Description("Specifies whether to play the output channel pulse train indefinitely when triggered, without needing to be re-triggered.")]
    public class ContinuousLoopConfiguration : OutputChannelParameterConfiguration
    {
        /// <summary>
        /// Gets or sets a value specifying whether to set the output channel in
        /// continuous loop mode.
        /// </summary>
        [Description("Specifies whether to set the output channel in continuous loop mode.")]
        public bool ContinuousLoop { get; set; }

        /// <inheritdoc/>
        public override void Configure(PulsePalDevice device)
        {
            device.SetContinuousLoop(Channel, ContinuousLoop);
        }
    }
}
