// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components;

namespace MudBlazor;

/// <summary>
/// Represents a column in a <see cref="MudDataGrid{T}"/> which can be expanded to show additional information.
/// </summary>
/// <typeparam name="T">The kind of item managed by the column.</typeparam>
public partial class HierarchyColumn<T> : MudComponentBase
{
    private bool _finishedInitialExpanded;

    /// <summary>
    /// The icon to display for the close button.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Filled.ChevronRight"/>.
    /// </remarks>
    [Parameter]
    public string ClosedIcon { get; set; } = Icons.Material.Filled.ChevronRight;

    /// <summary>
    /// The icon to display for the open button.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Filled.ExpandMore"/>.
    /// </remarks>
    [Parameter]
    public string OpenIcon { get; set; } = Icons.Material.Filled.ExpandMore;

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
    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (firstRender)
        {
            _finishedInitialExpanded = true;
        }
    }
}
