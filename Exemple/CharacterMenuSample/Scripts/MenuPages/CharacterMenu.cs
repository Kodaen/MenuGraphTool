using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MenuGraphTool
{
    public class CharacterMenu : MenuPage
    {
        #region Consts
        const string CHARACTER_SHEET = "CharacterSheet";
        const string INVENTORY = "Inventory";
        #endregion Consts

        #region Fields
        [MenuInput, MenuOutput(CHARACTER_SHEET)]
        private Character _character;

        [MenuOutput(INVENTORY)]
        private Inventory _inventory;

        // Visual Elements
        [SerializeField] private TextMeshProUGUI _characterNameLabel;
        [SerializeField] private Button _characterSheetButton;
        [SerializeField] private Button _inventoryButton;
        #endregion Fields

        #region Methods
        private void Start()
        {
            if (_character == null)
            {
                return;
            }
            
            _characterNameLabel.text = _character.Name;
        }

        private void OnEnable()
        {
            _characterSheetButton.onClick.AddListener(OnCharacterSheetButtonClicked);
            _inventoryButton.onClick.AddListener(OnInventoryButtonClicked);
        }

        private void OnDisable()
        {
            _characterSheetButton.onClick.RemoveListener(OnCharacterSheetButtonClicked);
            _inventoryButton.onClick.RemoveListener(OnInventoryButtonClicked);
        }
        #endregion Methods

        private void OnCharacterSheetButtonClicked()
        {
            OpenNextMenu(CHARACTER_SHEET);
        }

        private void OnInventoryButtonClicked()
        {
            _inventory = _character.Inventory;
            OpenNextMenu(INVENTORY);
        }
    }
}