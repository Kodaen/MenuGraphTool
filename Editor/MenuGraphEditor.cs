using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace MenuGraphTool.Editor
{
    [Serializable]
    [Graph(ASSET_EXTENSION)]
    public class MenuGraphEditor : Graph
    {
        #region Fields
        internal const string DEFAULT_GRAPH_NAME = "New Menu Graph";
        internal const string ASSET_EXTENSION = "menugraph";
        #endregion Fields

        #region Static Methods
        [MenuItem("Assets/Create/Menu Graph")]
        private static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<MenuGraphEditor>(DEFAULT_GRAPH_NAME);
        }
        #endregion Static Methods

        #region Methods
        public override void OnGraphChanged(GraphLogger infos)
        {
            base.OnGraphChanged(infos);
            CheckGraphErrors(infos);
        }

        private void CheckGraphErrors(GraphLogger infos)
        {
            CheckStartNode(infos);
            CheckInputPorts(infos);
        }

        private void CheckInputPorts(GraphLogger infos)
        {
            List<INode> nodes = GetNodes().ToList();
            foreach (INode node in nodes)
            {
                IEnumerable<IPort> inputPorts = node.GetInputPorts();
                foreach (IPort inputPort in inputPorts)
                {
                    if (inputPort.isConnected == false && !inputPort.dataType.IsPrimitive)
                    {
                        infos.LogWarning($"input node {inputPort.displayName} should be connected", node);
                    }
                }
            }
        }

        private void CheckStartNode(GraphLogger infos)
        {
            List<StartNode> startNodes = GetNodes().OfType<StartNode>().ToList();
            switch (startNodes.Count)
            {
                case 0:
                    infos.LogWarning("Add a StartNode in your Menu Graph.", this);
                    break;
                case >= 1:
                    {
                        foreach (StartNode startNode in startNodes.Skip(1))
                        {
                            infos.LogWarning($"MenuGraphTool only supports one StartNode per graph. Only the first created one will be used.", startNode);
                        }
                        break;
                    }
            }
        }
        #endregion Methods

    }
}
