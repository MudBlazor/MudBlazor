namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents the layout properties for a user interface.
    /// </summary>
    public class LayoutProperties
    {
        /// <summary>
        /// Gets or sets the default border radius.
        /// </summary>
        public string DefaultBorderRadius { get; set; } = "4px";

        /// <summary>
        /// Gets or sets the width of the mini drawer on the left side.
        /// </summary>
        public string DrawerMiniWidthLeft { get; set; } = "56px";

        /// <summary>
        /// Gets or sets the width of the mini drawer on the right side.
        /// </summary>
        public string DrawerMiniWidthRight { get; set; } = "56px";

        /// <summary>
        /// Gets or sets the width of the drawer on the left side.
        /// </summary>
        public string DrawerWidthLeft { get; set; } = "240px";

        /// <summary>
        /// Gets or sets the width of the drawer on the right side.
        /// </summary>
        public string DrawerWidthRight { get; set; } = "240px";

        /// <summary>
        /// Gets or sets the height of the appbar.
        /// </summary>
        public string AppbarHeight { get; set; } = "64px";
    }
}
