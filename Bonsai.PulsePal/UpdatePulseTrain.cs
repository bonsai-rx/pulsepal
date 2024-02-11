using OpenCV.Net;
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reactive.Linq;

namespace Bonsai.PulsePal
{
    [Description("Uploads a custom pulse train to the Pulse Pal.")]
    public class UpdatePulseTrain : Sink<Mat>
    {
        public UpdatePulseTrain()
        {
            PulseId = 1;
        }

        [Description("The name of the serial port.")]
        [TypeConverter(typeof(PortNameConverter))]
        public string PortName { get; set; }

        [Description("The identifier of the custom pulse train.")]
        public int PulseId { get; set; }

        [Description("The playback frequency (Hz) used for the custom pulse train.")]
        public int Frequency { get; set; }

        public override IObservable<Mat> Process(IObservable<Mat> source)
        {
            return Observable.Using(
                () => PulsePalManager.ReserveConnection(PortName),
                pulsePal => source.Do(input =>
                {
                    var pulseInterval = 1.0 / Frequency;
                    var pulseTimes = new double[input.Cols];
                    var pulseVoltages = new double[input.Cols];
                    for (int i = 0; i < pulseTimes.Length; i++)
                    {
                        pulseTimes[i] = pulseInterval * i;
                    }

                    using (var voltageHeader = Mat.CreateMatHeader(pulseVoltages))
                    {
                        CV.Convert(input, voltageHeader);
                    }

                    lock (pulsePal.PulsePal)
                    {
                        pulsePal.PulsePal.SendCustomPulseTrain((CustomTrainId)PulseId, pulseTimes, pulseVoltages);
                    }
                }));
        }
    }
}
