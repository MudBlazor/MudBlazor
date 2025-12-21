// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// Interface for a scroll listener that listens to scroll events on a specified element.
/// </summary>
public interface IScrollListener : IAsyncDisposable
{
    /// <summary>
    /// Gets or sets the CSS selector to which the scroll event will be attached.
    /// </summary>
    string? Selector { get; set; }

    /// <summary>
    /// The rate at which the <see cref="IScrollListener"/> will report scroll position changes (in milliseconds).
    /// </summary>
    /// <remarks>
    /// Defaults to <c>100</c>.
    /// </remarks>
    public int ReportRateMs { get; set; }

    /// <summary>
    /// Occurs when a scroll event is detected on the specified element.
    /// </summary>
    event EventHandler<ScrollEventArgs> OnScroll;

    /// <summary>
    /// Returns the same data as an <see cref="OnScroll"/> event would return without requiring user input.
    /// </summary>
    /// <returns><see cref="ScrollEventArgs"/></returns>
    public ValueTask<ScrollEventArgs> GetCurrentScrollDataAsync();
}
