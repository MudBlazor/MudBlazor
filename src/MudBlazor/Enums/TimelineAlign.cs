using System.ComponentModel;
using NetEscapades.EnumGenerators;

namespace MudBlazor;

/// <summary>
/// Specifies whether each item's dot aligns to the start, center, or end of its text in a <see cref="MudTimeline"/>.
/// </summary>
[EnumExtensions]
public enum TimelineAlign
{
    /// <summary>
    /// The dot is centered relative to its text.
    /// </summary>
    [Description("default")]
    Default,

    /// <summary>
    /// The dot is aligned with the start of the text.
    /// </summary>
    [Description("start")]
    Start,

    /// <summary>
    /// The dot is aligned with the end of the text.
    /// </summary>
    [Description("end")]
    End
}
