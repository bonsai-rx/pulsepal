using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents an operator that writes a sequence of text strings to the
    /// Pulse Pal oLED display.
    /// </summary>
    [Description("Writes a sequence of text strings to the Pulse Pal oLED display.")]
    public class UpdateDisplay : Sink<string>
    {
        /// <summary>
        /// Gets or sets the name of the Pulse Pal device.
        /// </summary>
        [TypeConverter(typeof(DeviceNameConverter))]
        [Description("The name of the Pulse Pal device.")]
        public string DeviceName { get; set; } = nameof(PulsePal);

        /// <summary>
        /// Writes an observable sequence of text strings to the Pulse Pal oLED display.
        /// </summary>
        /// <param name="source">
        /// A sequence where each value represents a text string to display on the top
        /// row of the oLED display. Text must be less than 17 characters in length.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing each text
        /// string to the Pulse Pal oLED display.
        /// </returns>
        public override IObservable<string> Process(IObservable<string> source)
        {
            return Observable.Using(
                () => PulsePalManager.ReserveConnection(DeviceName),
                connection => source.Do(value =>
                {
                    lock (connection.PulsePal)
                    {
                        connection.PulsePal.UpdateDisplay(value);
                    }
                }));
        }

        /// <summary>
        /// Writes an observable sequence of text strings to the Pulse Pal oLED display.
        /// </summary>
        /// <param name="source">
        /// A sequence where each value represents a pair of text strings to display
        /// respectively on the top and bottom rows of the oLED display. The text in
        /// each row must be less than 17 characters in length.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of writing each pair
        /// of text strings to the top and bottom rows of the Pulse Pal oLED display.
        /// </returns>
        public IObservable<Tuple<string, string>> Process(IObservable<Tuple<string, string>> source)
        {
            return Observable.Using(
                () => PulsePalManager.ReserveConnection(DeviceName),
                connection => source.Do(value =>
                {
                    lock (connection.PulsePal)
                    {
                        connection.PulsePal.UpdateDisplay(value.Item1, value.Item2);
                    }
                }));
        }
    }
}
