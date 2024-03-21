using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents an operator that creates a sequence of custom pulse train
    /// onset times and voltages.
    /// </summary>
    [Description("Creates a sequence of custom pulse train onset times and voltages.")]
    public class PulseOnset : Combinator<PulseOnset>
    {
        const double MinVoltage = PulsePalDevice.MinVoltage;
        const double MaxVoltage = PulsePalDevice.MaxVoltage;
        const double MinTimePeriod = PulsePalDevice.MinTimePeriod;
        const double MaxTimePeriod = PulsePalDevice.MaxTimePeriod;
        const int VoltageDecimalPlaces = OutputChannelParameterConfiguration.VoltageDecimalPlaces;
        const double VoltageIncrement = OutputChannelParameterConfiguration.VoltageIncrement;
        const int TimeDecimalPlaces = OutputChannelParameterConfiguration.TimeDecimalPlaces;

        /// <summary>
        /// Gets or sets the pulse onset time, in seconds relative to the
        /// start of the pulse.
        /// </summary>
        [XmlAttribute]
        [Range(0, MaxTimePeriod)]
        [Precision(TimeDecimalPlaces, MinTimePeriod)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The pulse onset time, in seconds relative to the start of the pulse.")]
        public double Time { get; set; }

        /// <summary>
        /// Gets or sets the pulse onset voltage, in volts.
        /// </summary>
        [XmlAttribute]
        [Range(MinVoltage, MaxVoltage)]
        [Precision(VoltageDecimalPlaces, VoltageIncrement)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The pulse onset voltage, in volts.")]
        public double Voltage { get; set; }

        /// <summary>
        /// Creates an observable sequence containing a single pulse train onset
        /// configured with the specified time and voltage.
        /// </summary>
        /// <returns>
        /// A sequence containing the created <see cref="PulseOnset"/> object.
        /// </returns>
        public IObservable<PulseOnset> Process()
        {
            return Process(Observable.Return(Unit.Default));
        }

        /// <summary>
        /// Creates an observable sequence of pulse train onset times and voltages,
        /// where each pulse onset is emitted only when an observable sequence
        /// emits a notification.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the <paramref name="source"/> sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence containing the notifications used for emitting new
        /// pulse onset times and voltages.
        /// </param>
        /// <returns>
        /// A sequence containing the created <see cref="PulseOnset"/> objects
        /// whenever the <paramref name="source"/> emits notifications.
        /// </returns>
        public override IObservable<PulseOnset> Process<TSource>(IObservable<TSource> source)
        {
            return source.Select(_ => new PulseOnset
            {
                Time = Time,
                Voltage = Voltage
            });
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Pulse({Time}s, {Voltage}V)";
        }
    }
}
