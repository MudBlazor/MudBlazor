// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using MudBlazor.Services;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents a service that serves to listen to browser window size changes and breakpoints.
/// </summary>
public interface IBrowserViewportService : IAsyncDisposable
{
    /// <summary>
    /// Gets the current resize options.
    /// </summary>
    ResizeOptions ResizeOptions { get; }

    /// <summary>
    /// Subscribes an observer to receive notifications of browser window size changes and breakpoints.
    /// </summary>
    /// <param name="observer">The observer to subscribe.</param>
    /// <param name="fireImmediately">Indicates whether the event will fire immediately with the current <see cref="BrowserWindowSize"/> and <see cref="Breakpoint"/> information without waiting for changes.
    /// When set to <c>true</c>, the event will be fired immediately. When set to <c>false</c>, it will wait for the service to observe any changes before firing the event.
    /// The event <see cref="IBrowserViewportObserver.NotifyBrowserViewportChangeAsync"/> won't fire if such <see cref="IBrowserViewportObserver.Id"/> already exist, for example when you re-subscribe.
    /// </param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SubscribeAsync(IBrowserViewportObserver observer, bool fireImmediately = true);

    /// <summary>
    /// Subscribes a lambda <see cref="Action{BrowserViewportEventArgs}"/> with a unique ID to receive notifications of browser window size changes and breakpoints.
    /// </summary>
    /// <param name="observerId">The unique ID associated with the observer. Use this ID to later <see cref="UnsubscribeAsync(Guid)"/>.</param>
    /// <param name="lambda">The lambda function to subscribe.</param>
    /// <param name="options">The resize options for the observer. When set to null, the global options provided during AddMudServices/AddMudBlazorResizeListener will be used.
    /// When specific options are provided, they will be used to observe the changes.
    /// The <see cref="Action{BrowserViewportEventArgs}"/> won't be invoked if such <see cref="IBrowserViewportObserver.Id"/> already exist, for example when you re-subscribe.
    /// After you pass the options, modifying the instance won't have any effect, including re-subscription, as C# and JS side doesn't support this, you need to <see cref="UnsubscribeAsync(Guid)"/> and subscribe again.
    /// </param>
    /// <param name="fireImmediately">Indicates whether the event will fire immediately with the current <see cref="BrowserWindowSize"/> and <see cref="Breakpoint"/> information without waiting for changes. When set to <c>true</c>, the event will be fired immediately. When set to <c>false</c>, it will wait for the service to observe any changes before firing the event.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SubscribeAsync(Guid observerId, Action<BrowserViewportEventArgs> lambda, ResizeOptions? options = null, bool fireImmediately = true);

    /// <summary>
    /// Subscribes a lambda <see cref="Func{BrowserViewportEventArgs, Task}"/> with a unique ID to receive notifications of browser window size changes and breakpoints.
    /// </summary>
    /// <param name="observerId">The unique ID associated with the observer. Use this ID to later <see cref="UnsubscribeAsync(Guid)"/>.</param>
    /// <param name="lambda">The lambda function to subscribe.</param>
    /// <param name="options">The resize options for the observer. When set to null, the global options provided during AddMudServices/AddMudBlazorResizeListener will be used.
    /// When specific options are provided, they will be used to observe the changes.
    /// The <see cref="Func{BrowserViewportEventArgs, Task}"/> won't be invoked if such <see cref="IBrowserViewportObserver.Id"/> already exist, for example when you re-subscribe.
    /// After you pass the options, modifying the instance won't have any effect, including re-subscription, as C# and JS side doesn't support this, you need to <see cref="UnsubscribeAsync(Guid)"/> and subscribe again.
    /// </param>
    /// <param name="fireImmediately">Indicates whether the event will fire immediately with the current <see cref="BrowserWindowSize"/> and <see cref="Breakpoint"/> information without waiting for changes. When set to <c>true</c>, the event will be fired immediately. When set to <c>false</c>, it will wait for the service to observe any changes before firing the event.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SubscribeAsync(Guid observerId, Func<BrowserViewportEventArgs, Task> lambda, ResizeOptions? options = null, bool fireImmediately = true);

    /// <summary>
    /// Unsubscribes an observer from receiving notifications.
    /// </summary>
    /// <param name="observer">The observer to unsubscribe.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UnsubscribeAsync(IBrowserViewportObserver observer);

    /// <summary>
    /// Unsubscribes with the specified ID from receiving notifications.
    /// </summary>
    /// <param name="observerId">The unique ID associated with the observer to unsubscribe.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UnsubscribeAsync(Guid observerId);

    /// <summary>
    /// Matches if the document currently matches the media query, or false if not. 
    /// </summary>
    /// <param name="mediaQuery">A string specifying the media query.</param>
    /// <returns>A task representing a boolean value that is <c>true</c> if the document currently matches the media query; otherwise, it's <c>false</c>.</returns>
    Task<bool> IsMediaQueryMatchAsync(string mediaQuery);

    /// <summary>
    /// Check if the current breakpoint fits within the current window size
    /// </summary>
    /// <param name="breakpoint">The breakpoint to check.</param>
    /// <returns>A task representing whether the media size meets the criteria. Returns <c>true</c> if the media size meets the criteria; otherwise, returns <c>false</c>. For example, if the current window size is <see cref="Breakpoint.Sm"/> and the breakpoint is set to <see cref="Breakpoint.SmAndDown"/>, this method will return <c>true</c>.</returns>
    Task<bool> IsBreakpointWithinWindowSizeAsync(Breakpoint breakpoint);

    /// <summary>
    /// Check if the current breakpoint fits within the reference size
    /// </summary>
    /// <param name="breakpoint">The breakpoint to check</param>
    /// <param name="reference">The reference breakpoint (<see cref="Breakpoint.Xs"/>, <see cref="Breakpoint.Sm"/>, <see cref="Breakpoint.Md"/>, <see cref="Breakpoint.Lg"/>,<see cref="Breakpoint.Xl"/>, <see cref="Breakpoint.Xxl"/>)</param>
    /// <returns>A task representing whether the media size meets the criteria. Returns <c>true</c> if the media size meets the criteria; otherwise, returns <c>false</c>. For example, if the reference size is <see cref="Breakpoint.Sm"/> and the breakpoint is set to <see cref="Breakpoint.SmAndDown"/>, this method will return <c>true</c>.</returns>
    Task<bool> IsBreakpointWithinReferenceSizeAsync(Breakpoint breakpoint, Breakpoint reference);

    /// <summary>
    /// Get the current breakpoint
    /// </summary>
    /// <returns>A task representing the current breakpoint</returns>
    Task<Breakpoint> GetCurrentBreakpointAsync();

    /// <summary>
    /// Get the current size of the window
    /// </summary>
    /// <returns>A task representing the current browser size</returns>
    Task<BrowserWindowSize> GetCurrentBrowserWindowSizeAsync();
}
