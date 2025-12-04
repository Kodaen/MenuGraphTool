using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MenuGraphTool
{
    public class MenuDirector : MonoBehaviour
    {
        #region Fields
        [SerializeField] private RuntimeMenuGraph _runtimeGraph;

        private Dictionary<string, RuntimeMenuNode> _nodeLookup = new();
        private RuntimeMenuNode _currentNode;
        private MenuPage _currentMenu;
        #endregion Fields

        // TEMP : Opening the graph should be done by calling the method OpenMenuGraph
        #region Methods
        private void Start()
        {
            // TEMP : Exemple Character
            Character chara = new() { Name = "bonjour" };
            OpenMenuGraph(_runtimeGraph, chara);
        }

        // TODO : Using params object[] is not good practice, as if we add a new variable in the graph, then errors will appear in
        // runtime when trying to open the graph
        public void OpenMenuGraph(RuntimeMenuGraph RuntimeGraph, params object[] variables)
        {
            if (string.IsNullOrEmpty(RuntimeGraph.EntryNodeID))
            {
                Debug.LogError($"{RuntimeGraph.name} doesn't have a start node");
                return;
            }

            foreach (RuntimeMenuNode node in RuntimeGraph.AllNodes)
            {
                _nodeLookup[node.NodeID] = node;
            }

            // TODO : Prone to mistakes and errors
            // Set Variables
            if (variables.Length != RuntimeGraph.AllVariables.Count)
            {
                Debug.LogWarning($"The number of variables to set for the MenuGraph {RuntimeGraph.name} doesn't correspond the number of parameters given to open it." +
                    $"Some errors may occur when navigating in the menus.");
            }

            for (int i = 0; i < variables.Length && i < RuntimeGraph.AllVariables.Count; i++)
            {
                RuntimeGraph.AllVariables[i].Value = variables[i];
            }

            OpenMenu(RuntimeGraph.EntryNodeID);
        }

        private void OpenMenu(string id)
        {
            // TODO : Hiding or not the previous menu should be an option
            DisablePreviousMenu();
            InstanciateNewMenu(id);
            PassParameterToMenu(_currentMenu, _currentNode);
        }

        private void DisablePreviousMenu()
        {
            if (_currentMenu)
            {
                _currentMenu.OnNextMenu -= OnNextMenu;
                _currentMenu.gameObject.SetActive(false);
            }
        }

        private void InstanciateNewMenu(string id)
        {
            _currentNode = _nodeLookup[id];
            _currentMenu = Instantiate(_currentNode.MenuPagePrefab);
            _currentNode.RuntimeMenuPage = _currentMenu;
            _currentMenu.OnNextMenu += OnNextMenu;
        }

        private void PassParameterToMenu(MenuPage targetMenu, RuntimeMenuNode targetNode)
        {
            foreach (KeyValuePair<string, InputInfos> kvp in targetNode.InputParamOutputDict)
            {
                BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                FieldInfo targetField = targetMenu.GetType().GetField(kvp.Key, flags);

                object param;
                switch (kvp.Value.InputOrigin)
                {
                    case InputOrigin.OtherNode:
                        param = RetrieveFieldFromNode(kvp.Value);
                        break;

                    case InputOrigin.Variable:
                        param = RetrieveFieldFromGraphVariable(kvp.Value);
                        break;

                    case InputOrigin.Field:
                        param = RetrieveFieldFromField(kvp.Value, targetField.FieldType);
                        break;

                    default:
                        Debug.LogError("Couldn't retrieve parameters for this menu.");
                        return;
                }

                targetField.SetValue(targetMenu, param);
            }
        }

        private object RetrieveFieldFromNode(InputInfos inputInfo)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            MenuPage sourceMenu = _nodeLookup[inputInfo.InputNodeID].RuntimeMenuPage;
            FieldInfo sourceField = sourceMenu.GetType().GetField(inputInfo.InputParamName, flags);

            return sourceField.GetValue(sourceMenu);
        }

        private object RetrieveFieldFromGraphVariable(InputInfos inputInfo)
        {
            return _runtimeGraph.AllVariables[inputInfo.VariableIndex].Value;
        }

        private object RetrieveFieldFromField(InputInfos inputInfo, Type type)
        {
            return Convert.ChangeType(inputInfo.rawVal, type);
        }
        #endregion Methods

        #region Callbacks
        private void OnNextMenu(string actionName)
        {
            OpenMenu(_nodeLookup[_currentNode.NextNodeDict[actionName]]?.NodeID);
        }
        #endregion Callbacks

    }
}
