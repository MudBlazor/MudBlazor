
using System;

namespace MudBlazor
{
    public class LayoutProperties
    {
        public string DefaultBorderRadius { get; set; } = "4px";
        [Obsolete]
        public string DrawerWidth { get; set; } = null;
        public string DrawerWidthLeft { get; set; } = "240px";
        public string DrawerWidthRight { get; set; } = "240px";
        public string DrawerHeightTop { get; set; } = "320px";
        public string DrawerHeightBottom { get; set; } = "320px";
        public string AppbarMinHeight { get; set; } = "64px";
    }
}
