// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor;

/// <summary>
/// Receives key-down events dispatched by the <see cref="IKeyInterceptorService"/>.
/// </summary>
public interface IKeyDownObserver
{
    /// <summary>
    /// Notifies the observer of a key down event.
    /// </summary>
    /// <param name="args">The keyboard event arguments associated with the key down event.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task NotifyOnKeyDownAsync(KeyboardEventArgs args) => Task.CompletedTask;
}
