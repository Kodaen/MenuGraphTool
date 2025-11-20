using System;
using Unity.GraphToolkit.Editor;

namespace MenuGraphTool.Editor
{
    [Serializable]
    internal class MenuNode : Node
    {
        #region Constants
        #endregion Constants

        #region Methods
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort("in")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort("out")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
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
