// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor;

/// <summary>
/// The behavior of a dropdown popover.
/// </summary>
public struct DropdownSettings
{
    /// <summary>
    /// Displays the dropdown popover in a fixed position, even while scrolling.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.FormComponent.Behavior)]
    public bool Fixed { get; set; }

    /// <summary>
    /// The behavior applied when there is not enough space for the dropdown popover to be visible.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="OverflowBehavior.FlipOnOpen"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Popover.Appearance)]
    public OverflowBehavior OverflowBehavior { get; set; } = OverflowBehavior.FlipOnOpen;

    public DropdownSettings() { }
}
