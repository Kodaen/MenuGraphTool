using System;
using UnityEngine;

namespace MenuGraphTool
{
    public class MenuPage : MonoBehaviour
    {
        protected Action<string> _onNextMenu = null;
        public virtual event Action<string> OnNextMenu
        {
            add
            {
                _onNextMenu -= value;
                _onNextMenu += value;
            }
            remove
            {
                _onNextMenu -= value;
            }
        }

        public virtual void OpenNextMenu(string ExecutionFlow)
        {
            _onNextMenu?.Invoke(ExecutionFlow);
        }

    }

    public class Character
    {
        public string Name;
    }
}