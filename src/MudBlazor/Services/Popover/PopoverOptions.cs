// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents the options for <see cref="IPopoverService"/>.
/// </summary>
public class PopoverOptions
{
    /// <summary>
    /// Gets or sets the amount of time in milliseconds to wait from opening the popover before beginning to perform the transition.
    /// The default value is <c>0</c>.
    /// </summary>
    public TimeSpan Delay { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Gets or sets the length of time that the opening transition takes to complete.
    /// The default value is <c>251 milliseconds</c>.
    /// </summary>
    public TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(251);

    /// <summary>
    /// Gets or sets a value indicating whether to check for the presence of a popover provider <see cref="MudPopoverProvider"/>.
    /// The default value is <c>true</c>.
    /// </summary>
    public bool CheckForPopoverProvider { get; set; } = true;

    /// <summary>
    /// Gets or sets the CSS class of the popover container.
    /// The default value is <c>mud-popover-provider</c>.
    /// </summary>
    public string ContainerClass { get; set; } = "mud-popover-provider";

    /// <summary>
    /// Gets or sets the FlipMargin for the popover.
    /// The default value is <c>0</c>.
    /// </summary>
    public int FlipMargin { get; set; } = 0;

    /// <summary>
    /// Gets the delay for batch popovers detachment.
    /// The default value is <c>0.5 seconds</c>.
    /// </summary>
    public TimeSpan QueueDelay { get; set; } = TimeSpan.FromSeconds(0.5);

    /// <summary>
    /// Gets or sets the overflow padding for the popover. This is used when adjusting popovers that go off screen at the top or left.
    /// It is also used to create max-height for popovers containing a list that will go off screen.
    /// The default value is <c>24</c> roughly equal to the 8dp margin of material design.
    /// </summary>
    public int OverflowPadding { get; set; } = 24;

    /// <summary>
    /// Gets or sets a value indicating whether to throw an exception when a duplicate <see cref="MudPopoverProvider"/> is encountered.
    /// The default value is <c>true</c>.
    /// </summary>
    public bool ThrowOnDuplicateProvider { get; set; } = true;

    /// <summary>
    /// Gets or sets the mode for displaying popovers.
    /// The default value is <c>PopoverMode.Default</c>.
    /// </summary>
    public PopoverMode Mode { get; set; } = PopoverMode.Default;

    /// <summary>
    /// Gets or sets a value indicating whether the modal overlay prevents interaction with background elements.
    /// </summary>
    /// <remarks>
    /// Only applies to components that use a <see cref="MudPopover"/> in conjunction with a <see cref="MudOverlay"/>
    /// to close the popover when a user clicks outside, such as <see cref="MudSelect{T}"/>.
    /// The default value is <c>false</c>.
    /// </remarks>
    public bool ModalOverlay { get; set; }

    /// <summary>
    /// Gets or sets the behavior applied when there is not enough space for a dropdown popover to be visible.
    /// </summary>
    /// <remarks>
    /// The default value is <see cref="OverflowBehavior.FlipAlways"/>.
    /// </remarks>
    public OverflowBehavior OverflowBehavior { get; set; } = OverflowBehavior.FlipAlways;
}
