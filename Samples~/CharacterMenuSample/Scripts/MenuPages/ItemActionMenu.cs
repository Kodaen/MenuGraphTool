using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MenuGraphTool
{
    [MenuOutput(INSPECT_ITEM)]
    public class ItemActionMenu : MenuPage
    {
        #region Consts
        private const string INSPECT_ITEM = "Inspect Item";
        #endregion Consts

        #region Fields
        [MenuInput]
        private Inventory _inventory;

        [MenuInput]
        private string _item;

        // Visual Elements
        [SerializeField] private Button _inspectItemButton;
        [SerializeField] private Button _discardItemButton;
        #endregion Fields

        #region Methods
        private void Start()
        {
            if (_inventory == null)
            {
                // Warning message
                return;
            }

            _discardItemButton.onClick.AddListener(OnDiscardItemButton);
            _inspectItemButton.onClick.AddListener(OnInspectItemButtonClicked);
        }
        #endregion Methods

        #region Callbacks
        private void OnInspectItemButtonClicked()
        {
            OpenNextMenu(INSPECT_ITEM);
        }

        private void OnDiscardItemButton()
        {
            if (_inventory.TryDiscardItem(_item))
            {
                ExitMenu();
            }
        }
        #endregion Callbacks

    }
}