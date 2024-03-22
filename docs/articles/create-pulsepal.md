# Initializing the Pulse Pal
The [`CreatePulsePal`](xref:Bonsai.PulsePal.CreatePulsePal) operator establishes the serial connection link with the device and should be the first node you add to your workflow.

:::workflow
![CreatePulsePal](../workflows/create-pulsepal.bonsai)
:::

> [!NOTE]
> If there is more than one Pulse Pal connected, they can be assigned different names using the `DeviceName` property (which has a default value of `PulsePal`). In downstream operators, you can specify which PulsePal configuration to modify by changing the `DeviceName` property.

## Configuring pulse trains and hardware triggers
 
The `OutputChannels` and `TriggerChannels` properties allow you to set the initial configuration for pulse trains and triggers. A more detailed discussion of channel configuration properties can be found in the [Programming Pulse Trains](~/articles/programming-pulse-trains.md) and [Triggering Playback](~/articles/trigger-output.md) guides.

## Modifying Pulse Pal configuration
In addition to the initial configuration that can be set when creating the Pulse Pal connection, the `Bonsai.PulsePal` package provides a set of operators that can be used to modify the configuration of the Pulse Pal in the middle of a workflow execution. These operators are identical to the properties that are in the [`CreatePulsePal`](xref:Bonsai.PulsePal.CreatePulsePal) operator. In the example below, these operators are triggered by separate key presses, but they could be easily triggered by some other Bonsai event. 

:::workflow
![ConfigurePulsePal](../workflows/configure-pulsepal.bonsai)
:::

> [!CAUTION]
> If the [`CreatePulsePal`](xref:Bonsai.PulsePal.CreatePulsePal) operator is set as an input to these operators, the `DeviceName` property is ignored.


