using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Indicates the position or direction a component is displayed relative to its parent container.
/// </summary>
public enum Origin
{
    /// <summary>
    /// The component's top-left corner is at the container's top-left corner.
    /// </summary>
    /// <remarks>
    /// For transform origins, the component displays down and to the right of the anchor origin.
    /// </remarks>
    [Description("top-left")]
    TopLeft,

    /// <summary>
    /// The component is centered on the container's top center.
    /// </summary>
    /// <remarks>
    /// For transform origins, the component displays below the anchor origin.
    /// </remarks>
    [Description("top-center")]
    TopCenter,

    /// <summary>
    /// The component's top-left corner is at the container's top-right corner.
    /// </summary>
    /// <remarks>
    /// For transform origins, the component displays down and to the left of the anchor origin.
    /// </remarks>
    [Description("top-right")]
    TopRight,

    /// <summary>
    /// The component's top-left corner is at the left of the container, and its center vertically.
    /// </summary>
    /// <remarks>
    /// For transform origins, the component displays to the right of the anchor origin.
    /// </remarks>
    [Description("center-left")]
    CenterLeft,

    /// <summary>
    /// The component's top-left corner is at the center of the container.
    /// </summary>
    /// <remarks>
    /// For transform origins, the component is centered over the anchor origin.
    /// </remarks>
    [Description("center-center")]
    CenterCenter,

    /// <summary>
    /// The component's top-left corner is at the right of the container, and its center vertically.
    /// </summary>
    /// <remarks>
    /// For transform origins, the component displays to the left of the anchor origin.
    /// </remarks>
    [Description("center-right")]
    CenterRight,

    /// <summary>
    /// The component's top-left corner is at the container's bottom-left corner.
    /// </summary>
    /// <remarks>
    /// For transform origins, the component displays up and to the right of the anchor origin.
    /// </remarks>
    [Description("bottom-left")]
    BottomLeft,

    /// <summary>
    /// The component is centered on the container's bottom center.
    /// </summary>
    /// <remarks>
    /// For transform origins, the component is centered above the anchor origin.
    /// </remarks>
    [Description("bottom-center")]
    BottomCenter,

    /// <summary>
    /// The component's top-left corner is at the container's bottom-right corner.
    /// </summary>
    /// <remarks>
    /// For transform origins, the component displays up and to the left of the anchor origin.
    /// </remarks>
    [Description("bottom-right")]
    BottomRight,
}
