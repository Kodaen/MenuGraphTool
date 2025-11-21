using System;
using System.Reflection;
using Unity.GraphToolkit.Editor;

namespace MenuGraphTool.Editor
{
    [Serializable]
    internal class MenuNode : Node
    {
        #region Constants
        const string MENU_OPTION_ID = "menu";
        #endregion Constants

        #region Methods
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            int outputCount = 0;
            int inputCount = 0;


            context.AddInputPort(inputCount.ToString())
                .WithDisplayName("")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
            inputCount++;

            // Add ports depending on the selected menu page
            INodeOption option = GetNodeOptionByName(MENU_OPTION_ID);
            if (option.TryGetValue(out MenuPage menu) == false || menu == null)
            {
                return;
            }

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo[] fields = menu.GetType().GetFields(flags);

            // Add input ports
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                MenuInputAttribute menuOutputAttribute = field.GetCustomAttribute<MenuInputAttribute>();
                if (menuOutputAttribute == null)
                {
                    continue;
                }

                context.AddInputPort(inputCount.ToString())
                  .WithDataType(field.FieldType)
                  .WithDisplayName(field.Name)
                  .Build();
                inputCount++;
            }

            // Add OutPut ports
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                MenuOutputAttribute menuOutputAttribute = field.GetCustomAttribute<MenuOutputAttribute>();
                if (menuOutputAttribute == null)
                {
                    continue;
                }

                string outputFlowName = menuOutputAttribute.DisplayName ?? field.Name;

                context.AddOutputPort(outputCount.ToString())
                  .WithDisplayName(outputFlowName)
                  .WithConnectorUI(PortConnectorUI.Arrowhead)
                  .Build();
                outputCount++;

                Type[] genericArgs = field.FieldType.GenericTypeArguments;

                for (int j = 0; j < genericArgs.Length; j++)
                {
                    Type paramType = genericArgs[j];

                    string outputParamName = menuOutputAttribute.OutputParamsName.Length > j
                        ? menuOutputAttribute.OutputParamsName[j]
                        : paramType.Name;
      

                    context.AddOutputPort(outputCount.ToString())
                        .WithDisplayName(outputParamName)
                        .WithDataType(paramType)
                        .Build();
                    outputCount++;
                }
            }
        }

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            context.AddOption<MenuPage>(MENU_OPTION_ID);
        }

        private void test()
        {

        }
        #endregion Methods
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
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
        #endregion Methods
    }
}
