using UnityEngine;
using UnityEngine.UI;

namespace MenuGraphTool
{
    [MenuOutput(AddOrReplaceMenu.ADD_MENU)]
    [MenuOutput(AddOrReplaceMenu.REPLACE_MENU)]
    public partial class AddOrReplaceMenu : MenuPage
    {
        #region Fields
        [SerializeField] private Button _add;
        [SerializeField] private Button _replace;

        #region Menu Flow
        private const string ADD_MENU = "Add Menu";
        private const string REPLACE_MENU = "Replace Menu";
        #endregion Menu Flow
        #endregion Fields

        private void OnEnable()
        {
            _add.onClick.AddListener(OnAddButtonClicked);
            _replace.onClick.AddListener(OnReplaceButtonClicked);
        }

        private void OnDisable()
        {
            _add.onClick.RemoveListener(OnAddButtonClicked);
            _replace.onClick.RemoveListener(OnReplaceButtonClicked);
        }

        #region Callbacks
        private void OnAddButtonClicked()
        {
            OpenNextMenu(ADD_MENU);
        }
        private void OnReplaceButtonClicked()
        {
            OpenNextMenu(REPLACE_MENU);
        }
        #endregion Callbacks
    }
}