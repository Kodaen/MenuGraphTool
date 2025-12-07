using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MenuGraphTool
{
    [Serializable]
    public class BackActionReference 
    {
        public BackActionType BackInputAction;
        public InputAction InputAction;
    }

    public enum BackActionType {
        /// <summary>
        /// The default back input action will be used, this menu can be closed with the back key
        /// </summary>
        Default,
        /// <summary>
        /// The selected back input action will be used, this menu can be closed with the selected back input action
        /// </summary>
        Override,
        /// <summary>
        /// The back input action is disabled, this menu cannot be closed with the back action
        /// </summary>
        None
    }
}