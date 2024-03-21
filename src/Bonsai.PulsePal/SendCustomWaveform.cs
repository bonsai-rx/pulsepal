using OpenCV.Net;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents an operator that uploads each array of voltages in a sequence to the
    /// Pulse Pal device, where each array specifies a custom train of continuous
    /// monophasic pulses, with periodic onset times.
    /// </summary>
    [Description("Uploads each array of voltages in a sequence to the Pulse Pal device, with periodic onset times.")]
    public class SendCustomWaveform : Sink<Mat>
    {
        const double MinTimePeriod = PulsePalDevice.MinTimePeriod;
        const double MaxTimePeriod = PulsePalDevice.MaxTimePeriod;
        const int TimeDecimalPlaces = OutputChannelParameterConfiguration.TimeDecimalPlaces;

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
        /// Gets or sets the sample rate, in Hz, used to playback the custom pulse train.
        /// </summary>
        [Range(MinTimePeriod, MaxTimePeriod)]
        [Precision(TimeDecimalPlaces, MinTimePeriod)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The sample rate, in Hz, used to playback the custom pulse train.")]
        public double SamplingPeriod { get; set; } = MinTimePeriod;

        /// <summary>
        /// Uploads each array of voltages in an observable sequence to the Pulse Pal
        /// device, where each array specifies a custom train of continuous monophasic
        /// pulses, with periodic onset times.
        /// </summary>
        /// <param name="source">
        /// A sequence of custom pulse trains, where each pulse train is specified by
        /// an array of pulse voltages. The vector of pulse onset times is automatically
        /// computed from the specified sample rate.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of uploading each
        /// custom pulse train in the sequence to the Pulse Pal device.
        /// </returns>
        public IObservable<double[]> Process(IObservable<double[]> source)
        {
            return Observable.Using(
                () => PulsePalManager.ReserveConnection(DeviceName),
                pulsePal => source.Do(pulseVoltages =>
                {
                    lock (pulsePal.PulsePal)
                    {
                        pulsePal.PulsePal.SendCustomWaveform(CustomTrainIdentity, SamplingPeriod, pulseVoltages);
                    }
                }));
        }

        /// <summary>
        /// Uploads each vector of voltages in an observable sequence to the Pulse Pal
        /// device, where each vector specifies a custom train of continuous monophasic
        /// pulses, with periodic onset times.
        /// </summary>
        /// <param name="source">
        /// A sequence of custom pulse trains, where each pulse train is specified by
        /// a row or column vector of pulse voltages. The vector of pulse onset times is
        /// automatically computed from the specified sample rate.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of uploading each
        /// custom pulse train in the sequence to the Pulse Pal device.
        /// </returns>
        public override IObservable<Mat> Process(IObservable<Mat> source)
        {
            return Observable.Using(
                () => PulsePalManager.ReserveConnection(DeviceName),
                pulsePal => source.Do(input =>
                {
                    var pulseVoltages = new double[input.Cols];
                    using (var voltageHeader = Mat.CreateMatHeader(pulseVoltages))
                    {
                        CV.Convert(input, voltageHeader);
                    }

                    lock (pulsePal.PulsePal)
                    {
                        pulsePal.PulsePal.SendCustomWaveform(CustomTrainIdentity, SamplingPeriod, pulseVoltages);
                    }
                }));
        }
    }
}
