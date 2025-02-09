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
    /// Gets or sets a value indicating whether to check for the presence of a popover provider <see cref="MudPopoverProvider"/>.
    /// The default value is <c>true</c>.
    /// </summary>
    public bool CheckForPopoverProvider { get; set; } = true;

    /// <summary>
    /// Gets or sets the CSS class of the popover container.
    /// The default value is <c>mudblazor-main-content</c>.
    /// </summary>
    public string ContainerClass { get; set; } = "mudblazor-main-content";

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
    /// Gets or sets a value indicating whether to throw an exception when a duplicate <see cref="MudPopoverProvider"/> is encountered.
    /// The default value is <c>true</c>.
    /// </summary>
    public bool ThrowOnDuplicateProvider { get; set; } = true;

    /// <summary>
    /// Gets or sets the mode for displaying popovers.
    /// The default value is <c>PopoverMode.Default</c>.
    /// </summary>
    public PopoverMode Mode { get; set; } = PopoverMode.Default;
}
