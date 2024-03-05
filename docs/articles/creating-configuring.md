
## Creating Pulsepal Connection 

The ['Create Pulsepal'](xref:Bonsai.PulsePal.CreatePulsePal) operator is the first node you will add to your workflow when using the Pulsepal. This source is responsible for establishing a serial connection link with the device and exposes a initial set of properties that can be used to configure the Pulsepal. Additional discussion of the configuration properties will follow in the next section.

:::workflow
![CreatePulsePal](../workflows/create-pulsepal.bonsai)
:::

## Configuring Pulsepal Connection
In addition to the initial configuration that can be set when creating the PulsePal connection, the `Bonsai.PulsePal` package provides a set of operators (['ConfigureTriggerChannel](#configuretriggerchannel))  that can be used to change the configuration of the PulsePal in the middle of a workflow execution.

:::workflow
![ConfigurePulsePal](../workflows/configure-pulsepal.bonsai)
:::

### Configure Trigger Channel
The ['ConfigureTriggerChannel'](xref:Bonsai.PulsePal.ConfigureTriggerChannel) allows you to configure the mode of the specified trigger channel.

In normal mode, an incoming trigger (low to high logic transition) received by a trigger channel will start pulse trains on all linked output channels. 

-Additional trigger pulses received during playback of the pulse train will be ignored.

In toggle mode, an incoming trigger received by a trigger channel will start pulse trains on linked output channels.

-If an additional trigger pulse is detected during playback, the pulse trains on all linked output channels are stopped.

In pulse gated mode, a low to high logic transition starts playback and a high to low transition stops playback.

