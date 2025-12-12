namespace MenuGraphTool
{
    public class ItemSelectionMenu : MenuPage
    {
        const string SELECTED_ITEM = "SelectedItem";

        [MenuInput]
        private Character Character;

        [MenuOutput(SELECTED_ITEM)]
        private int _itemId;
    }
}