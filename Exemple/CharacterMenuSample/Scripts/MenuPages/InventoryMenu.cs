using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MenuGraphTool
{
    public class InventoryMenu : MenuPage
    {
        #region Consts
        private const string ITEM_ACTION = "Item Action";
        #endregion Consts

        #region Fields
        [MenuInput]
        private Inventory _inventory;

        [MenuOutput(ITEM_ACTION)]
        private string _selectedItem;

        // Visual Elements
        [SerializeField] private Button _itemSlotPrefab;
        [SerializeField] private GameObject _itemListSlot;

        private SelectableRedirector _selectableRedirector;
        #endregion Fields

        #region Methods
        private void Start()
        {
            if (_inventory == null)
            {
                // Warning message
                return;
            }

            if (_inventory.InventoryItems == null)
            {
                // Warning message
                return;
            }

            _selectableRedirector = GetComponent<SelectableRedirector>();

            CreateItemList();
            _selectableRedirector.OnSelect(null);

            _inventory.OnInventoryChanged += CreateItemList;
        }

        private void CreateItemList()
        {
            for (int i = _itemListSlot.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(_itemListSlot.transform.GetChild(i).gameObject);
            }

            _selectableRedirector.RedirectionReceiver = null;

            foreach (string item in _inventory.InventoryItems)
            {
                Button prefab = Instantiate(_itemSlotPrefab, _itemListSlot.transform);
                prefab.GetComponentInChildren<TextMeshProUGUI>().text = item;

                prefab.onClick.AddListener(() =>
                {
                    OnItemSlotClicked(item);
                });

                if (_selectableRedirector.RedirectionReceiver == null)
                {
                    _selectableRedirector.RedirectionReceiver = prefab;

                }
            }
        }

        public override void OnMenuFocused()
        {
            base.OnMenuFocused();
            if (_selectableRedirector != null)
            {
                _selectableRedirector.OnSelect(null);
            }
        }

        private void OnEnable()
        {
            if (_inventory != null)
            {
                _inventory.OnInventoryChanged += CreateItemList;
            }
        }

        private void OnDisable()
        {
            if (_inventory != null)
            {
                _inventory.OnInventoryChanged -= CreateItemList;
            }
        }
        #endregion Methods

        private void OnItemSlotClicked(string item)
        {
            _selectedItem = item;
            OpenNextMenu(ITEM_ACTION);
        }
    }
}