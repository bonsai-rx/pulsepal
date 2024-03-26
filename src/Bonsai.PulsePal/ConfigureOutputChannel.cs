using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents an operator that configures output channel parameters on a
    /// Pulse Pal device.
    /// </summary>
    [Combinator]
    [WorkflowElementCategory(ElementCategory.Sink)]
    [Description("Configures output channel parameters on a Pulse Pal device.")]
    public class ConfigureOutputChannel : OutputChannelConfiguration, INamedElement
    {
        string INamedElement.Name => Channel == 0
            ? nameof(ConfigureOutputChannel)
            : $"ConfigureOutput{Channel}";

        /// <summary>
        /// Gets or sets the name of the Pulse Pal device.
        /// </summary>
        [Category(ChannelCategory)]
        [TypeConverter(typeof(DeviceNameConverter))]
        [Description("The name of the Pulse Pal device.")]
        public string DeviceName { get; set; } = nameof(PulsePal);

        /// <summary>
        /// Configures the output channel parameters on the Pulse Pal device whenever
        /// an observable sequence emits a notification.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the <paramref name="source"/> sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence containing the notifications used to trigger configuration
        /// of the Pulse Pal output channel.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of configuring the
        /// output channel parameters on the Pulse Pal device whenever the sequence
        /// emits a notification.
        /// </returns>
        public IObservable<TSource> Process<TSource>(IObservable<TSource> source)
        {
            return Observable.Using(
                () => PulsePalManager.ReserveConnection(DeviceName),
                connection => source.Do(input =>
                {
                    lock (connection.PulsePal)
                    {
                        Configure(connection.PulsePal);
                    }
                }));
        }

        /// <summary>
        /// Configures the output channel parameters on every Pulse Pal device
        /// in the observable sequence.
        /// </summary>
        /// <param name="source">
        /// The sequence of Pulse Pal devices to configure.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of configuring the
        /// output channel parameters on each Pulse Pal device.
        /// </returns>
        public IObservable<PulsePalDevice> Process(IObservable<PulsePalDevice> source)
        {
            return source.Do(Configure);
        }
    }
}
