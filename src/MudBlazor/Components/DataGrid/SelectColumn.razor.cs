﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor;

/// <summary>
/// Represents a checkbox column used to select rows in a <see cref="MudDataGrid{T}"/>.
/// </summary>
/// <typeparam name="T">The type of item to select.</typeparam>
public partial class SelectColumn<T>
{
    /// <summary>
    /// Shows a checkbox in the header.
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, all rows can be checked by selecting this checkbox.
    /// </remarks>
    [Parameter] 
    public bool ShowInHeader { get; set; } = true;

    /// <summary>
    /// Shows a checkbox in the footer.
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, all rows can be checked by selecting this checkbox.
    /// </remarks>
    [Parameter] public bool ShowInFooter { get; set; } = true;

    /// <summary>
    /// The size of the checkbox icon.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Size.Medium"/>.
    /// </remarks>
    [Parameter] public Size Size { get; set; } = Size.Medium;
}
