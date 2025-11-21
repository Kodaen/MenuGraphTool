using System;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using MenuGraphTool;
using System.Collections.Generic;
using NUnit.Framework.Internal;

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

            context.AddInputPort("in")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            var option = GetNodeOptionByName(MENU_OPTION_ID);
            option.TryGetValue(out MenuPage menu);

            // TODO : Remove Mock value
            menu.actions.Clear();

            List<Type> types = new List<Type>();
            types.Add(typeof(int));
            types.Add(typeof(Character));
            types.Add(typeof(Character));

            KeyValuePair<Action, List<Type>> kvp = new KeyValuePair<Action, List<Type>>(new Action(test), types);

            menu.actions.Add(kvp);
            menu.actions.Add(kvp);
            // 

            for (int i = 0; i < menu.actions.Count; i++)
            {
                KeyValuePair<Action, List<Type>> tkvp = menu.actions[i];

                context.AddOutputPort($"Action {i}")
                  .WithConnectorUI(PortConnectorUI.Arrowhead)
                  .Build();

                for (int y = 0; y < tkvp.Value.Count; y++)
                {
                    Type a = tkvp.Value[y];
                    context.AddOutputPort($"{i}{y} : {a.Name}").WithDataType(a).Build();
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
