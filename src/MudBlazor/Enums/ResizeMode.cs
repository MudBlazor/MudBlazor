// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// Indicates the column resizing behavior for a <see cref="MudDataGrid{T}"/>.
/// </summary>
public enum ResizeMode
{
    /// <summary>
    /// Nothing happens when the grid is resized.
    /// </summary>
    [Description("none")]
    None,

    /// <summary>
    /// Columns can be expanded a limited amount, ensuring all columns remain visible.
    /// </summary>
    [Description("column")]
    Column,

    /// <summary>
    /// Columns can be expanded any amount; the grid width will be expanded.
    /// </summary>
    [Description("container")]
    Container
}
