using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Indicates the amount of spacing to apply.
/// </summary>
public enum Margin
{
    /// <summary>
    /// No spacing is applied.
    /// </summary>
    [Description("none")]
    None,

    /// <summary>
    /// Compact spacing is applied.
    /// </summary>
    [Description("dense")]
    Dense,

    /// <summary>
    /// Normal spacing is applied.
    /// </summary>
    [Description("normal")]
    Normal,
}
