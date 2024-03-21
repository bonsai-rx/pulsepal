using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents an operator that sets a constant voltage on an output channel.
    /// </summary>
    [Description("Sets a constant voltage on an output channel.")]
    public class SetFixedVoltage : Sink
    {
        const double MinVoltage = PulsePalDevice.MinVoltage;
        const double MaxVoltage = PulsePalDevice.MaxVoltage;
        const int VoltageDecimalPlaces = OutputChannelParameterConfiguration.VoltageDecimalPlaces;
        const double VoltageIncrement = OutputChannelParameterConfiguration.VoltageIncrement;

        /// <summary>
        /// Gets or sets the name of the Pulse Pal device.
        /// </summary>
        [TypeConverter(typeof(DeviceNameConverter))]
        [Description("The name of the Pulse Pal device.")]
        public string DeviceName { get; set; } = nameof(PulsePal);

        /// <summary>
        /// Gets or sets a value specifying the output channel to set to a fixed voltage.
        /// </summary>
        [Description("Specifies the output channel to set to a fixed voltage.")]
        public OutputChannel Channel { get; set; } = OutputChannel.Channel1;

        /// <summary>
        /// Gets or sets the constant voltage for the output channel.
        /// </summary>
        [Range(MinVoltage, MaxVoltage)]
        [Precision(VoltageDecimalPlaces, VoltageIncrement)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The constant voltage for the output channel.")]
        public double Voltage { get; set; }

        /// <summary>
        /// Sets a constant voltage on an output channel in the Pulse Pal device whenever
        /// an observable sequence emits a notification.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the <paramref name="source"/> sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence containing the notifications used to set the fixed voltage
        /// on the specified Pulse Pal channel.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of setting a single
        /// channel on the Pulse Pal device to a constant voltage whenever the sequence
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
                        connection.PulsePal.SetFixedVoltage(Channel, Voltage);
                    }
                }));
        }
    }
}
