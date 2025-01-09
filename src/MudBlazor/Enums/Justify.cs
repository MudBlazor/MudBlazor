using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// The horizontal distribution of child items in a <see cref="MudStack"/> component.
/// </summary>
public enum Justify
{
    /// <summary>
    /// Items are aligned to the start of the <see cref="MudStack"/>.
    /// </summary>
    [Description("start")]
    FlexStart,

    /// <summary>
    /// Items are centered horizontally.
    /// </summary>
    [Description("center")]
    Center,

    /// <summary>
    /// Items are aligned to the end of the <see cref="MudStack"/>.
    /// </summary>
    [Description("end")]
    FlexEnd,

    /// <summary>
    /// Space is applied between each item, with items aligned against the start and end.
    /// </summary>
    [Description("space-between")]
    SpaceBetween,

    /// <summary>
    /// Space is applied between each item, with additional spacing for the first and last item.
    /// </summary>
    [Description("space-around")]
    SpaceAround,

    /// <summary>
    /// Space is applied evenly between each item, including the edges of the first and last item.
    /// </summary>
    [Description("space-evenly")]
    SpaceEvenly
}
