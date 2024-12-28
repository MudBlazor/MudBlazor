// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor;

/// <summary>
/// The behavior of a dropdown popover.
/// </summary>
public readonly struct DropdownSettings
{
    /// <summary>
    /// Displays the dropdown popover in a fixed position, even while scrolling.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Category(CategoryTypes.FormComponent.Behavior)]
    public bool Fixed { get; init; }

    /// <summary>
    /// The behavior applied when there is not enough space for the dropdown popover to be visible.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="OverflowBehavior.FlipOnOpen"/>.
    /// </remarks>
    [Category(CategoryTypes.Popover.Appearance)]
    public OverflowBehavior OverflowBehavior { get; init; } = OverflowBehavior.FlipOnOpen;

    /// <summary>
    /// The location this popover will appear relative to its parent container.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>Origin.BottomLeft</c>
    /// Use <see cref="TransformOrigin"/> to control the direction of the popover from this point.
    /// </remarks>
    [Category(CategoryTypes.Popover.Appearance)]
    public Origin AnchorOrigin { get; init; } = Origin.BottomLeft;

    /// <summary>
    /// The direction this popover will appear relative to the <see cref="AnchorOrigin"/>.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>Origin.TopLeft</c>
    /// </remarks>
    [Category(CategoryTypes.Popover.Appearance)]
    public Origin TransformOrigin { get; init; } = Origin.TopLeft;

    public DropdownSettings() { }
}
