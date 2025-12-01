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
    /// The rate at which the <see cref="IScrollListener"/> will report scroll position changes (in milliseconds).
    /// </summary>
    /// <remarks>
    /// Defaults to <c>100</c>.
    /// </remarks>
    public int ReportRateMs { get; set; }

    /// <summary>
    /// Whether to fire the <see cref="OnScroll"/> event immediately after creation.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    public bool FireOnStart { get; set; }

    /// <summary>
    /// Occurs when a scroll event is detected on the specified element.
    /// </summary>
    event EventHandler<ScrollEventArgs> OnScroll;
}
