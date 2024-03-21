using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using Bonsai.Expressions;

namespace Bonsai.PulsePal
{
    class DeviceNameConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (context != null)
            {
                var workflowBuilder = (WorkflowBuilder)context.GetService(typeof(WorkflowBuilder));
                if (workflowBuilder != null)
                {
                    var portNames = (from builder in workflowBuilder.Workflow.Descendants()
                                     where builder is not DisableBuilder
                                     let createPulsePal = ExpressionBuilder.GetWorkflowElement(builder) as CreatePulsePal
                                     where createPulsePal != null && !string.IsNullOrEmpty(createPulsePal.PortName)
                                     select !string.IsNullOrEmpty(createPulsePal.DeviceName) ? createPulsePal.DeviceName : createPulsePal.PortName)
                                     .Distinct()
                                     .ToList();
                    if (portNames.Count > 0)
                    {
                        return new StandardValuesCollection(portNames);
                    }
                }
            }

            return new StandardValuesCollection(SerialPort.GetPortNames());
        }
    }
}
