using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Indicates the edge of a conainter that a component will appear.
/// </summary>
public enum Anchor
{
    /// <summary>
    /// The component will display on the left edge.
    /// </summary>
    [Description("left")]
    Left,

    /// <summary>
    /// The component will display on the right edge.
    /// </summary>
    [Description("right")]
    Right,

    /// <summary>
    /// The component will display based on Right-to-Left (RTL) language settings.
    /// </summary>
    /// <remarks>
    /// When RTL is enabled, the component is displayed on the right edge, otherwise the left edge.
    /// </remarks>
    [Description("start")]
    Start,

    /// <summary>
    /// The component will display based on Right-to-Left (RTL) language settings.
    /// </summary>
    /// <remarks>
    /// When RTL is enabled, the component is displayed on the left edge, otherwise the right edge.
    /// </remarks>
    [Description("end")]
    End,

    /// <summary>
    /// The component will display on the top of the container.
    /// </summary>
    [Description("top")]
    Top,

    /// <summary>
    /// The component will display on the bottom of the container.
    /// </summary>
    [Description("bottom")]
    Bottom
}
