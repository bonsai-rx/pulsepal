using System.Collections.ObjectModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents a collection of trigger channel configuration objects.
    /// </summary>
    public class TriggerChannelConfigurationCollection : KeyedCollection<TriggerChannel, TriggerChannelConfiguration>
    {
        /// <summary>
        /// Returns the key for the specified configuration object.
        /// </summary>
        /// <param name="item">The configuration object from which to extract the key.</param>
        /// <returns>The key for the specified configuration object.</returns>
        protected override TriggerChannel GetKeyForItem(TriggerChannelConfiguration item)
        {
            return item.Channel;
        }
    }
}
