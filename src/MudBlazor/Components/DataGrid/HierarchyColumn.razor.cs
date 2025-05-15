// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace MudBlazor;

/// <summary>
/// Represents a column in a <see cref="MudDataGrid{T}"/> which can be expanded to show additional information.
/// </summary>
/// <typeparam name="T">The kind of item managed by the column.</typeparam>
/// <seealso cref="Column{T}"/>
/// <seealso cref="MudDataGrid{T}"/>
public partial class HierarchyColumn<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : MudComponentBase
{
    private bool _finishedInitialExpanded;
    private readonly HashSet<CellContext<T>> _initiallyExpandedItems = [];

    /// <summary>
    /// Whether the display should be right to left
    /// </summary>
    [CascadingParameter(Name = "RightToLeft")]
    public bool RightToLeft { get; set; }

    /// <summary>
    /// The icon to display for the close button.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Filled.ChevronRight"/> or <see cref="Icons.Material.Filled.ChevronLeft"/> if RightToLeft.
    /// </remarks>
    [Parameter]
    public string ClosedIcon { get; set; }

    /// <summary>
    /// The icon to display for the open button.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Filled.ExpandMore"/>.
    /// </remarks>
    [Parameter]
    public string OpenIcon { get; set; }

    /// <summary>
    /// The size of the open and close icons.
    /// </summary>
    [Parameter]
    public Size IconSize { get; set; } = Size.Medium;

    /// <summary>
    /// The function which determines whether buttons are disabled.
    /// </summary>
    [Parameter]
    public Func<T, bool> ButtonDisabledFunc { get; set; } = _ => false;

    /// <summary>
    /// Allows this column to be reordered via drag-and-drop operations.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>. When set, this overrides the <see cref="MudDataGrid{T}.DragDropColumnReordering"/> property.
    /// </remarks>
    [Parameter]
    public bool? DragAndDropEnabled { get; set; } = false;

    /// <summary>
    /// Allows this column to be hidden.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  When set, this overrides the <see cref="MudDataGrid{T}.Hideable"/> property.
    /// </remarks>
    [Parameter]
    public bool? Hideable { get; set; }

    /// <summary>
    /// Hides this column.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    public bool Hidden { get; set; }

    /// <summary>
    /// Occurs when the <see cref="Hidden"/> property has changed.
    /// </summary>
    [Parameter]
    public EventCallback<bool> HiddenChanged { get; set; }

    /// <summary>
    /// Whether or not to show a button in the header to expand/collapse all columns.
    /// </summary>
    /// <remarks>Defaults to <c>false</c>.</remarks>
    [Parameter]
    public bool EnableHeaderToggle { get; set; } = false;

    /// <summary>
    /// The CSS class applied to the header.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  Separate multiple classes with spaces.
    /// </remarks>
    [Parameter]
    public string HeaderClass { get; set; }

    /// <summary>
    /// The function which calculates CSS classes for the header.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  Separate multiple classes with spaces.
    /// </remarks>
    [Parameter]
    public Func<IEnumerable<T>, string> HeaderClassFunc { get; set; }

    /// <summary>
    /// The CSS style applied to this column's header.
    /// </summary>
    [Parameter]
    public string HeaderStyle { get; set; } = "width:0%;";

    /// <summary>
    /// The function which calculates CSS styles for the header.
    /// </summary>
    [Parameter]
    public Func<IEnumerable<T>, string> HeaderStyleFunc { get; set; }

    /// <summary>
    /// The template used to display this column's header.
    /// </summary>
    [Parameter]
    public RenderFragment<HeaderContext<T>> HeaderTemplate { get; set; }

    /// <summary>
    /// The template used to display this column's value cells.
    /// </summary>
    [Parameter]
    public RenderFragment<CellContext<T>> CellTemplate { get; set; }

    /// <summary>
    /// The function which determines whether the row should be initially expanded.
    /// </summary>
    /// <remarks>
    /// This function takes an item of type <typeparamref name="T"/> as input and returns a boolean indicating
    /// whether the row should be expanded.
    /// Defaults to a function that always returns <c>false</c>.
    /// </remarks>
    [Parameter]
    public Func<T, bool> InitiallyExpandedFunc { get; set; } = _ => false;

    /// <inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            _finishedInitialExpanded = true;

            foreach (var context in _initiallyExpandedItems)
            {
                await context.Actions.ToggleHierarchyVisibilityForItemAsync.Invoke();
            }
        }
    }

    private string GetGroupIcon(CellContext<T> context)
    {
        var isItemOpen = context.OpenHierarchies.Contains(context.Item);
        var isOpenIconEmpty = string.IsNullOrEmpty(OpenIcon);
        var isClosedIconEmpty = string.IsNullOrEmpty(ClosedIcon);
        var isGetGroupDefined = context.Actions.GetGroupIcon != null;

        if (isItemOpen)
        {
            return isOpenIconEmpty && isGetGroupDefined
                ? context.Actions.GetGroupIcon(true, RightToLeft)
                : OpenIcon;
        }
        else
        {
            return isClosedIconEmpty && isGetGroupDefined
                ? context.Actions.GetGroupIcon(false, RightToLeft)
                : ClosedIcon;
        }
    }
}
