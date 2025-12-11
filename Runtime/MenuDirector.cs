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
        public void OpenMenuGraph(MenuGraph runtimeGraph, params object[] variables)
        {
            if (string.IsNullOrEmpty(runtimeGraph.EntryNodeID))
            {
                Debug.LogError($"{runtimeGraph.name} doesn't have a start node");
                return;
            }

            ClearMenuGraph();
            CurrentActionReference = _defaultActionReference;

            foreach (MenuNode node in runtimeGraph.AllNodes)
            {
                _nodeLookup[node.NodeID] = node;
            }

            // TODO : Prone to mistakes and errors
            // Set Variables
            if (variables.Length != runtimeGraph.AllVariables.Count)
            {
                Debug.LogWarning($"The number of variables to set for the MenuGraph {runtimeGraph.name} doesn't correspond the number of parameters given to open it." +
                    $"Some errors may occur when navigating in the menus.");
            }

            for (int i = 0; i < variables.Length && i < runtimeGraph.AllVariables.Count; i++)
            {
                runtimeGraph.AllVariables[i].Value = variables[i];
            }

            _runtimeGraph = runtimeGraph;

            TryOpenMenu(runtimeGraph.EntryNodeID);
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

            MenuPage menuPage = InstanciateMenu(id);

            if (menuPage.OpeningMode == MenuOpeningMode.Add)
            {
                UnfocusMenu(_currentMenu);
            }
            else
            {
                DisableMenu(_currentMenu);
            }

            SetCurrentMenu(menuPage);
            PassParameterToMenu(_currentMenu, _currentNode);

            return true;
        }

        private bool TryOpenParentMenu()
        {
            DestroyMenu(_currentMenu);

            MenuPage parentMenuPage = _currentMenu.Parent;
            if (parentMenuPage == null)
            {
                return false;
            }

            if (parentMenuPage.OpeningMode == MenuOpeningMode.Add)
            {
                ReopenAdditiveParentMenu(parentMenuPage);
            }
   
            SetCurrentMenu(parentMenuPage);
          
            return true;
        }

        private void ReopenAdditiveParentMenu(MenuPage menuPage)
        {
            MenuPage parentMenuPage = menuPage.Parent;
            if (parentMenuPage == null)
            {
                return;
            }

            parentMenuPage.gameObject.SetActive(true);
            if (parentMenuPage.OpeningMode == MenuOpeningMode.Add)
            {
                ReopenAdditiveParentMenu(parentMenuPage);
            }
        }

        private MenuPage InstanciateMenu(string id)
        {
            MenuPage menuPage = Instantiate(_nodeLookup[id].MenuPagePrefab);
            menuPage.Parent = _currentMenu ?? null;
            menuPage.RuntimeMenuNode = _nodeLookup[id];

            return menuPage;
        }

        private void SetCurrentMenu(MenuPage menuPage)
        {
            _currentMenu = menuPage;

            _currentNode = _nodeLookup[menuPage.RuntimeMenuNode.NodeID];
            _currentNode.RuntimeMenuPage = _currentMenu;

            _currentMenu.gameObject.SetActive(true);
            FocusMenu(_currentMenu);
            AssignBackInput();
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

        #region Menu Operations
        private void UnfocusMenu(MenuPage menuPage)
        {
            if (menuPage == null)
            {
                return;
            }

            menuPage.OnNextMenu -= OnNextMenu;
            // TODO : Not sure about this one
            menuPage.OnExitMenu -= OnExitMenu;
            menuPage.OnMenuUnfocused();
        }

        private void FocusMenu(MenuPage menuPage)
        {
            if (menuPage == null)
            {
                return;
            }

            if (menuPage.LastSelected != null)
            {
                EventSystem.current.SetSelectedGameObject(menuPage.LastSelected.gameObject);
            }
            else if (menuPage.FirstSelected != null)
            {
                EventSystem.current.SetSelectedGameObject(menuPage.FirstSelected.gameObject);
            }

            menuPage.OnNextMenu += OnNextMenu;
            menuPage.OnExitMenu += OnExitMenu;
            menuPage.OnMenuFocused();
        }

        private void DisableMenu(MenuPage menuPage)
        {
            if (menuPage == null)
            {
                return;
            }

            if (menuPage.OpeningMode == MenuOpeningMode.Add)
            {
                DisableMenu(menuPage.Parent);
            }

            menuPage.OnNextMenu -= OnNextMenu;
            menuPage.OnExitMenu -= OnExitMenu;
            menuPage.gameObject.SetActive(false);
        }

        private void DestroyMenu(MenuPage menuPage)
        {
            if (menuPage == null)
            {
                return;
            }

            menuPage.OnNextMenu -= OnNextMenu;
            menuPage.OnExitMenu -= OnExitMenu;
            Destroy(menuPage.gameObject);

        } 
        #endregion Menu Operations

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

        private void OnExitMenu()
        {
            if (TryOpenParentMenu() == false)
            {
                CloseMenuCurrentMenuGraph();
            }
        }
        #endregion Callbacks
    }
}
