using System.ComponentModel;

namespace MudBlazor
{
    public enum Placement
    {
        [Description("left")]
        Left,
        [Description("right")]
        Right,
        [Description("end")]
        End,
        [Description("start")]
        Start,
        [Description("top")]
        Top,
        [Description("bottom")]
        Bottom
    }
}
