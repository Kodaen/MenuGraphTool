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
        public const string MENU_PREVIEW_ID = "menu_preview";
        private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        #endregion Constants

        private int _outputCount = 0;
        private int _inputCount = 0;
        #endregion Fields

        #region Methods
        private void AddOutputPorts(IPortDefinitionContext context, FieldInfo[] fields)
        {
            Dictionary<string, List<FieldInfo>> outputDict = new();

            if (fields.Length == 0)
            {
                return;
            }

            // Register parameterless flows in the dict
            IEnumerable<MenuOutputAttribute> menuOutputAttributes = fields[0].DeclaringType.GetCustomAttributes<MenuOutputAttribute>();
            foreach (MenuOutputAttribute flow in menuOutputAttributes)
            {
                if (!outputDict.ContainsKey(flow.FlowName))
                {
                    outputDict[flow.FlowName] = new();
                }
            }

            // Register flows with parameters in the dict
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                IEnumerable<MenuOutputAttribute> menuOutputWithParametersAttributes = field.GetCustomAttributes<MenuOutputAttribute>();
                foreach (MenuOutputAttribute menuOutputAttribute in menuOutputWithParametersAttributes)
                {
                    if (!outputDict.ContainsKey(menuOutputAttribute.FlowName))
                    {
                        outputDict[menuOutputAttribute.FlowName] = new();
                    }

                    outputDict[menuOutputAttribute.FlowName].Add(field);
                }
            }

            // Create the ports based on the dictionnary
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

        private void AddMenuPreview(MenuPage menu)
        {
            INodeOption option2 = GetNodeOptionByName(MENU_PREVIEW_ID);
            if (option2.TryGetValue(out MenuPreview menuPreview) == false || menu == null)
            {
                return;
            }
            menuPreview.MenuPage = menu;
        }
        #endregion Methods
        #region Callbacks
        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<MenuPage>(MENU_OPTION_ID);
            context.AddOption<MenuPreview>(MENU_PREVIEW_ID);
        }

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

            AddMenuPreview(menu);

            FieldInfo[] fields = menu.GetType().GetFields(BINDING_FLAGS);
       
            AddInputPorts(context, fields);
            AddOutputPorts(context, fields);
        }
        #endregion Callbacks
    }

    [Serializable]
    internal class StartNode : Node
    {
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

    public class ExecutionFlow
    { }
}
