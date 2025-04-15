using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Specifies the alignment of each item's dot relative to its text in a <see cref="MudTimeline"/>.
/// </summary>
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
