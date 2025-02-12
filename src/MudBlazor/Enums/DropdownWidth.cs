using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Indicates the behavior of a <see cref="MudPopover"/> drop down relative width.
/// </summary>
public enum DropdownWidth
{
    /// <summary>
    /// The dropdown will open at the max width of it's activator parent preventing it from growing beyond that width.
    /// </summary>
    [Description("relative")]
    Relative,

    /// <summary>
    /// The dropdown will open at the min width of it's activator parent, preventing it from being less wide but allowing it to grow.
    /// </summary>
    [Description("adaptive")]
    Adaptive,

    /// <summary>
    /// The dropdown width will be ignored.
    /// </summary>
    [Description("ignore")]
    Ignore,
}
