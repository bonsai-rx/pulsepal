namespace Bonsai.PulsePal
{
    internal class PulsePalConfiguration
    {
        internal static readonly PulsePalConfiguration Default = new();

        public string PortName { get; set; }

        public OutputChannelConfigurationCollection OutputChannels { get; } = new();

        public TriggerChannelConfigurationCollection TriggerChannels { get; } = new();

        public void Configure(PulsePalDevice pulsePal)
        {
            foreach (var channelConfiguration in OutputChannels)
            {
                channelConfiguration.Configure(pulsePal);
            }

            foreach (var channelConfiguration in TriggerChannels)
            {
                channelConfiguration.Configure(pulsePal);
            }
        }
    }
}
