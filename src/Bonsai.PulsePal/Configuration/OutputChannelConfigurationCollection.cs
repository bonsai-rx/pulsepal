using System.Collections.ObjectModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents a collection of output channel configuration objects.
    /// </summary>
    public class OutputChannelConfigurationCollection : KeyedCollection<OutputChannel, OutputChannelConfiguration>
    {
        /// <summary>
        /// Returns the key for the specified configuration object.
        /// </summary>
        /// <param name="item">The configuration object from which to extract the key.</param>
        /// <returns>The key for the specified configuration object.</returns>
        protected override OutputChannel GetKeyForItem(OutputChannelConfiguration item)
        {
            return item.Channel;
        }
    }
}
