using OpenCV.Net;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents an operator that uploads each custom pulse train in the sequence
    /// to the Pulse Pal device.
    /// </summary>
    [Description("Uploads each custom pulse train in the sequence to the Pulse Pal device.")]
    public class SendCustomPulseTrain : Sink<PulseOnset[]>
    {
        /// <summary>
        /// Gets or sets the name of the Pulse Pal device.
        /// </summary>
        [TypeConverter(typeof(DeviceNameConverter))]
        [Description("The name of the Pulse Pal device.")]
        public string DeviceName { get; set; } = nameof(PulsePal);

        /// <summary>
        /// Gets or sets the identity of the custom pulse train to program.
        /// </summary>
        [Description("The identity of the custom pulse train to program.")]
        public CustomTrainId CustomTrainIdentity { get; set; } = CustomTrainId.CustomTrain1;

        /// <summary>
        /// Uploads each custom pulse train in an observable sequence to the Pulse Pal device,
        /// where each pulse train is specified by an array of pulse onset times and voltages.
        /// </summary>
        /// <param name="source">
        /// A sequence of custom pulse trains, where each pulse train is specified by
        /// an array of pulse onset times and voltages.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of uploading each
        /// custom pulse train in the sequence to the Pulse Pal device.
        /// </returns>
        public override IObservable<PulseOnset[]> Process(IObservable<PulseOnset[]> source)
        {
            return Observable.Using(
                () => PulsePalManager.ReserveConnection(DeviceName),
                pulsePal => source.Do(input =>
                {
                    lock (pulsePal.PulsePal)
                    {
                        pulsePal.PulsePal.SendCustomPulseTrain(CustomTrainIdentity, input);
                    }
                }));
        }

        /// <summary>
        /// Uploads each custom pulse train in an observable sequence to the Pulse Pal device,
        /// where each pulse train is specified by a rectangular array of pulse onset
        /// times and voltages.
        /// </summary>
        /// <param name="source">
        /// A sequence of custom pulse trains, where each pulse train is specified by
        /// a rectangular array of pulse onset times and voltages. The first row of the
        /// array represents the vector of pulse onset times in seconds, and the second row
        /// the corresponding vector of pulse voltages in volts.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of uploading each
        /// custom pulse train in the sequence to the Pulse Pal device.
        /// </returns>
        public IObservable<double[,]> Process(IObservable<double[,]> source)
        {
            return Observable.Using(
                () => PulsePalManager.ReserveConnection(DeviceName),
                pulsePal => source.Do(input =>
                {
                    lock (pulsePal.PulsePal)
                    {
                        pulsePal.PulsePal.SendCustomPulseTrain(CustomTrainIdentity, input);
                    }
                }));
        }

        /// <summary>
        /// Uploads each custom pulse train in an observable sequence to the Pulse Pal device,
        /// where each pulse train is specified by a matrix of pulse onset times and voltages.
        /// </summary>
        /// <param name="source">
        /// A sequence of custom pulse trains, where each pulse train is specified by
        /// a matrix of pulse onset times and voltages. The first row of the matrix represents
        /// the vector of pulse onset times in seconds, and the second row the corresponding
        /// vector of pulse voltages in volts.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of uploading each
        /// custom pulse train in the sequence to the Pulse Pal device.
        /// </returns>
        public IObservable<Mat> Process(IObservable<Mat> source)
        {
            return Observable.Using(
                () => PulsePalManager.ReserveConnection(DeviceName),
                pulsePal => source.Do(input =>
                {
                    var pulseTrain = new double[2, input.Cols];
                    using (var pulseTrainHeader = Mat.CreateMatHeader(pulseTrain))
                    {
                        CV.Convert(input, pulseTrainHeader);
                    }

                    lock (pulsePal.PulsePal)
                    {
                        pulsePal.PulsePal.SendCustomPulseTrain(CustomTrainIdentity, pulseTrain);
                    }
                }));
        }
    }
}
