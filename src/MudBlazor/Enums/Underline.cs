using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Indicates how a <see cref="MudLink"/> is decorated.
/// </summary>
public enum Underline
{
    /// <summary>
    /// No underline is displayed.
    /// </summary>
    [Description("none")]
    None,

    /// <summary>
    /// An underline is displayed when hovering over the link.
    /// </summary>
    [Description("hover")]
    Hover,

    /// <summary>
    /// An underline is always displayed.
    /// </summary>
    [Description("always")]
    Always
}
