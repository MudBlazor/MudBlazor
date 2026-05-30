using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;


/// <summary>
/// A header cell which labels a column of data for a <see cref="MudTable{T}"/>.
/// </summary>
public partial class MudTh : MudComponentBase
{
    protected string Classname => new CssBuilder("mud-table-cell")
        .AddClass(Context?.Table?.CellClass)
        .AddClass(Class)
        .Build();

    /// <summary>
    /// The current state of the <see cref="MudTable{T}"/> containing this group.
    /// </summary>
    [CascadingParameter]
    public TableContext? Context { get; set; }

    /// <summary>
    /// The content within this header cell.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Occurs when the user triggers a contextmenu event (typically right-click) on this header cell.
    /// </summary>
    [Parameter]
    public EventCallback OnContextMenu { get; set; }

    /// <summary>
    /// Prevents the browser's default context menu from appearing when <see cref="OnContextMenu"/> is triggered.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    public bool OnContextMenuPreventDefault { get; set; }
}
