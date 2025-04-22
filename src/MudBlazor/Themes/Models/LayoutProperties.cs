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
        /// Gets or sets the base height of the appbar. <br/>
        /// The actual height of the appbar is relative to this value, and it also depends on the current <see cref="Breakpoint"/>, or <see cref="MudAppBar.Dense"/>.
        /// </summary>
        public string AppBarBaseHeight { get; set; } = "64px";
    }
}
