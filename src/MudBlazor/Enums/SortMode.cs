// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Indicates the sorting mode for a <see cref="MudDataGrid{T}"/>.
/// </summary>
public enum SortMode
{
    /// <summary>
    /// Sorting is not allowed.
    /// </summary>
    [Description("none")]
    None,

    /// <summary>
    /// Sorting can occur for one column at a time.
    /// </summary>
    [Description("single")]
    Single,

    /// <summary>
    /// Sorting can be done for multiple columns.
    /// </summary>
    [Description("multiple")]
    Multiple
}
