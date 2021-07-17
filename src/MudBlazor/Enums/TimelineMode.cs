using System.ComponentModel;

namespace MudBlazor
{
    public enum TimelineMode
    {
        [Description("default")]
        Default,
        [Description("reverse")]
        Reverse,
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
