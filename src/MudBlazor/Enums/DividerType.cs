using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// The type of <see cref="MudDivider"/> to display.
/// </summary>
public enum DividerType
{
    /// <summary>
    /// The divider will fill the width of its container.
    /// </summary>
    [Description("fullwidth")]
    FullWidth,

    /// <summary>
    /// The divider has a margin on its left side.
    /// </summary>
    [Description("inset")]
    Inset,

    /// <summary>
    /// The divider has a margin on the left and right sides.
    /// </summary>
    [Description("middle")]
    Middle
}
