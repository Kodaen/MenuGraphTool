namespace MenuGraphTool
{
    public class CharacterMenu : MenuPage
    {
        const string ATTACK = "Attack";
        const string ITEMS = "Item";
        const string TRADE = "Trade";

        [MenuInput]
        private Character _inCharacter;

        [MenuOutput(ATTACK), MenuOutput(ITEMS), MenuOutput(TRADE)]
        private Character _character;
    }
}