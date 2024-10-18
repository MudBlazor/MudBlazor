﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// Factory interface for creating instances of <see cref="IScrollListener"/>.
/// </summary>
public interface IScrollListenerFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IScrollListener"/> for the specified selector.
    /// </summary>
    /// <param name="selector">The CSS selector for the element to listen for scroll events.</param>
    /// <returns>A new instance of <see cref="IScrollListener"/>.</returns>
    IScrollListener Create(string selector);
}
