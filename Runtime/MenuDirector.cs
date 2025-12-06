using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;


namespace MenuGraphTool
{
    public class MenuDirector : MonoBehaviour
    {
        #region Fields
        // Graph
        [SerializeField] private RuntimeMenuGraph _runtimeGraph;
        private Dictionary<string, RuntimeMenuNode> _nodeLookup = new();
        private RuntimeMenuNode _currentNode;

        // Runtime Menus
        private MenuPage _currentMenu;

        [SerializeField] private InputActionReference _backActionReference = null;
        #endregion Fields

        #region Methods
        private void Awake()
        {
            // TODO : Make this class a singleton, so it's easier to open menus, and to disable inputs
            // TODO : Depending on the menu opened, it shall automatically detect to desiable the back action if
            // the parent menu had it disabled when opening its submenu, and it is openned back
            _backActionReference.action.Enable();
            _backActionReference.action.performed += OnBackPerformed;
        }

        // TEMP : Opening the graph should be done by calling the method OpenMenuGraph
        private void Start()
        {
            // TEMP : Exemple Character
            Character chara = new() { Name = "bonjour" };
            OpenMenuGraph(_runtimeGraph, chara);
        }

        #region Opening Menus
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

            TryOpenMenu(RuntimeGraph.EntryNodeID);
        }

        public void CloseMenuCurrentMenuGraph()
        {
            // TODO : Invoke an event to tell the caller of the menu graph that it is closed
        }

        private bool TryOpenMenu(string id)
        {
            if (!_nodeLookup.ContainsKey(id))
            {
                Debug.LogWarning("Couldn't find menu to open.");
                return false;
            }
            // TODO : Hiding or not the previous menu should be an option
            DisableCurrentMenu();
            MenuPage menuPage = InstanciateMenu(id);
            SetCurrentMenu(menuPage);
            PassParameterToMenu(_currentMenu, _currentNode);
            return true;
        }

        private bool TryOpenParentMenu()
        {
            DestroyCurrentMenu();

            MenuPage parentMenuPage = _currentMenu.Parent;
            if (parentMenuPage == null)
            {
                // TODO : should we close the menu graph then ? CloseMenuGraph()
                return false;
            }

            SetCurrentMenu(parentMenuPage);
            return true;
        }

        private MenuPage InstanciateMenu(string id)
        {
            MenuPage menuPage = Instantiate(_nodeLookup[id].MenuPagePrefab);
            menuPage.Parent = _currentMenu ?? null;
            menuPage.RuntimeMenuNode = _nodeLookup[id];

            return menuPage;
        }

        private void DisableCurrentMenu()
        {
            if (_currentMenu)
            {
                _currentMenu.OnNextMenu -= OnNextMenu;
                _currentMenu.gameObject.SetActive(false);
            }
        }

        private void DestroyCurrentMenu()
        {
            if (_currentMenu)
            {
                _currentMenu.OnNextMenu -= OnNextMenu;
                Destroy(_currentMenu.gameObject);
            }
        }

        private void SetCurrentMenu(MenuPage menuPage)
        {
            _currentMenu = menuPage;

            _currentNode = _nodeLookup[menuPage.RuntimeMenuNode.NodeID];
            _currentNode.RuntimeMenuPage = _currentMenu;

            _currentMenu.gameObject.SetActive(true);
            _currentMenu.OnNextMenu += OnNextMenu;
        }

        private void PassParameterToMenu(MenuPage targetMenu, RuntimeMenuNode targetNode)
        {
            // TODO : targetMenu = targetNode.MenuPagePrefab
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
        #endregion Opening Menus

        #region Inputs
        private void OnBackPerformed(InputAction.CallbackContext context)
        {
            if (TryOpenParentMenu() == false)
            {
                // TODO : should we close the menu graph then ? CloseMenuGraph()
            }
        }
        #endregion Inputs
        #endregion Methods

        #region Callbacks
        private void OnNextMenu(string actionName)
        {
            if (!_currentNode.NextNodeDict.ContainsKey(actionName))
            {
                Debug.LogWarning($"No menu assigned for the flow \"{actionName}\" of the menu \"{_currentNode.MenuPagePrefab.name}\".");
                return;
            }

            string id = _currentNode.NextNodeDict[actionName];
            if (!_nodeLookup.ContainsKey(id))
            {
                Debug.LogWarning($"No menu assigned for the flow \"{actionName}\" of the menu \"{_currentNode.MenuPagePrefab.name}\".");
                return;
            }
            
            TryOpenMenu(_nodeLookup[id].NodeID);
        }
        #endregion Callbacks
    }
}
