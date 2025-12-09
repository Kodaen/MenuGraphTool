namespace MenuGraphTool
{
    public enum MenuOpeningMode
    {
        /// <summary>
        /// Opens menu and disables parent menu. 
        /// Useful for fullscreen menus.
        /// </summary>
        Replace,
        /// <summary>
        /// Opens the menu without closing its parent.
        /// Useful for popups.
        /// </summary>
        Add
    }
}