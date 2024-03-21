using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents configuration parameters specifying the voltage for the first
    /// phase of each pulse.
    /// </summary>
    [DisplayName(nameof(ParameterCode.Phase1Voltage))]
    [Description("Specifies the voltage for the first phase of each pulse.")]
    public class Phase1VoltageConfiguration : OutputChannelParameterConfiguration
    {
        /// <summary>
        /// Gets or sets the voltage for the first phase of each pulse.
        /// </summary>
        [Range(MinVoltage, MaxVoltage)]
        [Precision(VoltageDecimalPlaces, VoltageIncrement)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The voltage for the first phase of each pulse.")]
        public double Phase1Voltage { get; set; }

        /// <inheritdoc/>
        public override void Configure(PulsePalDevice device)
        {
            device.SetPhase1Voltage(Channel, Phase1Voltage);
        }
    }
}
