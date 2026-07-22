using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Specifies how children of a flex container are aligned along the cross axis.
/// </summary>
public enum AlignItems
{
    [Description("baseline")]
    Baseline,
    [Description("center")]
    Center,
    [Description("start")]
    Start,
    [Description("end")]
    End,
    [Description("stretch")]
    Stretch
}
