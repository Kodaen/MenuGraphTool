using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace MenuGraphTool
{
    public class MenuDirector : MonoSingleton<MenuDirector>
    {
        #region Fields
        // Graph
        private MenuGraph _runtimeGraph;
        private Dictionary<string, MenuNode> _nodeLookup = new();
        private MenuNode _currentNode;

        // Runtime Menus
        private MenuPage _currentMenu;

        [SerializeField] private InputAction _defaultActionReference = null;
        private InputAction _currentActionReference = null;
        #endregion Fields

        #region Properties
        public InputAction CurrentActionReference
        {
            get { return _currentActionReference; }
            set
            {
                if (_currentActionReference == value)
                {
                    return;
                }

                EnableBackInput(false);
                _currentActionReference = value;
                EnableBackInput(true);
            }
        }
        #endregion Properties

        #region Event
        private static Action _onCurrentMenuGraphCloses = null;
        public static event Action OnCurrentMenuGraphCloses
        {
            add
            {
                _onCurrentMenuGraphCloses -= value;
                _onCurrentMenuGraphCloses += value;
            }
            remove
            {
                _onCurrentMenuGraphCloses -= value;
            }
        }
        #endregion Event

        #region Methods
        private void Awake()
        {
            // TODO : Depending on the menu opened, it shall automatically detect to disable the back action if
            // the parent menu had it disabled when opening its submenu, and it is openned back
        }

        private void ClearMenuGraph()
        {
            _runtimeGraph = null;
            _nodeLookup.Clear();
            _currentNode = null;
            _currentMenu = null;
            CurrentActionReference = null;
        }

        #region Opening Menus
        // TODO : Using params object[] is not good practice, as if we add a new variable in the graph, then errors will appear in
        // runtime when trying to open the graph
        public void OpenMenuGraph(MenuGraph RuntimeGraph, params object[] variables)
        {
            if (string.IsNullOrEmpty(RuntimeGraph.EntryNodeID))
            {
                Debug.LogError($"{RuntimeGraph.name} doesn't have a start node");
                return;
            }

            ClearMenuGraph();
            CurrentActionReference = _defaultActionReference;

            foreach (MenuNode node in RuntimeGraph.AllNodes)
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

        private void CloseMenuCurrentMenuGraph()
        {
            ClearMenuGraph();

            _onCurrentMenuGraphCloses?.Invoke();
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

            AssignBackInput();

            return true;
        }

        private bool TryOpenParentMenu()
        {
            DestroyCurrentMenu();

            MenuPage parentMenuPage = _currentMenu.Parent;
            if (parentMenuPage == null)
            {
                return false;
            }

            SetCurrentMenu(parentMenuPage);
            AssignBackInput();
            return true;
        }

        private MenuPage InstanciateMenu(string id)
        {
            MenuPage menuPage = Instantiate(_nodeLookup[id].MenuPagePrefab);
            menuPage.Parent = _currentMenu ?? null;
            menuPage.RuntimeMenuNode = _nodeLookup[id];

            if (menuPage.FirstSelected != null)
            {
                EventSystem.current.SetSelectedGameObject(menuPage.FirstSelected.gameObject);
            }

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

        private void PassParameterToMenu(MenuPage targetMenu, MenuNode targetNode)
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

        private void AssignBackInput()
        {
            switch (_currentMenu.BackInput.BackInputAction)
            {
                case BackActionType.Default:
                    CurrentActionReference = _defaultActionReference;
                    break;

                case BackActionType.Override:
                    CurrentActionReference = _currentMenu.BackInput.InputAction;
                    break;

                case BackActionType.None:
                    CurrentActionReference = null;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
        #endregion Opening Menus

        #region Inputs
        private void OnBackPerformed(InputAction.CallbackContext context)
        {
            if (TryOpenParentMenu() == false)
            {
                CloseMenuCurrentMenuGraph();
            }
        }

        public void EnableBackInput(bool enable = true)
        {
            if (_currentActionReference == null)
            {
                return;
            }

            if (enable)
            {
                _currentActionReference.Enable();
                _currentActionReference.performed += OnBackPerformed;
            }
            else
            {
                _currentActionReference.Disable();
                _currentActionReference.performed -= OnBackPerformed;
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
