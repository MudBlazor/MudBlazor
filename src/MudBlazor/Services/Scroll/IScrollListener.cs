// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// Interface for a scroll listener that listens to scroll events on a specified element.
/// </summary>
public interface IScrollListener : IDisposable
{
    /// <summary>
    /// Gets or sets the CSS selector to which the scroll event will be attached.
    /// </summary>
    string? Selector { get; set; }

    /// <summary>
    /// Occurs when a scroll event is detected on the specified element.
    /// </summary>
    event EventHandler<ScrollEventArgs> OnScroll;
}
