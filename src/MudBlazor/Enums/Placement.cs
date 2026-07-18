using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor
{
    /// <summary>
    /// The position of an element relative to its anchor.
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
