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
        /// <remarks>
        /// It is highly recommanded to add a canvas group to your prefab in order to 
        /// disable navigation input in the parent Menu. See <see cref="MenuPage.SetCanvasGroupInteractible"/>.
        /// </remarks>
        Add
    }
}