﻿using System.ComponentModel;
using Bonsai.Expressions;
using System.IO.Ports;
using System.Linq;

namespace Bonsai.PulsePal
{
    class PortNameConverter : StringConverter
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
                                     let createPort = ExpressionBuilder.GetWorkflowElement(builder) as CreatePulsePal
                                     where createPort != null && !string.IsNullOrEmpty(createPort.PortName)
                                     select !string.IsNullOrEmpty(createPort.Name) ? createPort.Name : createPort.PortName)
                                     .Distinct()
                                     .ToList();

                    if (portNames.Count > 0) return new StandardValuesCollection(portNames);
                    else return new StandardValuesCollection(SerialPort.GetPortNames());
                }
            }

            return base.GetStandardValues(context);
        }
    }
}
