
# Pulse Trains

The `Bonsai.PulsePal` package provides access to two different methods for programming pulse trains for playback on the PulsePal output channels

## Method 1 - Configuring the output channels
The first method for programming pulse trains is by configuring the output channels in either the ['Create Pulsepal'](xref:Bonsai.PulsePal.CreatePulsePal) or ['Configure Output Channel'](xref:Bonsai.PulsePal.ConfigureOutputChannel) operators. Using the PulsePal terminology, pulse trains are built hierarchically from sequences of pulses and bursts. The following sections will detail how to configure the parameters for each in Bonsai. Additional information on these parameters can be found on the [PulsePal Parameter Guide](https://sites.google.com/site/pulsepalwiki/parameter-guide/)

### Pulse parameters
Pulses can either be monophasic pulses or biphasic. The time course of a pulse is governed by several parameters as shown in the image below. 

!['PulsePal Wiki-Pulse Parameters'](~/images/PulsePalWiki-PulseParams.png)
(Image reproduced from the [PulsePal Wiki](https://sites.google.com/site/pulsepalwiki/))

For monophasic pulses, set the properties that are outlined in the image below. When defining monophasic pulses, the `Phase2Duration` and `Phase2Voltage` and `InterPhaseInterval` properties are ignored. The other properties will be discussed in the following sections.

!['Bonsai - Monophasic Pulse'](~/images/monophasic-pulse-outlined.png)

For biphasic pulses, set the properties that are outlined in the image below.

!['Bonsai - Biphasic Pulse'](~/images/biphasic-pulse-outlined.png)

### Burst parameters
Pulses can be grouped together into bursts, which have the following parameters.

!['PulsePal Wiki-Burst Parameters'](~/images/PulsePalWiki-BurstParams.png)
(Image reproduced from the [PulsePal Wiki](https://sites.google.com/site/pulsepalwiki/))

To reproduce the biphasic pulse burst train in the image above in Bonsai, in addition to setting the pulse properties, change the burst properties as outlined in the image below.

!['Bonsai - Burst'](~/images/burst-outlined.png)

> [!TIP]
> To disable burst mode and enable a continuous sequence of pulses, set the `InterPulseInterval` property to your desired value but set the `BurstDuration` property to 0.

### Train parameters
A pulse train of pulses and bursts can be additionally configured with the parameters below.

!['PulsePal Wiki-Train Parameters'](~/images/PulsePalWiki-TrainParams.png)
(Image reproduced from the [PulsePal Wiki](https://sites.google.com/site/pulsepalwiki/))

To reproduce the pulse train above, change the train properties as outlined in the image below.

!['Bonsai - Train'](~/images/train-outlined.png)

## Method 2 - Creating custom pulse trains
In order to create more complex pulse trains, the Bonsai 
















### Train parameters

### Linking pulse train playback to trigger channels








## Creating and Uploading Custom Pulse Trains to PulsePal
Custom pulse trains can be created and uploaded to the PulsePal device using the ['Send Custom Pulse Train'](xref:Bonsai.PulsePal.SendCustomPulseTrain) or ['Send Custom Waveform'](xref:Bonsai.PulsePal.SendCustomWaveform) operators.





> [!CAUTION]
> The  ['Send Custom Waveform'](xref:Bonsai.PulsePal.SendCustomWaveform) operator does not "exactly" reproduce the input waveform as a custom train.
> From the [Pulse Pal](https://sites.google.com/site/pulsepalwiki/) wiki - "While onset times and voltages are specified in software, the Phase1Duration parameter still controls pulse width for all pulses in a custom train."
> It is advised that custom trains be verified with an oscilloscope.


