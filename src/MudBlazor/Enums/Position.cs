using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor
{
    /// <summary>
    /// The position of an element relative to its container.
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
