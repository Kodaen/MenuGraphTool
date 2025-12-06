using System;
using UnityEngine;

namespace MenuGraphTool
{
    public class MenuPage : MonoBehaviour
    {
        #region Fields
        private RuntimeMenuNode _runtimeMenuNode;
        private MenuPage _parent;

        #endregion Fields

        #region Properties
        public RuntimeMenuNode RuntimeMenuNode
        {
            get { return _runtimeMenuNode; }
            set { _runtimeMenuNode = value; }
        }

        public MenuPage Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        #endregion Properties

        #region Events
        protected Action<string> _onNextMenu = null;
        public virtual event Action<string> OnNextMenu
        {
            add
            {
                _onNextMenu -= value;
                _onNextMenu += value;
            }
            remove
            {
                _onNextMenu -= value;
            }
        } 
        #endregion Events


        #region Methods
        public virtual void OpenNextMenu(string ExecutionFlow)
        {
            _onNextMenu?.Invoke(ExecutionFlow);
        } 
        #endregion Methods
    }

    // TEMP : Remove this exemple class
    public class Character
    {
        public string Name;
    }
}