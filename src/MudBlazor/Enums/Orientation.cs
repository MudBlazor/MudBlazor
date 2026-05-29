using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor;

/// <summary>
/// Indicates the display orientation of a <see cref="MudPicker{T}"/>.
/// </summary>
[EnumExtensions]
public enum Orientation
{
    /// <summary>
    /// The picker is taller than it is wide, with a title on top.
    /// </summary>
    [Description("portrait")]
    Portrait,

    /// <summary>
    /// The picker is wider than it is tall, with a title on the left.
    /// </summary>
    [Description("landscape")]
    Landscape,
}
