// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor;

/// <summary>
/// Represents a checkbox column used to select rows in a <see cref="MudDataGrid{T}"/>.
/// </summary>
/// <typeparam name="T">The type of item to select.</typeparam>
public partial class SelectColumn<T>
{
    /// <summary>
    /// Shows a checkbox in the header.
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, all rows can be checked by selecting this checkbox.
    /// </remarks>
    [Parameter]
    public bool ShowInHeader { get; set; } = true;

    /// <summary>
    /// Shows a checkbox in the footer.
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, all rows can be checked by selecting this checkbox.
    /// </remarks>
    [Parameter]
    public bool ShowInFooter { get; set; } = false;

    /// <summary>
    /// The size of the checkbox icon.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Size.Medium"/>.
    /// </remarks>
    [Parameter]
    public Size Size { get; set; } = Size.Medium;

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
}
