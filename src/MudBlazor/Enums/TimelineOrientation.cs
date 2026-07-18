using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor;

/// <summary>
/// Specifies whether items in a <see cref="MudTimeline"/> are arranged vertically or horizontally.
/// </summary>
/// <seealso cref="MudTimeline" />
/// <seealso cref="MudTimelineItem" />
/// <seealso cref="TimelineAlign" />
[EnumExtensions]
public enum TimelineOrientation
{
    /// <summary>
    /// Items are displayed vertically.
    /// </summary>
    [Description("vertical")]
    Vertical,

    /// <summary>
    /// Items are displayed horizontally.
    /// </summary>
    [Description("horizontal")]
    Horizontal
}
