using System;

namespace MenuGraphTool
{
    public class UseItemMenu : MenuPage
    {
        [MenuInput]
        private int ItemID;


        [MenuOutput(displayName: "Trade", "Character")]
        private Action<Character> _onStartTrade = null;
        public event Action<Character> OnStartTrade
        {
            add
            {
                _onStartTrade -= value;
                _onStartTrade += value;
            }
            remove
            {
                _onStartTrade -= value;
            }
        }
    }
}