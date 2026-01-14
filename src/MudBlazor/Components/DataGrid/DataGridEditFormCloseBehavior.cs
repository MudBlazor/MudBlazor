// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Defines how the data grid edit form behaves after an edit operation.
/// </summary>
public enum DataGridEditFormCloseBehavior
{
    /// <summary>
    /// Close the edit form after <see cref="MudDataGrid{T}.CommittedItemChangesBehavior"/> has completed.
    /// </summary>
    Close,

    /// <summary>
    /// Keep the edit form open after <see cref="MudDataGrid{T}.CommittedItemChangesBehavior"/> has completed.
    /// </summary>
    KeepOpen
}
