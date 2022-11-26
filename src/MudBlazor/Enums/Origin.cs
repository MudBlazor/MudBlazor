using System.ComponentModel;

namespace MudBlazor
{
    public enum Origin
    {
        [Description("top-end")]
        TopEnd,
        [Description("center-end")]
        CenterEnd,
        [Description("bottom-end")]
        BottomEnd,
        [Description("top-start")]
        TopStart,
        [Description("center-start")]
        CenterStart,
        [Description("bottom-start")]
        BottomStart,
        [Description("top-left")]
        TopLeft,
        [Description("top-center")]
        TopCenter,
        [Description("top-right")]
        TopRight,
        [Description("center-left")]
        CenterLeft,
        [Description("center-center")]
        CenterCenter,
        [Description("center-right")]
        CenterRight,
        [Description("bottom-left")]
        BottomLeft,
        [Description("bottom-center")]
        BottomCenter,
        [Description("bottom-right")]
        BottomRight,
    }
}
