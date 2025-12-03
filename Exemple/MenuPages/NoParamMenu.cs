using UnityEngine;
using UnityEngine.UI;

namespace MenuGraphTool
{
    public partial class NoParamMenu : MenuPage
    {
        #region Fields
        [SerializeField] private Button _nextMenuButton;

        #region Menu Flow
        private const string NEXT_MENU = "Next Menu";

        [MenuOutput(NEXT_MENU)]
        private Character _chara;

        [MenuOutput(NEXT_MENU)]
        private int _itemId;
        #endregion Menu Flow
        #endregion Fields

        private void OnEnable()
        {
            _nextMenuButton.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _nextMenuButton.onClick.RemoveListener(OnButtonClicked);
        }

        #region Callbacks
        private void OnButtonClicked()
        {
            // TEMP
            _chara = new() { Name = "bonjour" };
            OpenNextMenu(NEXT_MENU);
        } 
        #endregion Callbacks

    }
}