using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Specifies how items are drawn in a <see cref="MudTimeline" />.
/// </summary>
public enum TimelinePosition
{
    /// <summary>
    /// Items alternate on either side of centered dots.
    /// </summary>
    [Description("alternate")]
    Alternate,

    /// <summary>
    /// Dots are displayed above the text of each timeline item.
    /// </summary>
    /// <remarks>
    /// Only applies if <see cref="MudTimeline.TimelineOrientation"/> is <see cref="TimelineOrientation.Horizontal"/>.
    /// </remarks>
    [Description("top")]
    Top,

    /// <summary>
    /// Dots are displayed below the text of each timeline item.
    /// </summary>
    /// <remarks>
    /// Only applies if <see cref="MudTimeline.TimelineOrientation"/> is <see cref="TimelineOrientation.Vertical"/>.
    /// </remarks>
    [Description("bottom")]
    Bottom,

    /// <summary>
    /// Dots are displayed to the left of text for each timeline item.
    /// </summary>
    /// <remarks>
    /// Only applies if <see cref="MudTimeline.TimelineOrientation"/> is <see cref="TimelineOrientation.Vertical"/>.
    /// </remarks>
    [Description("left")]
    Left,

    /// <summary>
    /// Dots are displayed to the right of text for each timeline item.
    /// </summary>
    /// <remarks>
    /// Only applies if <see cref="MudTimeline.TimelineOrientation"/> is <see cref="TimelineOrientation.Vertical"/>.
    /// </remarks>
    [Description("right")]
    Right,

    /// <summary>
    /// Dots are displayed at the start based on the Right-to-Left setting of the <see cref="MudRTLProvider"/>.
    /// </summary>
    /// <remarks>
    /// Only applies if <see cref="MudTimeline.TimelineOrientation"/> is <see cref="TimelineOrientation.Vertical"/>.<br />
    /// When Right-to-Left is enabled, the dots are displayed on the right of each item's text.<br />
    /// When Right-to-Left is disabled, the dots are displayed on the left of each item's text.
    /// </remarks>
    [Description("start")]
    Start,

    /// <summary>
    /// Dots are displayed at the end based on the Right-to-Left setting of the <see cref="MudRTLProvider"/>.
    /// </summary>
    /// <remarks>
    /// Only applies if <see cref="MudTimeline.TimelineOrientation"/> is <see cref="TimelineOrientation.Vertical"/>.<br />
    /// When Right-to-Left is enabled, the dots are displayed on the left of each item's text.<br />
    /// When Right-to-Left is disabled, the dots are displayed on the right of each item's text.
    /// </remarks>
    [Description("end")]
    End
}
