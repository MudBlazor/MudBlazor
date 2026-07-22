// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable

/// <summary>
/// Indicates the position of the commit button during inline edits to a <see cref="MudTable{T}"/>.
/// </summary>
public enum TableApplyButtonPosition
{
    /// <summary>
    /// The commit button is at the start of the row.
    /// </summary>
    Start,

    /// <summary>
    /// The commit button is at the end of the row.
    /// </summary>
    End,

    /// <summary>
    /// The commit button is at both the start and end of the row.
    /// </summary>
    StartAndEnd,
}
