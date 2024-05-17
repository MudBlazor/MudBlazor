// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Indicates the behavior which begins editing a cell.
/// </summary>
public enum DataGridEditTrigger
{
    /// <summary>
    /// Clicking on the row will edit the cell.
    /// </summary>
    OnRowClick,

    /// <summary>
    /// Clicking the edit button will edit the cell.
    /// </summary>
    Manual
}
