using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor;

/// <summary>
/// Specifies where an adornment such as an icon, text, or button appears within a form field.
/// </summary>
/// <remarks>
/// Adornments can be placed at the start or end of a field, or not displayed at all.
/// </remarks>
[EnumExtensions]
public enum Adornment
{
    /// <summary>
    /// No adornment is displayed.
    /// </summary>
    [Description("none")]
    None,

    /// <summary>
    /// The adornment is placed at the start of the field.
    /// </summary>
    [Description("start")]
    Start,

    /// <summary>
    /// The adornment is placed at the end of the field.
    /// </summary>
    [Description("end")]
    End,
}
