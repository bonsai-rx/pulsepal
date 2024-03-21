using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Xml.Serialization;
using Bonsai.Expressions;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents an operator that configures a single channel parameter on a
    /// Pulse Pal device.
    /// </summary>
    [DefaultProperty(nameof(Parameter))]
    [XmlInclude(typeof(BiphasicConfiguration))]
    [XmlInclude(typeof(Phase1VoltageConfiguration))]
    [XmlInclude(typeof(Phase2VoltageConfiguration))]
    [XmlInclude(typeof(Phase1DurationConfiguration))]
    [XmlInclude(typeof(InterPhaseIntervalConfiguration))]
    [XmlInclude(typeof(Phase2DurationConfiguration))]
    [XmlInclude(typeof(InterPulseIntervalConfiguration))]
    [XmlInclude(typeof(BurstDurationConfiguration))]
    [XmlInclude(typeof(InterBurstIntervalConfiguration))]
    [XmlInclude(typeof(PulseTrainDurationConfiguration))]
    [XmlInclude(typeof(PulseTrainDelayConfiguration))]
    [XmlInclude(typeof(TriggerOnChannel1Configuration))]
    [XmlInclude(typeof(TriggerOnChannel2Configuration))]
    [XmlInclude(typeof(CustomTrainIdentityConfiguration))]
    [XmlInclude(typeof(CustomTrainTargetConfiguration))]
    [XmlInclude(typeof(CustomTrainLoopConfiguration))]
    [XmlInclude(typeof(RestingVoltageConfiguration))]
    [XmlInclude(typeof(TriggerModeConfiguration))]
    [XmlInclude(typeof(ContinuousLoopConfiguration))]
    [WorkflowElementCategory(ElementCategory.Sink)]
    [Description("Configures a single channel parameter on a Pulse Pal device.")]
    public class ConfigureChannelParameter : PolymorphicCombinator, INamedElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigureChannelParameter"/> class.
        /// </summary>
        public ConfigureChannelParameter()
        {
            Parameter = new BiphasicConfiguration();
        }

        string INamedElement.Name => $"Set{ExpressionBuilder.GetElementDisplayName(Parameter)}";

        /// <summary>
        /// Gets or sets the name of the Pulse Pal device.
        /// </summary>
        [TypeConverter(typeof(DeviceNameConverter))]
        [Description("The name of the Pulse Pal device.")]
        public string DeviceName { get; set; } = nameof(PulsePal);

        /// <summary>
        /// Gets or sets the channel parameter to configure.
        /// </summary>
        [DesignOnly(true)]
        [Externalizable(false)]
        [RefreshProperties(RefreshProperties.All)]
        [Category(nameof(CategoryAttribute.Design))]
        [Description("The channel parameter to configure.")]
        [TypeConverter(typeof(CombinatorTypeConverter))]
        public ChannelParameterConfiguration Parameter
        {
            get { return (ChannelParameterConfiguration)Operator; }
            set { Operator = value ?? throw new ArgumentNullException(nameof(value)); }
        }

        /// <summary>
        /// Configures a single channel parameter on the Pulse Pal device whenever
        /// an observable sequence emits a notification.
        /// </summary>
        /// <typeparam name="TSource">
        /// The type of the elements in the <paramref name="source"/> sequence.
        /// </typeparam>
        /// <param name="source">
        /// The sequence containing the notifications used to trigger configuration
        /// of the Pulse Pal channel parameter.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of configuring a
        /// single channel parameter on the Pulse Pal device whenever the sequence
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
                        Parameter.Configure(connection.PulsePal);
                    }
                }));
        }

        /// <summary>
        /// Configures a single channel parameter on every Pulse Pal device
        /// in the observable sequence.
        /// </summary>
        /// <param name="source">
        /// The sequence of Pulse Pal devices to configure.
        /// </param>
        /// <returns>
        /// An observable sequence that is identical to the <paramref name="source"/>
        /// sequence but where there is an additional side effect of configuring a
        /// single channel parameter on each Pulse Pal device.
        /// </returns>
        public IObservable<PulsePalDevice> Process(IObservable<PulsePalDevice> source)
        {
            return source.Do(Parameter.Configure);
        }
    }
}
