using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// The position of a component relative to its container.
/// </summary>
public enum HorizontalAlignment
{
    /// <summary>
    /// The component is horizontally centered.
    /// </summary>
    [Description("center")]
    Center,

    /// <summary>
    /// The component is aligned to the left.
    /// </summary>
    [Description("left")]
    Left,

    /// <summary>
    /// The component is aligned to the right.
    /// </summary>
    [Description("right")]
    Right,

    /// <summary>
    /// The component is aligned based on Right-to-Left (RTL) settings.
    /// </summary>
    /// <remarks>
    /// When Right-to-Left is enabled, the component is aligned to the right.  Otherwise, the left.
    /// </remarks>
    [Description("start")]
    Start,

    /// <summary>
    /// The component is aligned based on Right-to-Left (RTL) settings.
    /// </summary>
    /// <remarks>
    /// When Right-to-Left is enabled, the component is aligned to the left.  Otherwise, the right.
    /// </remarks>
    [Description("end")]
    End
}
