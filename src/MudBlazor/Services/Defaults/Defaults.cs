//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

namespace MudBlazor
{
    public static class Defaults
    {
        public static Color Color { get; set; }
        public static bool Dense { get; set; }
        public static Margin Margin { get; set; }
        public static Origin AnchorOrigin { get; set; }
        public static Origin TransformOrigin { get; set; }
        public static Size Size { get; set; }
        public static Variant Variant { get; set; }
        public static PickerVariant PickerVariant { get; set; }

        public static class Classes
        {
            public static class Position
            {
                public const string TopLeft = "mud-snackbar-location-top-left";
                public const string TopCenter = "mud-snackbar-location-top-center";
                public const string TopRight = "mud-snackbar-location-top-right";
                public const string TopStart = "mud-snackbar-location-top-start";
                public const string TopEnd = "mud-snackbar-location-top-end";

                public const string BottomLeft = "mud-snackbar-location-bottom-left";
                public const string BottomCenter = "mud-snackbar-location-bottom-center";
                public const string BottomRight = "mud-snackbar-location-bottom-right";
                public const string BottomStart = "mud-snackbar-location-bottom-start";
                public const string BottomEnd = "mud-snackbar-location-bottom-end";
            }
        }
    }
}
