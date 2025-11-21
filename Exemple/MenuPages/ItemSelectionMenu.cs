using System;

namespace MenuGraphTool
{
    public class ItemSelectionMenu : MenuPage
    {
        [MenuInput]
        private Character Character;

        [MenuOutput(displayName: "SelectedItem", "SelectedItemID")]
        private Action<int> _onItemSelected = null;
        public event Action<int> OnItemSelected
        {
            add
            {
                _onItemSelected -= value;
                _onItemSelected += value;
            }
            remove
            {
                _onItemSelected -= value;
            }
        }
    }
}