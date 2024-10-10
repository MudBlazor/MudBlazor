// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents an observer that observes and responds to key up events.
/// </summary>
public interface IKeyUpObserver
{
    /// <summary>
    /// Notifies the observer of a key up event.
    /// </summary>
    /// <param name="args">The keyboard event arguments associated with the key up event.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task NotifyOnKeyUpAsync(KeyboardEventArgs args) => Task.CompletedTask;
}
