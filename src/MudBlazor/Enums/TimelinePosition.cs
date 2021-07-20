using System.ComponentModel;

namespace MudBlazor
{
    public enum TimelinePosition
    {
        [Description("alternate")]
        Alternate,
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
