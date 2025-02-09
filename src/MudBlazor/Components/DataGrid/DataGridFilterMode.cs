// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Indicates the filtering behavior for <see cref="MudDataGrid{T}"/> rows.
/// </summary>
public enum DataGridFilterMode
{
    /// <summary>
    /// All filters are managed in a single popover.
    /// </summary>
    Simple,

    /// <summary>
    /// Only the current column filter is managed in a popover.
    /// </summary>
    ColumnFilterMenu,

    /// <summary>
    /// Filters are shown as a row in the grid.
    /// </summary>
    ColumnFilterRow,
}
