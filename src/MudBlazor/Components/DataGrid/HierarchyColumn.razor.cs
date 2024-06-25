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
    /// <summary>
    /// The icon shown when a row is collapsed.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Filled.ChevronRight"/>.
    /// </remarks>
    [Parameter] public string ClosedIcon { get; set; } = Icons.Material.Filled.ChevronRight;

    /// <summary>
    /// The icon shown when a row is expanded.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Filled.ExpandMore"/>.
    /// </remarks>
    [Parameter] public string OpenIcon { get; set; } = Icons.Material.Filled.ExpandMore;

    /// <summary>
    /// The color of the icon button shown when a row is collapsed.
    /// </summary>
    [Parameter] public Color ClosedIconColor { get; set; } = Color.Default;

    /// <summary>
    /// The color of the icon button shown when a row is expanded.
    /// </summary>
    [Parameter] public Color OpenIconColor { get; set; } = Color.Default;

    /// <summary>
    /// The size of both of the icon buttons shown when a row is expanded or collapsed. Overridden by
    /// <see cref="ClosedIconSize" /> and <see cref="OpenIconSize" /> if either is provided. />
    /// </summary>
    [Parameter] public Size IconSize { get; set; } = Size.Medium;

    /// <summary>
    /// The size of the icon button shown when a row is collapsed.
    /// </summary>
    [Parameter] public Size? ClosedIconSize { get; set; }

    /// <summary>
    /// The size of the icon button shown when a row is expanded.
    /// </summary>
    [Parameter] public Size? OpenIconSize { get; set; }

    /// <summary>
    /// The variant of the icon button shown when a row is collapsed.
    /// </summary>
    [Parameter] public Variant ClosedIconVariant { get; set; } = Variant.Text;

    /// <summary>
    /// The variant of the icon button shown when a row is expanded.
    /// </summary>
    [Parameter] public Variant OpenIconVariant { get; set; } = Variant.Text;

    /// <summary>
    /// Function that determines whether the expand/collapse icon button is disabled.
    /// </summary>
    [Parameter] public Func<T, bool> ButtonDisabledFunc { get; set; } = _ => false;

    /// <summary>
    /// When true, the expand/collapse icon button is omitted entirely when it is disabled (i.e. whenever
    /// <see cref="ButtonDisabledFunc" /> returns true) instead of showing it as a visible but disabled component.
    /// </summary>
    [Parameter] public bool HideDisabledIcons { get; set; }

    /// <summary>
    /// Allows this column to be reordered via drag-and-drop operations.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>. When set, this overrides the <see cref="MudDataGrid{T}.DragDropColumnReordering"/> property.
    /// </remarks>
    [Parameter]
    public bool? DragAndDropEnabled { get; set; } = false;
}
