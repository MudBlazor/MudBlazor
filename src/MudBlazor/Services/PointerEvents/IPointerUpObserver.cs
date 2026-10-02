// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;


/// <summary>
/// Receives pointer-up events for an element tracked by the <see cref="IPointerEventsNoneService"/>.
/// </summary>
public interface IPointerUpObserver
{
    /// <summary>
    /// Notifies the observer of a pointer up event.
    /// </summary>
    /// <param name="args">The event arguments associated with the pointer up event.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task NotifyOnPointerUpAsync(EventArgs args) => Task.CompletedTask;
}
