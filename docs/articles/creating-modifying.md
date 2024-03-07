# Creating And Modifying Pulse Pal Connection
The `Bonsai.PulsePal` package provides operators to initialize a Pulse Pal connection, set initial parameters and modify them in the middle of a workflow.

## Creating Pulse Pal connection 

The ['CreatePulsePal'](xref:Bonsai.PulsePal.CreatePulsePal) operator is the first node you will add to your workflow when using the Pulse Pal. This source is responsible for establishing a serial connection link with the device and exposes a initial set of properties that can be used to program pulse trains on the output channels and configure hardware trigger channels. Additional discussion of those properties can be found in the [Programming Pulse Trains](~/articles/programming-pulse-trains.md) and [Triggering Playback](~/articles/trigger-output.md) articles.

:::workflow
![CreatePulsePal](../workflows/create-pulsepal.bonsai)
:::

## Modifying Pulse Pal configuration
In addition to the initial configuration that can be set when creating the Pulse Pal connection, the `Bonsai.PulsePal` package provides a set of operators that can be used to modify the configuration of the Pulse Pal in the middle of a workflow execution. These properties are identical to the properties that are in the ['CreatePulsePal'](xref:Bonsai.PulsePal.CreatePulsePal) operator. In the example below, these operators are triggered by separate key presses, but they could be easily triggered by some other Bonsai event. 

:::workflow
![ConfigurePulsePal](../workflows/configure-pulsepal.bonsai)
:::



