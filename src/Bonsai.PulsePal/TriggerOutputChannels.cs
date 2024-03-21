using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents an operator that triggers the specified output channels to
    /// begin playing their pulse sequences.
    /// </summary>
    [Description("Triggers the specified output channels to begin playing their pulse sequences.")]
    public class TriggerOutputChannels : Sink
    {
        /// <summary>
        /// Gets or sets the name of the Pulse Pal device.
        /// </summary>
        [TypeConverter(typeof(DeviceNameConverter))]
        [Description("The name of the Pulse Pal device.")]
        public string DeviceName { get; set; } = nameof(PulsePal);

        /// <summary>
        /// Gets or sets a value specifying which output channel triggers to set.
        /// </summary>
        [Description("Specifies which output channel triggers to set.")]
        public ChannelTriggers Channels { get; set; }

        /// <summary>
        /// Triggers the specified output channels on the Pulse Pal device whenever
        /// an observable sequence emits a notification.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the <paramref name="source"/> sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence containing the notifications used to trigger the output
        /// channels on the Pulse Pal device.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of triggering the
        /// specified output channels on the Pulse Pal device whenever the sequence
        /// emits a notification.
        /// </returns>
        public override IObservable<TSource> Process<TSource>(IObservable<TSource> source)
        {
            return Observable.Using(
                () => PulsePalManager.ReserveConnection(DeviceName),
                connection => source.Do(input =>
                {
                    lock (connection.PulsePal)
                    {
                        connection.PulsePal.TriggerOutputChannels(Channels);
                    }
                }));
        }
    }
}
