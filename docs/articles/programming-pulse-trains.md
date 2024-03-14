# Programming Pulse Trains

Pulse trains can be programmed by configuring the output channel properties in either the [`CreatePulsepal`](xref:Bonsai.PulsePal.CreatePulsePal) or [`ConfigureOutputChannel`](xref:Bonsai.PulsePal.ConfigureOutputChannel) operators. Using the Pulse Pal terminology, pulse trains are built hierarchically from sequences of pulses and bursts. The following sections will detail how to configure the parameters for each in Bonsai. Additional information on these parameters can be found on the [Pulse Pal Parameter Guide](https://sites.google.com/site/pulsepalwiki/parameter-guide/).

## Pulse parameters
Pulses can either be monophasic pulses or biphasic. The time course of a pulse is governed by several parameters as shown in the image below. 

!['Pulse Pal Wiki-Pulse Parameters'](~/images/PulsePalWiki-PulseParams.png)

(Image reproduced from the [Pulse Pal Wiki](https://sites.google.com/site/pulsepalwiki/))

For monophasic pulses, set the properties that are outlined in the image below. When defining monophasic pulses, the `Phase2Duration` and `Phase2Voltage` and `InterPhaseInterval` properties are ignored. The other properties will be discussed in the following sections.

!['Bonsai - Monophasic Pulse'](~/images/monophasic-pulse-outlined.png)

For biphasic pulses, set the properties that are outlined in the image below.

!['Bonsai - Biphasic Pulse'](~/images/biphasic-pulse-outlined.png)

## Burst parameters
Pulses can be grouped together into bursts, which have the following parameters.

!['Pulse Pal Wiki-Burst Parameters'](~/images/PulsePalWiki-BurstParams.png)

(Image reproduced from the [Pulse Pal Wiki](https://sites.google.com/site/pulsepalwiki/))

To reproduce the biphasic pulse burst train in the image above in Bonsai, in addition to setting the pulse properties, change the burst properties as outlined in the image below.

!['Bonsai - Burst'](~/images/burst-outlined.png)

> [!TIP]
> To disable burst mode and enable a continuous sequence of pulses, set the `InterPulseInterval` property to your desired value but set the `BurstDuration` property to 0.

## Train parameters
A pulse train of pulses and bursts can be additionally configured with the parameters below.

!['Pulse Pal Wiki-Train Parameters'](~/images/PulsePalWiki-TrainParams.png)

(Image reproduced from the [Pulse Pal Wiki](https://sites.google.com/site/pulsepalwiki/))

To reproduce the pulse train above, change the train properties as outlined in the image below.

!['Bonsai - Train'](~/images/train-outlined.png)

## Pulse train playback settings
Besides setting the output channel to playback the pulse train from with the `Channel` property in either the the [`CreatePulsepal`](xref:Bonsai.PulsePal.CreatePulsePal) or [`ConfigureOutputChannel`](xref:Bonsai.PulsePal.ConfigureOutputChannel) operators, the `ContinuousLoop` property can be used to control if the pulse train on the output channel is played back continuously when triggered or only played once.

