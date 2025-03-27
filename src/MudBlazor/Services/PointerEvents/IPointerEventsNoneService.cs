// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable

/// <summary>
/// Represents a service that enables C# components to receive pointer event notifications for HTML elements
/// with <c>pointer-events: none</c>, which normally do not receive any pointer interactions.
/// </summary>
internal interface IPointerEventsNoneService : IAsyncDisposable
{
    /// <summary>
    /// Subscribes an observer to pointer events for a specified element.
    /// </summary>
    /// <param name="observer">The observer that will receive pointer event notifications.</param>
    /// <param name="options">Options for configuring the pointer event listener behavior.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SubscribeAsync(IPointerEventsNoneObserver observer, PointerEventsNoneOptions options);

    /// <summary>
    /// Subscribes to pointer events for a specified element by its ID and optionally provides callbacks for pointer down and up events.
    /// </summary>
    /// <param name="elementId">The unique ID of the HTML element to observe.</param>
    /// <param name="options">Options for configuring the pointer event listener behavior.</param>
    /// <param name="pointerDown">Optional observer that handles pointer down events.</param>
    /// <param name="pointerUp">Optional observer that handles pointer up events.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SubscribeAsync(string elementId, PointerEventsNoneOptions options, IPointerDownObserver? pointerDown = null, IPointerUpObserver? pointerUp = null);

    /// <summary>
    /// Unsubscribes a previously registered observer from pointer events.
    /// </summary>
    /// <param name="observer">The observer to unsubscribe.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UnsubscribeAsync(IPointerEventsNoneObserver observer);

    /// <summary>
    /// Unsubscribes from pointer events for a specified element by its ID.
    /// </summary>
    /// <param name="elementId">The unique ID of the HTML element to stop observing.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UnsubscribeAsync(string elementId);
}
