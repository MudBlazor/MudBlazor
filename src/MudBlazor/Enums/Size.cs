using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor;

/// <summary>
/// Indicates the size of a component.
/// </summary>
[EnumExtensions]
public enum Size
{
    /// <summary>
    /// The smallest size.
    /// </summary>
    [Description("small")]
    Small,

    /// <summary>
    /// A medium size.
    /// </summary>
    [Description("medium")]
    Medium,

    /// <summary>
    /// The largest size.
    /// </summary>
    [Description("large")]
    Large,
}
