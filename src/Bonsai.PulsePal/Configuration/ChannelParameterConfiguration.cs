namespace Bonsai.PulsePal
{
    /// <summary>
    /// Provides an abstract base class for configuring Pulse Pal
    /// channel parameters.
    /// </summary>
    public abstract class ChannelParameterConfiguration
    {
        internal const string ChannelCategory = "Channel";

        /// <summary>
        /// Applies the channel parameter configuration to the specified
        /// Pulse Pal device.
        /// </summary>
        /// <param name="device">The Pulse Pal device to configure.</param>
        public abstract void Configure(PulsePalDevice device);
    }
}
