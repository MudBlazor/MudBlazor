// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor.Interop;

namespace MudBlazor;

/// <summary>
/// Centralizes scroll operations that need JS interop (scrolling to elements, locking scroll, etc.).
/// </summary>
/// <remarks>
/// Components use this service to perform consistent scroll behaviors across the library, keeping JS interop calls in one place and avoiding duplicate logic.
/// </remarks>
internal sealed class ScrollManager : IScrollManager
{
    private readonly IJSRuntime _jSRuntime;
    private readonly ILogger<ScrollManager> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScrollManager"/> class with the specified JavaScript runtime.
    /// </summary>
    /// <param name="jSRuntime">The JavaScript runtime.</param>
    /// <param name="logger">The logger.</param>
    public ScrollManager(IJSRuntime jSRuntime, ILogger<ScrollManager> logger)
    {
        _jSRuntime = jSRuntime;
        _logger = logger;
    }

    /// <inheritdoc />
    public ValueTask ScrollToAsync(string? id, int left, int top, ScrollBehavior scrollBehavior) =>
        InvokeVoidSafelyAsync("mudScrollManager.scrollTo", id, left, top, scrollBehavior.ToStringFast(true));

    /// <inheritdoc />
    public ValueTask ScrollIntoViewAsync(string? selector, ScrollBehavior behavior) =>
        InvokeVoidSafelyAsync("mudScrollManager.scrollIntoView", selector, behavior.ToStringFast(true));

    /// <inheritdoc />
    public ValueTask ScrollToTopAsync(string? id, ScrollBehavior scrollBehavior = ScrollBehavior.Auto) =>
        ScrollToAsync(id, 0, 0, scrollBehavior);

    /// <inheritdoc />
    public ValueTask ScrollToBottomAsync(string elementId, ScrollBehavior scrollBehavior = ScrollBehavior.Auto) =>
        InvokeVoidSafelyAsync("mudScrollManager.scrollToBottom", elementId, scrollBehavior.ToStringFast(true));

    /// <inheritdoc />
    public ValueTask ScrollToYearAsync(string elementId) =>
        InvokeVoidSafelyAsync("mudScrollManager.scrollToYear", elementId);

    /// <inheritdoc />
    public ValueTask ScrollToListItemAsync(string elementId) =>
        InvokeVoidSafelyAsync("mudScrollManager.scrollToListItem", elementId);

    // lockScroll and unlockScroll use a counter system in javascript so we can lock/unlock without limit
    // and maintain the proper lock. IF YOU CHANGE THIS, CHANGE THE JAVASCRIPT AS WELL
    /// <inheritdoc />
    public ValueTask LockScrollAsync(string selector = "body", string cssClass = "scroll-locked") =>
        InvokeVoidSafelyAsync("mudScrollManager.lockScroll", selector, cssClass);

    /// <inheritdoc />
    public ValueTask UnlockScrollAsync(string selector = "body", string cssClass = "scroll-locked") =>
        _jSRuntime.InvokeVoidAsyncIgnoreErrors("mudScrollManager.unlockScroll", selector, cssClass);

    /// <inheritdoc />
    public ValueTask ScrollToVirtualizedItemAsync(string containerId, int itemIndex, double itemHeight, string targetItemId, ScrollBehavior scrollBehavior = ScrollBehavior.Auto) =>
        _jSRuntime.InvokeVoidAsyncIgnoreErrors("mudScrollManager.scrollToVirtualizedItem", containerId, itemIndex, itemHeight, targetItemId, scrollBehavior.ToStringFast(true));

    // Several callers await these from component lifecycle methods (e.g. MudOverlay locks scroll for every dialog), where an unhandled JSException tears down a Blazor Server circuit.
    // Tolerate a missing MudBlazor script: log actionable guidance once instead of crashing (#13477).
    private async ValueTask InvokeVoidSafelyAsync(string identifier, params object?[] args)
    {
        try
        {
            await _jSRuntime.InvokeVoidAsync(identifier, args);
        }
        catch (JSException)
        {
            // mudScrollManager is undefined, so the MudBlazor script isn't loaded on the page.
            ScriptDiagnostics.LogMissingScriptOnce(_logger);
        }
        catch (JSDisconnectedException)
        {
        }
        catch (TaskCanceledException)
        {
        }
        catch (ObjectDisposedException)
        {
        }
    }
}
