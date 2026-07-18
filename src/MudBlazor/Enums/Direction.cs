using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor
{
    /// <summary>
    /// Specifies a direction such as top, bottom, left, or right, used to orient or position components like drawers, menus, and swipe areas.
    /// </summary>
    [EnumExtensions]
    public enum Direction
    {
        [Description("bottom")]
        Bottom,
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
