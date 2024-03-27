# Triggering Playback
Triggering of the Pulse Pal output channels can be done either through the hardware trigger channels or through software. The `Bonsai.PulsePal` package provides operators to configure and use both approaches.

## Link output channels to hardware trigger channels
When configuring output channels in the [`CreatePulsepal`](xref:Bonsai.PulsePal.CreatePulsePal) or [`ConfigureOutputChannel`](xref:Bonsai.PulsePal.ConfigureOutputChannel) operators, these properties must be set to link playback to either of the hardware triggers. 

|     Category     |   Property Name     | Value        | Description     |
| ---------------- | ------------------- | ----------   | --------------- |
| Pulse Trigger    | `TriggerOnChannel1` | True/False   | Set to True to enable trigger on this hardware channel |
| Pulse Trigger    | `TriggerOnChannel2` | True/False   | Set to True to enable trigger on this hardware channel |


## Configure hardware trigger channels
Configuration of the hardware trigger channels can be set either during the initial connection [`CreatePulsepal`](xref:Bonsai.PulsePal.CreatePulsePal) or modified during workflow execution using the [`ConfigureTriggerChannel`](xref:Bonsai.PulsePal.ConfigureTriggerChannel) operator. The most important parameter to adjust is the `ToggleMode` property which specifies the behavior of the trigger channel.

- **Normal** mode (default): an incoming trigger (low to high logic transition) received by a trigger channel will start pulse trains on all linked output channels. Additional trigger pulses received during playback of the pulse train will be ignored.

- **Toggle** mode: an incoming trigger received by a trigger channel will start pulse trains on linked output channels. If an additional trigger pulse is detected during playback, the pulse trains on all linked output channels are stopped.

- **Pulse Gated** mode: a low to high logic transition starts playback and a high to low transition stops playback.


## Software trigger
Playback of output channels can be triggered in software by the [`TriggerOutputChannels`](xref:Bonsai.PulsePal.TriggerOutputChannels) operator. In the example below, triggering of the output channels is linked to a keypress, but they could also be easily linked to other Bonsai events.

:::workflow
![Trigger Output Channels](../workflows/trigger-output.bonsai)
:::

## Set fixed voltage
A constant, fixed voltage can also be set immediately on any output channel by using the [`SetFixedVoltage`](xref:Bonsai.PulsePal.SetFixedVoltage) operator.

:::workflow
![Set Fixed Voltage](../workflows/set-fixed-voltage.bonsai)
:::






