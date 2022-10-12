
using System;

namespace MudBlazor
{
    public class LayoutProperties
    {
        public string DefaultBorderRadius { get; set; } = "4px";
        [Obsolete("DrawerWidth has been removed.", true)]
        public string DrawerWidth { get; set; } = null;
        public string DrawerMiniWidthLeft { get; set; } = "56px";
        public string DrawerMiniWidthRight { get; set; } = "56px";
        public string DrawerWidthLeft { get; set; } = "240px";
        public string DrawerWidthRight { get; set; } = "240px";
        public string DrawerHeightTop { get; set; } = "320px";
        public string DrawerHeightBottom { get; set; } = "320px";
        [Obsolete("AppbarMinHeight has been removed.", true)]
        public string AppbarMinHeight { get; set; } = null;
        public string AppbarHeight { get; set; } = "64px";
    }
}
