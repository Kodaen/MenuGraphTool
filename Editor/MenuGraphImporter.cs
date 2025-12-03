using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace MenuGraphTool.Editor
{
    [ScriptedImporter(1, MenuGraph.ASSET_EXTENSION)]
    public class MenuGraphImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            MenuGraph editorGraph = GraphDatabase.LoadGraphForImporter<MenuGraph>(ctx.assetPath);
            RuntimeMenuGraph runtimeGraph = ScriptableObject.CreateInstance<RuntimeMenuGraph>();
            Dictionary<INode, string> nodeIdMap = new();

            foreach (INode node in editorGraph.GetNodes())
            {
                nodeIdMap[node] = Guid.NewGuid().ToString();
            }

            StartNode startNode = editorGraph.GetNodes().OfType<StartNode>().FirstOrDefault();
            if (startNode == null)
            {
                return;
            }

            IPort entryPort = startNode.GetOutputPorts().FirstOrDefault()?.firstConnectedPort;
            if (entryPort == null)
            {
                return;
            }

            runtimeGraph.EntryNodeID = nodeIdMap[entryPort.GetNode()];

            foreach (INode node in editorGraph.GetNodes())
            {
                if (node is StartNode)
                {
                    continue;
                }

                RuntimeMenuNode runtimeNode = new RuntimeMenuNode { NodeID = nodeIdMap[node] };
                if (node is MenuNode menuNode)
                {
                    ProcessMenuGraphNode(menuNode, runtimeNode, nodeIdMap);
                }
                runtimeGraph.AllNodes.Add(runtimeNode);
            }

            ctx.AddObjectToAsset("RuntimeData", runtimeGraph);
            ctx.SetMainObject(runtimeGraph);
        }

        private void ProcessMenuGraphNode(MenuNode node, RuntimeMenuNode runtimeNode, Dictionary<INode, string> nodeIdMap)
        {
            INodeOption menuPrefab = node.GetNodeOptionByName(MenuNode.MENU_OPTION_ID);

            // Process MenuNode
            if (menuPrefab.TryGetValue<MenuPage>(out MenuPage menuPagePrefab) && menuPagePrefab != null)
            {
                runtimeNode.MenuPagePrefab = menuPagePrefab;

                IEnumerable<IPort> inputPorts = node.GetInputPorts().Where(p => p.dataType != typeof(ExecutionFlow));

                foreach (IPort inputPort in inputPorts)
                {
                    if (!inputPort.isConnected)
                    {
                        continue;
                    }

                    string inputNodeID = nodeIdMap[inputPort.firstConnectedPort.GetNode()];
                    // TODO : Don't use display name
                    string inputParamName = inputPort.firstConnectedPort.displayName;

                    // TODO : Don't use display name
                    string paramName = inputPort.displayName;
                    InputInfos inputInfos = new()
                    {
                        InputNodeID = inputNodeID,
                        InputParamName = inputParamName
                    };
                    runtimeNode.InputParamOutputDict.Add(paramName, inputInfos);
                }
            }

            // Process flow
            IEnumerable<IPort> outputFlowPort = node.GetOutputPorts()
                .Where(p => p.dataType == typeof(ExecutionFlow));

            foreach (IPort flowPort in outputFlowPort)
            {
                if (!flowPort.isConnected)
                {
                    continue;
                }
                // TODO : Don't use display name
                runtimeNode.NextNodeDict.Add(flowPort.displayName, nodeIdMap[flowPort.firstConnectedPort.GetNode()]);
            }
        }
    }
}

