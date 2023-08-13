using System;

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
        /// [Obsolete] This property has been removed. Use at your own risk.
        /// </summary>
        [Obsolete("DrawerWidth has been removed.", true)]
        public string? DrawerWidth { get; set; } = null;

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
        /// Gets or sets the height of the drawer at the top.
        /// </summary>
        [Obsolete("DrawerHeightTop is not used.", true)]
        public string DrawerHeightTop { get; set; } = "320px";

        /// <summary>
        /// Gets or sets the height of the drawer at the bottom.
        /// </summary>
        [Obsolete("DrawerHeightBottom is not used.", true)]
        public string DrawerHeightBottom { get; set; } = "320px";

        /// <summary>
        /// [Obsolete] This property has been removed. Use at your own risk.
        /// </summary>
        [Obsolete("AppbarMinHeight has been removed.", true)]
        public string? AppbarMinHeight { get; set; } = null;

        /// <summary>
        /// Gets or sets the height of the appbar.
        /// </summary>
        public string AppbarHeight { get; set; } = "64px";
    }
}
