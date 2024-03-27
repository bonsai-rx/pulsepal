# Getting Started

`Bonsai.PulsePal` is a [Bonsai](https://bonsai-rx.org/) interface for the [Pulse Pal](https://sites.google.com/site/pulsepalwiki/) open source pulse train generator. All device initialization, configuration, and triggering functionality is exposed via reactive operators.

To install `Bonsai.PulsePal` use the Bonsai package manager and search for the **Bonsai - PulsePal** package.

## Initialize the Pulse Pal
The [`CreatePulsePal`](xref:Bonsai.PulsePal.CreatePulsePal) operator establishes the serial connection link with the device and should be the first node you add to your workflow. The `PortName` property must be set to the name of the serial port used by the operating system to communicate with the device (e.g. `COM3`).

:::workflow
![CreatePulsePal](~/workflows/create-pulsepal.bonsai)
:::

The `OutputChannels` and `TriggerChannels` properties allow you to set the initial configuration for each output channel pulse train and the behavior of the hardware trigger channels. A more detailed discussion of channel configuration properties can be found in the [Programming Pulse Trains](~/articles/programming-pulse-trains.md) and [Triggering Playback](~/articles/trigger-output.md) guides.

> [!TIP]
> If you want to use more than one Pulse Pal, you can add multiple `CreatePulsePal` sources and assign each device a unique name by setting the `DeviceName` property (which has a default value of `PulsePal`). In downstream operators, you can specify which PulsePal configuration to modify by changing their respective `DeviceName` properties.

## Modify Pulse Pal configuration
In addition to the initial configuration that can be set when creating the Pulse Pal connection, the `Bonsai.PulsePal` package provides a set of operators that can be used to modify the configuration of the Pulse Pal while the workflow is running. These operators provide functionality which is identical to the properties in [`CreatePulsePal`](xref:Bonsai.PulsePal.CreatePulsePal). Below we show an example using separate key presses to drive these operators, but they could be easily triggered by any other Bonsai event. 

:::workflow
![ConfigurePulsePal](~/workflows/configure-pulsepal.bonsai)
:::

> [!WARNING]
> If the [`CreatePulsePal`](xref:Bonsai.PulsePal.CreatePulsePal) operator is set as an input to these operators, the `DeviceName` property is ignored.


