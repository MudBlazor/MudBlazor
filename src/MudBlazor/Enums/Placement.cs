using System.ComponentModel;

namespace MudBlazor
{
    public enum Placement
    {
        [Description("left-start")]
        LeftStart,
        [Description("left")]
        Left,
        [Description("left-end")]
        LeftEnd,
        [Description("right-start")]
        RightStart,
        [Description("right")]
        Right,
        [Description("right-end")]
        RightEnd,
        [Description("top-start")]
        TopStart,
        [Description("top")]
        Top,
        [Description("top-end")]
        TopEnd,
        [Description("bottom-start")]
        BottomStart,
        [Description("bottom")]
        Bottom,
        [Description("bottom-end")]
        BottomEnd,
        [Description("center")]
        Center
    }
}
