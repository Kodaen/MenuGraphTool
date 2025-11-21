using System;

namespace MenuGraphTool
{
    public class TradeSelectionMenu : MenuPage
    {
        [MenuInput]
        private Character Character;


        [MenuOutput(displayName: "TargetSeleted", "Character1", "Character2")]
        private Action<Character, Character> _onCharacterSelected = null;
        public event Action<Character, Character> OnCharacterSelected
        {
            add
            {
                _onCharacterSelected -= value;
                _onCharacterSelected += value;
            }
            remove
            {
                _onCharacterSelected -= value;
            }
        }
    }
}