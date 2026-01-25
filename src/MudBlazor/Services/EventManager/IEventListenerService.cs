// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents a service for listening to JavaScript events.
/// </summary>
/// <remarks>
/// This service supports multiple concurrent subscriptions and can be used to listen to DOM events
/// on specific elements or globally on the document. Subscriptions are automatically cleaned up when
/// the service is disposed.
/// </remarks>
public interface IEventListenerService : IAsyncDisposable
{
    /// <summary>
    /// Subscribes an observer to a JavaScript event on a specific DOM element.
    /// </summary>
    /// <param name="observer">The observer to subscribe.</param>
    /// <param name="eventName">Name of the DOM event without "on" prefix (e.g., "click", "mousemove").</param>
    /// <param name="elementId">The value of the id field of the DOM element.</param>
    /// <param name="projectionName">The name of a JS function (relative to window) that is used to project the event before it is sent back to .NET. Can be null if no projection is needed.</param>
    /// <param name="throttleInterval">The delay in milliseconds between the last time the event occurred and the callback is fired. Set to zero if no delay is requested.</param>
    /// <param name="eventType">The type of the event arguments.</param>
    /// <param name="eventProperties">The properties of the event type to include in the projection.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SubscribeAsync(IEventListenerObserver observer, string eventName, string elementId, string? projectionName, int throttleInterval, Type eventType, string[] eventProperties);

    /// <summary>
    /// Subscribes to a JavaScript event on a specific DOM element.
    /// </summary>
    /// <typeparam name="T">The type of the event args, for instance, MouseEventArgs for mousemove.</typeparam>
    /// <param name="subscriptionId">A unique identifier for the subscription.</param>
    /// <param name="eventName">Name of the DOM event without "on" prefix (e.g., "click", "mousemove").</param>
    /// <param name="elementId">The value of the id field of the DOM element.</param>
    /// <param name="projectionName">The name of a JS function (relative to window) that is used to project the event before it is sent back to .NET. Can be null if no projection is needed.</param>
    /// <param name="throttleInterval">The delay in milliseconds between the last time the event occurred and the callback is fired. Set to zero if no delay is requested.</param>
    /// <param name="callback">The asynchronous method that is invoked when the DOM element event is fired. The object will be of type T.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SubscribeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Guid subscriptionId, string eventName, string elementId, string? projectionName, int throttleInterval, Func<object, Task> callback);

    /// <summary>
    /// Subscribes to a JavaScript event on a specific DOM element.
    /// </summary>
    /// <typeparam name="T">The type of the event args, for instance, MouseEventArgs for mousemove.</typeparam>
    /// <param name="subscriptionId">A unique identifier for the subscription.</param>
    /// <param name="eventName">Name of the DOM event without "on" prefix (e.g., "click", "mousemove").</param>
    /// <param name="elementId">The value of the id field of the DOM element.</param>
    /// <param name="projectionName">The name of a JS function (relative to window) that is used to project the event before it is sent back to .NET. Can be null if no projection is needed.</param>
    /// <param name="throttleInterval">The delay in milliseconds between the last time the event occurred and the callback is fired. Set to zero if no delay is requested.</param>
    /// <param name="callback">The synchronous method that is invoked when the DOM element event is fired. The object will be of type T.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SubscribeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Guid subscriptionId, string eventName, string elementId, string? projectionName, int throttleInterval, Action<object> callback);

    /// <summary>
    /// Subscribes an observer to a JavaScript event on the document itself.
    /// </summary>
    /// <param name="observer">The observer to subscribe.</param>
    /// <param name="eventName">Name of the DOM event without "on" prefix (e.g., "click", "mousemove").</param>
    /// <param name="throttleInterval">The delay in milliseconds between the last time the event occurred and the callback is fired. Set to zero if no delay is requested.</param>
    /// <param name="eventType">The type of the event arguments.</param>
    /// <param name="eventProperties">The properties of the event type to include in the projection.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SubscribeGlobalAsync(IEventListenerObserver observer, string eventName, int throttleInterval, Type eventType, string[] eventProperties);

    /// <summary>
    /// Subscribes to a JavaScript event on the document itself.
    /// </summary>
    /// <typeparam name="T">The type of the event args, for instance, MouseEventArgs for mousemove.</typeparam>
    /// <param name="subscriptionId">A unique identifier for the subscription.</param>
    /// <param name="eventName">Name of the DOM event without "on" prefix (e.g., "click", "mousemove").</param>
    /// <param name="throttleInterval">The delay in milliseconds between the last time the event occurred and the callback is fired. Set to zero if no delay is requested.</param>
    /// <param name="callback">The asynchronous method that is invoked when the DOM element event is fired. The object will be of type T.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SubscribeGlobalAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Guid subscriptionId, string eventName, int throttleInterval, Func<object, Task> callback);

    /// <summary>
    /// Subscribes to a JavaScript event on the document itself.
    /// </summary>
    /// <typeparam name="T">The type of the event args, for instance, MouseEventArgs for mousemove.</typeparam>
    /// <param name="subscriptionId">A unique identifier for the subscription.</param>
    /// <param name="eventName">Name of the DOM event without "on" prefix (e.g., "click", "mousemove").</param>
    /// <param name="throttleInterval">The delay in milliseconds between the last time the event occurred and the callback is fired. Set to zero if no delay is requested.</param>
    /// <param name="callback">The synchronous method that is invoked when the DOM element event is fired. The object will be of type T.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SubscribeGlobalAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Guid subscriptionId, string eventName, int throttleInterval, Action<object> callback);

    /// <summary>
    /// Unsubscribes an observer from receiving event notifications.
    /// </summary>
    /// <param name="observer">The observer to unsubscribe.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UnsubscribeAsync(IEventListenerObserver observer);

    /// <summary>
    /// Unsubscribes from receiving event notifications with the specified subscription ID.
    /// </summary>
    /// <param name="subscriptionId">The unique subscription identifier.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UnsubscribeAsync(Guid subscriptionId);
}
