// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Indicates how values are editing for <see cref="MudDataGrid{T}"/> cells.
/// </summary>
public enum DataGridEditMode
{
    /// <summary>
    /// Values can be edited in place within cells.
    /// </summary>
    Cell,

    /// <summary>
    /// A dialog is shown to edit cell values.
    /// </summary>
    Form
}
