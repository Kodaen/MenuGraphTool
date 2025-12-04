using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.GraphToolkit.Editor;

namespace MenuGraphTool.Editor
{
    [Serializable]
    public class MenuNode : Node
    {
        #region Fields
        #region Constants
        public const string MENU_OPTION_ID = "menu";
        private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        #endregion Constants

        private int _outputCount = 0;
        private int _inputCount = 0;
        #endregion Fields

        #region Methods
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            _outputCount = 0;
            _inputCount = 0;

            // Add default input port
            context.AddInputPort(_inputCount.ToString())
                .WithDataType<ExecutionFlow>()
                .WithDisplayName("")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
            _inputCount++;

            // Add ports depending on the selected menu page
            INodeOption option = GetNodeOptionByName(MENU_OPTION_ID);
            if (option.TryGetValue(out MenuPage menu) == false || menu == null)
            {
                return;
            }

            FieldInfo[] fields = menu.GetType().GetFields(BINDING_FLAGS);

            AddInputPorts(context, fields);
            AddOutputPorts(context, fields);
        }

        private void AddOutputPorts(IPortDefinitionContext context, FieldInfo[] fields)
        {
            Dictionary<string, List<FieldInfo>> outputDict = new();

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                IEnumerable<MenuOutputAttribute> menuOutputAttributes = field.GetCustomAttributes<MenuOutputAttribute>();
                foreach (MenuOutputAttribute menuOutputAttribute in menuOutputAttributes)
                {
                    if (!outputDict.ContainsKey(menuOutputAttribute.FlowName))
                    {
                        outputDict[menuOutputAttribute.FlowName] = new();
                    }

                    outputDict[menuOutputAttribute.FlowName].Add(field);
                }
            }

            foreach (string flowName in outputDict.Keys)
            {
                context.AddOutputPort(_outputCount.ToString())
                 .WithDataType<ExecutionFlow>()
                 .WithDisplayName(flowName)
                 .WithConnectorUI(PortConnectorUI.Arrowhead)
                 .Build();
                _outputCount++;

                foreach (FieldInfo field in outputDict[flowName])
                {
                    context.AddOutputPort(_outputCount.ToString())
                        .WithDisplayName(field.Name)
                        .WithDataType(field.FieldType)
                        .Build();
                    _outputCount++;
                }
            }
        }

        private void AddInputPorts(IPortDefinitionContext context, FieldInfo[] fields)
        {
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                MenuInputAttribute menuOutputAttribute = field.GetCustomAttribute<MenuInputAttribute>();
                if (menuOutputAttribute == null)
                {
                    continue;
                }

                context.AddInputPort(_inputCount.ToString())
                  .WithDataType(field.FieldType)
                  .WithDisplayName(field.Name)
                  .Build();
                _inputCount++;
            }
        }

        #endregion Methods
        #region Callbacks
        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<MenuPage>(MENU_OPTION_ID);
        }
        #endregion Callbacks

    }

    [Serializable]
    internal class StartNode : Node
    {
        #region Constants
        #endregion Constants

        #region Methods
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort("out")
                .WithDataType<ExecutionFlow>()
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
        #endregion Methods
    }

    // TODO : Bad naming
    public class ExecutionFlow
    { }
}
