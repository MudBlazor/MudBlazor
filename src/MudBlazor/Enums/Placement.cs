using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor
{
    [EnumExtensions]
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
