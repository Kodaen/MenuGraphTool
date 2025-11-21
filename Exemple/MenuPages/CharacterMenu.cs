using System;

namespace MenuGraphTool
{
    public class CharacterMenu : MenuPage
    {
        [MenuInput]
        private Character Character;

        [MenuOutput(displayName: "Attack", "Character")]
        private Action<Character> _attack = null;
        public event Action<Character> Attack
        {
            add
            {
                _attack -= value;
                _attack += value;
            }
            remove
            {
                _attack -= value;
            }
        }

        [MenuOutput(displayName: "Items", "Character")]
        private Action<Character> _items = null;
        public event Action<Character> Item
        {
            add
            {
                _items -= value;
                _items += value;
            }
            remove
            {
                _items -= value;
            }
        }

        [MenuOutput(displayName: "Trade", "Character")]
        private Action<Character> _trade = null;
        public event Action<Character> Trade
        {
            add
            {
                _trade -= value;
                _trade += value;
            }
            remove
            {
                _trade -= value;
            }
        }
    }
}