// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using MudBlazor.Services;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Provides data for the browser viewport event.
/// </summary>
public class BrowserViewportEventArgs : EventArgs
{
    /// <summary>
    /// Gets the ID of the JavaScript listener.
    /// </summary>
    public Guid JavaScriptListenerId { get; }

    /// <summary>
    /// Gets the browser window size.
    /// </summary>
    public BrowserWindowSize BrowserWindowSize { get; }

    /// <summary>
    /// Gets the breakpoint associated with the browser size.
    /// </summary>
    public Breakpoint Breakpoint { get; }

    /// <summary>
    /// Gets a value indicating whether this is the first event that was fired.
    /// This is true when you set <c>fireImmediately</c> to <c>true</c> in the <see cref="IBrowserViewportService.SubscribeAsync(IBrowserViewportObserver, bool)"/>, <see cref="IBrowserViewportService.SubscribeAsync(Guid, Action{BrowserViewportEventArgs}, ResizeOptions?, bool)"/>, <see cref="IBrowserViewportService.SubscribeAsync(Guid, Func{BrowserViewportEventArgs, Task}, ResizeOptions?, bool)"/>  method.
    /// </summary>
    public bool IsImmediate { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BrowserViewportEventArgs"/> class.
    /// </summary>
    /// <param name="javaScriptListenerId">The ID of the JavaScript listener.</param>
    /// <param name="browserWindowSize">The browser window size.</param>
    /// <param name="breakpoint">The breakpoint associated with the browser size.</param>
    /// <param name="isImmediate">Specifies whether this is the first event that was fired.</param>
    public BrowserViewportEventArgs(Guid javaScriptListenerId, BrowserWindowSize browserWindowSize, Breakpoint breakpoint, bool isImmediate = false)
    {
        JavaScriptListenerId = javaScriptListenerId;
        BrowserWindowSize = browserWindowSize;
        Breakpoint = breakpoint;
        IsImmediate = isImmediate;
    }
}
