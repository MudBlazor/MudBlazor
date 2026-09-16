// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Indicates how columns are reordered when dragging column headers in a <see cref="MudDataGrid{T}"/>.
/// </summary>
public enum DataGridDragAndDropColumnReorderMode
{
    /// <summary>
    /// Swaps the locations of the dragged column with the target column.
    /// </summary>
    Swap,

    /// <summary>
    /// Inserts the dragged column at the target column's position, shifting all subsequent columns one position.
    /// </summary>
    Insert
}
