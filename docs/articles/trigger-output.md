
# Triggering playback

Triggering of the PulsePal output channels can be done either through the hardware trigger channels or through software. The `Bonsai.PulsePal` package provides operators to configure and access both methods.

## Hardware Trigger
Configuration of the hardware trigger channels can be set either during the initial connection ['Create Pulsepal'](xref:Bonsai.PulsePal.CreatePulsePal) or modified during workflow execution using the ['ConfigureTriggerChannel'](xref:Bonsai.PulsePal.ConfigureTriggerChannel) operator. The most important parameter to adjust is the `ToggleMode` property which specifies the behavior of the trigger channel.

- In Normal mode (default): an incoming trigger (low to high logic transition) received by a trigger channel will start pulse trains on all linked output channels. Additional trigger pulses received during playback of the pulse train will be ignored.

- In Toggle mode: an incoming trigger received by a trigger channel will start pulse trains on linked output channels. If an additional trigger pulse is detected during playback, the pulse trains on all linked output channels are stopped.

- In Pulse gated mode: a low to high logic transition starts playback and a high to low transition stops playback.

> [!NOTE]
> Remember to link the output channel to the trigger channel by setting the `TriggerOnChannelX` property in either the ['Create Pulsepal'](xref:Bonsai.PulsePal.CreatePulsePal) or ['Configure Output Channel'](xref:Bonsai.PulsePal.ConfigureOutputChannel) operators.

## Software Trigger
Playback of output channels can be triggered by the ['Trigger Output Channels'](xref:Bonsai.PulsePal.TriggerOutputChannels) operator. In the example below, triggering of the output channels is linked to a keypress, but they could also be easily linked to other Bonsai events.

:::workflow
![Trigger Output Channels](../workflows/trigger-output.bonsai)
:::

A constant, fixed voltage can also be broadcast immediately on an output channel using the ['Set Fixed Voltage'](xref:Bonsai.PulsePal.SetFixedVoltage) operator.

:::workflow
![Set Fixed Voltage](../workflows/set-fixed-voltage.bonsai)
:::






