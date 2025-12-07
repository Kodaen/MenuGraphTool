using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MenuGraphTool
{
    public class MenuPage : MonoBehaviour
    {
        #region Fields
        private RuntimeMenuNode _runtimeMenuNode;
        private MenuPage _parent;

        [SerializeField] private BackActionReference _backInput;
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

        public BackActionReference BackInput
        {
            get { return _backInput; }
            set { _backInput = value; }
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
}