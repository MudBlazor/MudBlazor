// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace MudBlazor;

public interface IEventListener : IAsyncDisposable
{
    /// <summary>
    /// Listing to a javascript event
    /// </summary>
    /// <typeparam name="T">The type of the event args for instance MouseEventArgs for mousemove</typeparam>
    /// <param name="eventName">Name of the DOM event without "on"</param>
    /// <param name="elementId">The value of the id field of the DOM element</param>
    /// <param name="projectionName">The name of a JS function (relative to window) that used to project the event before it is send back to .NET. Can be null, if no projection is needed </param>
    /// <param name="throotleInterval">The delay between the last time the event occurred and the callback is fired. Set to zero, if no delay is requested</param>
    /// <param name="callback">The method that is invoked, if the DOM element is fired. Object will be of type T</param>
    /// <returns>A unique identifier for the event subscription. Should be used to cancel the subscription</returns>
    Task<Guid> Subscribe<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, string elementId, string projectionName, int throotleInterval, Func<object, Task> callback);

    /// <summary>
    /// Listing to a javascript event on the document itself
    /// </summary>
    /// <typeparam name="T">The type of the event args for instance MouseEventArgs for mousemove</typeparam>
    /// <param name="eventName">Name of the DOM event without "on"</param>
    /// <param name="throotleInterval">The delay between the last time the event occurred and the callback is fired. Set to zero, if no delay is requested</param>
    /// <param name="callback">The method that is invoked, if the DOM element is fired. Object will be of type T</param>
    /// <returns>A unique identifier for the event subscription. Should be used to cancel the subscription</returns>
    Task<Guid> SubscribeGlobal<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, int throotleInterval, Func<object, Task> callback);

    /// <summary>
    /// Cancel (unsubscribe) the listening to a DOM event, previous connected by Subscribe
    /// </summary>
    /// <param name="key">The unique event identifier</param>
    /// <returns>true for if the event listener was detached, false if not</returns>
    Task<bool> Unsubscribe(Guid key);
}
