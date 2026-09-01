// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Receives both key-down and key-up events from the <see cref="IKeyInterceptorService"/> for a specific HTML element.
/// </summary>
public interface IKeyInterceptorObserver : IKeyDownObserver, IKeyUpObserver
{
    /// <summary>
    /// Gets the ID of the ancestor HTML element associated with this observer.
    /// This ID should be a unique identifier.
    /// </summary>
    string ElementId { get; }
}
