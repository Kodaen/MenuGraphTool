namespace MenuGraphTool
{
    public class CharacterMenu : MenuPage
    {
        const string ATTACK = "Attack";
        const string ITEMS = "Item";
        const string TRADE = "Trade";

        [MenuInput]
        private Character Character;

        [MenuOutput(ATTACK)]
        private Character _character_attack;

        [MenuOutput(ITEMS)]
        private Character _character_items;

        [MenuOutput(TRADE)]
        private Character _character_trade;
    }
}