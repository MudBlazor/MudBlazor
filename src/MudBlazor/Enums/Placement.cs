using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor
{
    /// <summary>
    /// Specifies where an element such as a tooltip or an input's label is placed relative to its anchor, such as top, bottom, left, right, start, or end.
    /// </summary>
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
