// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// Factory interface for creating instances of <see cref="IScrollListener"/>.
/// </summary>
public interface IScrollListenerFactory
{
    /// <inheritdoc cref="Create(string?, int, bool)"/>
    IScrollListener Create(string? selector);

    /// <inheritdoc cref="Create(string?, int, bool)"/>
    IScrollListener Create(string? selector, int reportRateMs);

    /// <summary>
    /// Creates a new instance of <see cref="IScrollListener"/> for the specified selector.
    /// </summary>
    /// <param name="selector">The CSS selector for the element to listen for scroll events.</param>
    /// <param name="reportRateMs">The rate at which the <see cref="IScrollListener"/> will report scroll position changes (in milliseconds). Defaults to <c>100</c>.</param>
    /// <param name="fireOnStart">Whether to fire the <see cref="IScrollListener.OnScroll"/> event immediately after creation. Defaults to <c>false</c>.</param>
    /// <remarks>
    /// If you are creating this <see cref="IScrollListener"/> instance yourself using this factory, then you need to manually call <see cref="ScrollListener.Dispose"/>; otherwise, you will get a memory leak.
    /// </remarks>
    /// <returns>A new instance of <see cref="IScrollListener"/>.</returns>
    IScrollListener Create(string? selector, int reportRateMs, bool fireOnStart);
}
