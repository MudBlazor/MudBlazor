// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Interface for listening to JavaScript events.
/// </summary>
public interface IEventListener : IAsyncDisposable
{
    /// <summary>
    /// Listens to a JavaScript event.
    /// </summary>
    /// <typeparam name="T">The type of the event args, for instance, MouseEventArgs for mousemove.</typeparam>
    /// <param name="eventName">Name of the DOM event without "on".</param>
    /// <param name="elementId">The value of the id field of the DOM element.</param>
    /// <param name="projectionName">The name of a JS function (relative to window) that is used to project the event before it is sent back to .NET. Can be null if no projection is needed.</param>
    /// <param name="throttleInterval">The delay between the last time the event occurred and the callback is fired. Set to zero if no delay is requested.</param>
    /// <param name="callback">The method that is invoked when the DOM element event is fired. The object will be of type T.</param>
    /// <returns>A unique identifier for the event subscription. Should be used to cancel the subscription.</returns>
    Task<Guid> Subscribe<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, string elementId, string projectionName, int throttleInterval, Func<object, Task> callback);

    /// <summary>
    /// Listens to a JavaScript event on the document itself.
    /// </summary>
    /// <typeparam name="T">The type of the event args, for instance, MouseEventArgs for mousemove.</typeparam>
    /// <param name="eventName">Name of the DOM event without "on".</param>
    /// <param name="throttleInterval">The delay between the last time the event occurred and the callback is fired. Set to zero if no delay is requested.</param>
    /// <param name="callback">The method that is invoked when the DOM element event is fired. The object will be of type T.</param>
    /// <returns>A unique identifier for the event subscription. Should be used to cancel the subscription.</returns>
    Task<Guid> SubscribeGlobal<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, int throttleInterval, Func<object, Task> callback);

    /// <summary>
    /// Cancels (unsubscribes) the listening to a DOM event, previously connected by Subscribe.
    /// </summary>
    /// <param name="key">The unique event identifier.</param>
    /// <returns>true if the event listener was detached, false if not.</returns>
    Task<bool> Unsubscribe(Guid key);
}
