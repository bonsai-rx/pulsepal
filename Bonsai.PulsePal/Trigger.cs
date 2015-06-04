﻿using Bonsai.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.PulsePal
{
    public class Trigger : Sink
    {
        [Description("The name of the serial port.")]
        [TypeConverter(typeof(SerialPortNameConverter))]
        public string PortName { get; set; }

        public byte Channels { get; set; }

        public override IObservable<TSource> Process<TSource>(IObservable<TSource> source)
        {
            return Observable.Using(
                () => PulsePalManager.ReserveConnection(PortName),
                pulsePal => source.Do(input =>
                {
                    lock (pulsePal.PulsePal)
                    {
                        pulsePal.PulsePal.TriggerOutputChannels(Channels);
                    }
                }));
        }
    }
}