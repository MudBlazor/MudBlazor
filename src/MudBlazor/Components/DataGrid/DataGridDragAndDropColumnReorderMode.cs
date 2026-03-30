// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

public enum DataGridDragAndDropColumnReorderMode
{
    /// <summary>
    /// Swaps the locations of the dragged column with the target column.
    /// </summary>
    Swap,

    /// <summary>
    /// Inserts the dragged column at the target column's position, shifting all following columns to the right.
    /// </summary>
    Insert
}
