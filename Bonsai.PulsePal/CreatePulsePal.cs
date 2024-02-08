﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.IO.Ports;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents an operator that creates and configures a serial connection
    /// to a Pulse Pal device.
    /// </summary>
    [Description("Creates and configures a serial connection to a Pulse Pal device.")]
    public class CreatePulsePal : Source<PulsePal>, INamedElement
    {
        readonly PulsePalConfiguration configuration = new();

        /// <summary>
        /// Gets or sets the optional alias for the Pulse Pal device.
        /// </summary>
        [Description("The optional alias for the Pulse Pal device.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the serial port used to communicate
        /// with the Pulse Pal.
        /// </summary>
        [TypeConverter(typeof(SerialPortNameConverter))]
        [Description("The name of the serial port used to communicate with the Pulse Pal.")]
        public string PortName
        {
            get => configuration.PortName;
            set => configuration.PortName = value;
        }

        /// <summary>
        /// Gets the collection of output channels to configure on the Pulse Pal device.
        /// </summary>
        [Description("The collection of output channels to configure on the Pulse Pal device.")]
        public ConfigureOutputChannelCollection OutputChannels => configuration.OutputChannels;

        /// <summary>
        /// Gets the collection of trigger channels to configure on the Pulse Pal device.
        /// </summary>
        [Description("The collection of trigger channels to configure on the Pulse Pal device.")]
        public ConfigureTriggerChannelCollection TriggerChannels => configuration.TriggerChannels;

        /// <summary>
        /// Generates an observable sequence that contains the serial interface object.
        /// </summary>
        /// <returns>
        /// A sequence containing a single instance of the <see cref="PulsePal"/> class
        /// representing the serial interface to Pulse Pal.
        /// </returns>
        public override IObservable<PulsePal> Generate()
        {
            return Observable.Using(
                () => PulsePalManager.ReserveConnection(Name, configuration),
                resource => Observable.Return(resource.PulsePal)
                                      .Concat(Observable.Never(resource.PulsePal)));
        }
    }
}
