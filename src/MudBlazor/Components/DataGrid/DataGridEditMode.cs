// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Indicates how values are edited for <see cref="MudDataGrid{T}"/> cells.
/// </summary>
public enum DataGridEditMode
{
    /// <summary>
    /// Values are edited in the cell.
    /// </summary>
    Cell,

    /// <summary>
    /// A dialog is shown to edit values.
    /// </summary>
    Form
}
