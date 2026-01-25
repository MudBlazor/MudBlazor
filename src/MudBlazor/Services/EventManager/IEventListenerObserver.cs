// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents an observer for JavaScript DOM events.
/// </summary>
public interface IEventListenerObserver
{
    /// <summary>
    /// Gets the unique identifier for the event subscription.
    /// </summary>
    Guid SubscriptionId { get; }

    /// <summary>
    /// Notifies the observer when a JavaScript event occurs.
    /// </summary>
    /// <param name="eventArgs">The event data.</param>
    /// <returns>A task representing the asynchronous notification operation.</returns>
    Task NotifyEventOccurredAsync(object eventArgs);
}
