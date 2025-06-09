namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents the layout properties for a user interface.
    /// </summary>
    public class LayoutProperties
    {
        /// <summary>
        /// Default border radius.
        /// </summary>
        public string DefaultBorderRadius { get; set; } = "4px";

        /// <summary>
        /// Width of the mini drawer on the left side.
        /// </summary>
        public string DrawerMiniWidthLeft { get; set; } = "56px";

        /// <summary>
        /// Width of the mini drawer on the right side.
        /// </summary>
        public string DrawerMiniWidthRight { get; set; } = "56px";

        /// <summary>
        /// Width of the drawer on the left side.
        /// </summary>
        public string DrawerWidthLeft { get; set; } = "240px";

        /// <summary>
        /// Width of the drawer on the right side.
        /// </summary>
        public string DrawerWidthRight { get; set; } = "240px";

        /// <summary>
        /// Height of the appbar.
        /// </summary>
        public string AppbarHeight { get; set; } = "64px";
    }
}
