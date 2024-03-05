
# Pulse Trains

The `Bonsai.PulsePal` package provides two ways to create pulse trains.





## Creating and Uploading Custom Pulse Trains to PulsePal
Custom pulse trains can be created and uploaded to the PulsePal device using the ['Send Custom Pulse Train'](xref:Bonsai.PulsePal.SendCustomPulseTrain) or ['Send Custom Waveform'](xref:Bonsai.PulsePal.SendCustomWaveform) operators.





> [!CAUTION]
> The  ['Send Custom Waveform'](xref:Bonsai.PulsePal.SendCustomWaveform) operator does not "exactly" reproduce the input waveform as a custom train.
> From the [Pulse Pal](https://sites.google.com/site/pulsepalwiki/) wiki - "While onset times and voltages are specified in software, the Phase1Duration parameter still controls pulse width for all pulses in a custom train."
> It is advised that custom trains be verified with an oscilloscope.


