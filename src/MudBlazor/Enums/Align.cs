using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor
{
    /// <summary>
    /// Specifies the horizontal alignment of text or inline content, such as inherit, left, center, right, justify, start, or end.
    /// </summary>
    [EnumExtensions]
    public enum Align
    {
        [Description("inherit")]
        Inherit,
        [Description("left")]
        Left,
        [Description("center")]
        Center,
        [Description("right")]
        Right,
        [Description("justify")]
        Justify,
        [Description("start")]
        Start,
        [Description("end")]
        End,
    }
}
