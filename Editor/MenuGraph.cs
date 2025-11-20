using UnityEngine;
using UnityEditor;
using Unity.GraphToolkit.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;

namespace MenuGraphTool.Editor
{
    [Serializable]
    [Graph(ASSET_EXTENSION)]
    public class MenuGraph : Graph
    {
        #region Fields
        internal const string DEFAULT_GRAPH_NAME = "New Menu Graph";
        internal const string ASSET_EXTENSION = "menugraph";
        #endregion Fields

        #region Static Methods
        [MenuItem("Assets/Create/Menu Graph")]
        private static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<MenuGraph>(DEFAULT_GRAPH_NAME);
        }
        #endregion Static Methods


        #region Methods
        public override void OnGraphChanged(GraphLogger infos)
        {
            base.OnGraphChanged(infos);

            CheckGraphErrors(infos);
        }

        void CheckGraphErrors(GraphLogger infos)
        {
            List<StartNode> startNodes = GetNodes().OfType<StartNode>().ToList();
            switch (startNodes.Count)
            {
                case 0:
                    infos.LogError("Add a StartNode in your Menu Graph.", this);
                    break;
                case >= 1:
                    {
                        foreach (var startNode in startNodes.Skip(1))
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
