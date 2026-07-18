using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor
{
    /// <summary>
    /// Specifies where an element is positioned relative to its container, such as top, bottom, left, right, or center.
    /// </summary>
    [EnumExtensions]
    public enum Position
    {
        [Description("bottom")]
        Bottom,
        [Description("center")]
        Center,
        [Description("top")]
        Top,
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
