namespace Bonsai.PulsePal
{
    internal class PulsePalConfiguration
    {
        internal static readonly PulsePalConfiguration Default = new();

        public string PortName { get; set; }

        public ConfigureOutputChannelCollection OutputChannels { get; } = new();

        public ConfigureTriggerChannelCollection TriggerChannels { get; } = new();

        public void Configure(PulsePalDevice pulsePal)
        {
            foreach (var outputChannel in OutputChannels)
            {
                outputChannel.Configure(pulsePal);
            }

            foreach (var triggerChannel in TriggerChannels)
            {
                triggerChannel.Configure(pulsePal);
            }
        }
    }
}
