using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MenuGraphTool
{
    public class MenuPage : MonoBehaviour
    {
        #region Fields
        private MenuNode _runtimeMenuNode;
        private MenuPage _parent;
        private CanvasGroup _canvasGroup;

        [Header("Menu parameters")]
        [SerializeField] private Selectable _firstSelectedElement;
        [Tooltip(
            "Replace: Opens menu and disables parent menu. Useful for fullscreen menus.\r\n\r\n" +
            "Add: Opens the menu without closing its parent. Useful for popups. " +
            "It is highly recommanded to add a canvas group to your prefab in order to disable navigation input in the parent Menu.")]
        [SerializeField] private MenuOpeningMode _openingMode;
        [SerializeField] private BackActionReference _backInput;

        protected GameObject _lastSelectedElement;
        #endregion Fields

        #region Properties
        public MenuNode RuntimeMenuNode
        {
            get { return _runtimeMenuNode; }
            set { _runtimeMenuNode = value; }
        }

        public MenuPage Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public MenuOpeningMode OpeningMode
        {
            get { return _openingMode; }
            set { _openingMode = value; }
        }

        public Selectable FirstSelected
        {
            get { return _firstSelectedElement; }
            set { _firstSelectedElement = value; }
        }

        public GameObject LastSelected
        {
            get { return _lastSelectedElement; }
            set { _lastSelectedElement = value; }
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

        private Action _onExitMenu = null;
        public event Action OnExitMenu
        {
            add
            {
                _onExitMenu -= value;
                _onExitMenu += value;
            }
            remove
            {
                _onExitMenu -= value;
            }
        }
        #endregion Events

        #region Methods
        public virtual void OpenNextMenu(string ExecutionFlow)
        {
            GameObject currentSelectedObject = EventSystem.current.currentSelectedGameObject;
            if (currentSelectedObject != null)
            {
                _lastSelectedElement = currentSelectedObject;
            }

            _onNextMenu?.Invoke(ExecutionFlow);
        }

        public virtual void ExitMenu()
        {
            _onExitMenu?.Invoke();
        }

        public virtual void OnMenuFocused()
        {
            SetCanvasGroupInteractible(true);
        }

        public virtual void OnMenuUnfocused()
        {
            SetCanvasGroupInteractible(false);
        }

        private void SetCanvasGroupInteractible(bool interactible = true)
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponentInChildren<CanvasGroup>();
                if (_canvasGroup == null)
                {
                    return;
                }
            }
            _canvasGroup.interactable = interactible;
        }
        #endregion Methods
    }
}