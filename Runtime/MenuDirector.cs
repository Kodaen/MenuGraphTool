using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MenuGraphTool
{
    public class MenuDirector : MonoBehaviour
    {
        #region Fields
        [SerializeField] private RuntimeMenuGraph RuntimeGraph;

        private Dictionary<string, RuntimeMenuNode> _nodeLookup = new();
        private RuntimeMenuNode _currentNode;
        private MenuPage _currentMenu; 
        #endregion Fields


        // TEMP : Opening the 
        #region Methods
        private void Start()
        {
            foreach (RuntimeMenuNode node in RuntimeGraph.AllNodes)
            {
                _nodeLookup[node.NodeID] = node;
            }

            if (!string.IsNullOrEmpty(RuntimeGraph.EntryNodeID))
            {
                // TODO : We need to fill the BlackBoard before opening it
                // Or pass the parameters for the graph
                OpenMenu(RuntimeGraph.EntryNodeID);
            }
            else
            {

            }
        }


        private void OpenMenu(string id, string actionName = null)
        {
            // TODO : Hiding or not the previous menu should be an option
            DisablePreviousMenu();
            InstanciateNewMenu(id);
            // TODO : No need of previousMenu, we have to get the Id
            PassParameterToMenu(_currentMenu, _currentNode, actionName);
        }


        private void PassParameterToMenu(MenuPage targetMenu, RuntimeMenuNode targetNode, string actionName)
        {
            if (string.IsNullOrEmpty(actionName))
            {
                return;
            }

            foreach (KeyValuePair<string, InputInfos> kvp in targetNode.InputParamOutputDict)
            {
                BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

                FieldInfo targetField = targetMenu.GetType().GetField(kvp.Key, flags);

                MenuPage sourceMenu = _nodeLookup[kvp.Value.InputNodeID].RuntimeMenuPage;
                FieldInfo sourceField = sourceMenu.GetType().GetField(kvp.Value.InputParamName, flags);

                targetField.SetValue(targetMenu, sourceField.GetValue(sourceMenu));
            }
        }

        private void InstanciateNewMenu(string id)
        {
            _currentNode = _nodeLookup[id];
            _currentMenu = Instantiate(_currentNode.MenuPagePrefab);
            _currentNode.RuntimeMenuPage = _currentMenu;
            _currentMenu.OnNextMenu += OnNextMenu;
        }

        private void DisablePreviousMenu()
        {
            if (_currentMenu)
            {
                _currentMenu.OnNextMenu -= OnNextMenu;
                _currentMenu.gameObject.SetActive(false);
            }
        } 
        #endregion Methods


        #region Callbacks
        private void OnNextMenu(string actionName)
        {
            OpenMenu(_nodeLookup[_currentNode.NextNodeDict[actionName]]?.NodeID, actionName);
        } 
        #endregion Callbacks

    }
}
