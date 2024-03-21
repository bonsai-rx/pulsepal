using System.ComponentModel;

namespace Bonsai.PulsePal
{
    /// <summary>
    /// Represents configuration parameters specifying the voltage for the second
    /// phase of each pulse.
    /// </summary>
    [DisplayName(nameof(ParameterCode.Phase2Voltage))]
    [Description("Specifies the voltage for the second phase of each pulse.")]
    public class Phase2VoltageConfiguration : OutputChannelParameterConfiguration
    {
        /// <summary>
        /// Gets or sets the voltage for the second phase of each pulse.
        /// </summary>
        [Range(MinVoltage, MaxVoltage)]
        [Precision(VoltageDecimalPlaces, VoltageIncrement)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The voltage for the second phase of each pulse.")]
        public double Phase2Voltage { get; set; }

        /// <inheritdoc/>
        public override void Configure(PulsePalDevice device)
        {
            device.SetPhase2Voltage(Channel, Phase2Voltage);
        }
    }
}
