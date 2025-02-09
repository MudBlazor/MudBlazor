// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents a service that intercepts key events for specified HTML elements.
/// </summary>
public interface IKeyInterceptorService : IAsyncDisposable
{
    /// <summary>
    /// Subscribes an observer to key events for a specified element with the provided options.
    /// </summary>
    /// <param name="observer">The observer that will receive key events.</param>
    /// <param name="options">The options for key interception.</param>
    /// <remarks>
    /// If you re-subscribe with the same observer and different <see cref="KeyInterceptorOptions"/> settings, the new <see cref="KeyInterceptorOptions"/> will not have any effect.
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SubscribeAsync(IKeyInterceptorObserver observer, KeyInterceptorOptions options);

    /// <summary>
    /// Subscribes to key events for a specified element with the provided options.
    /// </summary>
    /// <param name="elementId">The unique identifier of the HTML element.</param>
    /// <param name="options">The options for key interception.</param>
    /// <param name="keyDown">The observer for key down events.</param>
    /// <param name="keyUp">The observer for key up events.</param>
    /// <remarks>
    /// If you re-subscribe with the same elementId and different <see cref="KeyInterceptorOptions"/> settings, the new <see cref="KeyInterceptorOptions"/> will not have any effect.
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SubscribeAsync(string elementId, KeyInterceptorOptions options, IKeyDownObserver? keyDown = null, IKeyUpObserver? keyUp = null);

    /// <summary>
    /// Subscribes to key events for a specified element with the provided options.
    /// </summary>
    /// <param name="elementId">The unique identifier of the HTML element.</param>
    /// <param name="options">The options for key interception.</param>
    /// <param name="keyDown">The lambda action to invoke on key down events.</param>
    /// <param name="keyUp">The lambda action to invoke on key up events.</param>
    /// <remarks>
    /// If you re-subscribe with the same elementId and different <see cref="KeyInterceptorOptions"/> settings, the new <see cref="KeyInterceptorOptions"/> will not have any effect.
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SubscribeAsync(string elementId, KeyInterceptorOptions options, Action<KeyboardEventArgs>? keyDown = null, Action<KeyboardEventArgs>? keyUp = null);

    /// <summary>
    /// Subscribes to key events for a specified element with the provided options.
    /// </summary>
    /// <param name="elementId">The unique identifier of the HTML element.</param>
    /// <param name="options">The options for key interception.</param>
    /// <param name="keyDown">The asynchronous lambda function to invoke on key down events.</param>
    /// <param name="keyUp">The asynchronous lambda function to invoke on key up events.</param>
    /// <remarks>
    /// If you re-subscribe with the same elementId and different <see cref="KeyInterceptorOptions"/> settings, the new <see cref="KeyInterceptorOptions"/> will not have any effect.
    /// </remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SubscribeAsync(string elementId, KeyInterceptorOptions options, Func<KeyboardEventArgs, Task>? keyDown = null, Func<KeyboardEventArgs, Task>? keyUp = null);

    /// <summary>
    /// Updates the key options for a specified element.
    /// </summary>
    /// <param name="observer">The observer.</param>
    /// <param name="option">The key options to update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateKeyAsync(IKeyInterceptorObserver observer, KeyOptions option);

    /// <summary>
    /// Updates the key options for a specified element.
    /// </summary>
    /// <param name="elementId">The unique identifier of the HTML element.</param>
    /// <param name="option">The key options to update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateKeyAsync(string elementId, KeyOptions option);

    /// <summary>
    /// Unsubscribes an observer from key events.
    /// </summary>
    /// <param name="observer">The observer to unsubscribe.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UnsubscribeAsync(IKeyInterceptorObserver observer);

    /// <summary>
    /// Unsubscribes from key events for a specified element.
    /// </summary>
    /// <param name="elementId">The unique identifier of the HTML element.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UnsubscribeAsync(string elementId);
}
