using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor
{
    /// <summary>
    /// The horizontal alignment of text or inline content.
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
