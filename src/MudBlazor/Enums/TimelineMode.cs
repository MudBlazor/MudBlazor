using System.ComponentModel;

namespace MudBlazor
{
    public enum TimelineMode
    {
        [Description("vertical")]
        Vertical,
        [Description("horizontal")]
        Horizontal,
        [Description("top")]
        Top,
        [Description("bottom")]
        Bottom,
        [Description("left")]
        Left,
        [Description("right")]
        Right,
        [Description("start")]
        Start,
        [Description("end")]
        End
    }
}
