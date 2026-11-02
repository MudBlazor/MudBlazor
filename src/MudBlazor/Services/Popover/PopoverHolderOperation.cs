// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Indicates whether a popover holder is being created, removed, or updated in the <see cref="IPopoverService"/>.
/// </summary>
public enum PopoverHolderOperation
{
    /// <summary>
    /// Specifies the creation operation for a popover holder.
    /// </summary>
    Create = 0,

    /// <summary>
    /// Specifies the removal operation for a popover holder.
    /// </summary>
    Remove = 1,

    /// <summary>
    /// Specifies the update operation for a popover holder.
    /// </summary>
    Update = 2
}
