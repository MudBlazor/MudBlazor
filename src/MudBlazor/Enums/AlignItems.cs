using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// The vertical alignment applied to items in a <see cref="MudStack"/> or <see cref="MudDataGrid{T}"/>.
/// </summary>
public enum AlignItems
{
    /// <summary>
    /// Items are aligned to keep text consistently aligned.
    /// </summary>
    [Description("baseline")]
    Baseline,

    /// <summary>
    /// The center of items is aligned to the center of the container.
    /// </summary>
    [Description("center")]
    Center,

    /// <summary>
    /// The top edge of items are aligned to the top of the container.
    /// </summary>
    [Description("start")]
    Start,

    /// <summary>
    /// The bottom edge of items are aligned to the bottom of the container.
    /// </summary>
    [Description("end")]
    End,

    /// <summary>
    /// Items will have the same height as the container.
    /// </summary>
    [Description("stretch")]
    Stretch
}
