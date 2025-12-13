using System;
using System.Collections.Generic;
using System.Linq;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace MenuGraphTool.Editor
{
    [ScriptedImporter(1, MenuGraphEditor.ASSET_EXTENSION)]
    public class MenuGraphImporter : ScriptedImporter
    {
        private MenuGraphEditor _editorGraph;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            _editorGraph = GraphDatabase.LoadGraphForImporter<MenuGraphEditor>(ctx.assetPath);
            MenuGraph runtimeGraph = ScriptableObject.CreateInstance<MenuGraph>();
            Dictionary<INode, string> nodeIdMap = new();

            foreach (INode node in _editorGraph.GetNodes())
            {
                nodeIdMap[node] = Guid.NewGuid().ToString();
            }

            StartNode startNode = _editorGraph.GetNodes().OfType<StartNode>().FirstOrDefault();
            if (startNode == null)
            {
                return;
            }

            IPort entryPort = startNode.GetOutputPorts().FirstOrDefault()?.firstConnectedPort;
            if (entryPort == null)
            {
                return;
            }

            foreach (IVariable variable in _editorGraph.GetVariables())
            {
                runtimeGraph.AllVariables.Add(new());
            }

            runtimeGraph.EntryNodeID = nodeIdMap[entryPort.GetNode()];

            foreach (INode node in _editorGraph.GetNodes())
            {
                if (node is StartNode)
                {
                    continue;
                }

                MenuGraphTool.MenuNode runtimeNode = new MenuGraphTool.MenuNode { NodeID = nodeIdMap[node] };
                if (node is MenuNode menuNode)
                {
                    ProcessMenuGraphNode(menuNode, runtimeNode, nodeIdMap);
                }
                runtimeGraph.AllNodes.Add(runtimeNode);
            }

            ctx.AddObjectToAsset("RuntimeData", runtimeGraph);
            ctx.SetMainObject(runtimeGraph);
        }

        private void ProcessMenuGraphNode(MenuNode node, MenuGraphTool.MenuNode runtimeNode, Dictionary<INode, string> nodeIdMap)
        {
            INodeOption menuPrefab = node.GetNodeOptionByName(MenuNode.MENU_OPTION_ID);

            // Process MenuNode
            if (menuPrefab.TryGetValue<MenuPage>(out MenuPage menuPagePrefab) && menuPagePrefab != null)
            {
                runtimeNode.MenuPagePrefab = menuPagePrefab;

                IEnumerable<IPort> inputPorts = node.GetInputPorts().Where(p => p.dataType != typeof(ExecutionFlow));

                foreach (IPort inputPort in inputPorts)
                {
                    // TODO : Don't use display name
                    string paramName = inputPort.displayName;
                    InputInfos inputInfos = new();

                    // Input comes from a Field
                    if (inputPort.isConnected == false)
                    {
                        if (!inputPort.TryGetValue(out object value) || value == null)
                        {
                            continue;
                        }

                        if (!value.GetType().IsPrimitive)
                        {
                            continue;
                        }

                        inputInfos.InputOrigin = InputOrigin.Field;
                        inputInfos.rawVal = value.ToString(); 
                    }
                    // Input comes from a Variable
                    else if (inputPort.firstConnectedPort.GetNode() is IVariableNode variable)
                    {
                        if (!TryGetVariableIndex(variable.variable, out int index))
                        {
                            continue;
                        }

                        inputInfos.InputOrigin = InputOrigin.Variable;
                        inputInfos.VariableIndex = index;
                    }
                    // Input comes from another Node
                    else
                    {
                        inputInfos.InputOrigin = InputOrigin.OtherNode;
                        string inputNodeID = nodeIdMap[inputPort.firstConnectedPort.GetNode()];
                        // TODO : Don't use display name
                        string inputParamName = inputPort.firstConnectedPort.displayName;

                        inputInfos.InputNodeID = inputNodeID;
                        inputInfos.InputParamName = inputParamName;
                    }

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

        private bool TryGetVariableIndex(IVariable variable, out int index)
        {
            index = -1;

            int count = _editorGraph.GetVariables().Count();
            for (int i = 0; i < count; i++)
            {
                if (variable == _editorGraph.GetVariable(i))
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }
    }
}

