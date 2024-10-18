// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.JSInterop;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Manages scroll behavior.
/// </summary>
internal sealed class ScrollManager : IScrollManager
{
    private readonly IJSRuntime _jSRuntime;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScrollManager"/> class with the specified JavaScript runtime.
    /// </summary>
    /// <param name="jSRuntime">The JavaScript runtime.</param>
    public ScrollManager(IJSRuntime jSRuntime)
    {
        _jSRuntime = jSRuntime;
    }

    /// <inheritdoc />
    public ValueTask ScrollToAsync(string? id, int left, int top, ScrollBehavior behavior) =>
        _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollTo", id, left, top, behavior.ToDescriptionString());

    /// <inheritdoc />
    public ValueTask ScrollIntoViewAsync(string? selector, ScrollBehavior behavior) =>
        _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollIntoView", selector, behavior.ToDescriptionString());

    /// <inheritdoc />
    public ValueTask ScrollToTopAsync(string? id, ScrollBehavior scrollBehavior = ScrollBehavior.Auto) =>
        ScrollToAsync(id, 0, 0, scrollBehavior);

    /// <inheritdoc />
    public ValueTask ScrollToBottomAsync(string id, ScrollBehavior behavior) =>
        _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollToBottom", id, behavior.ToDescriptionString());

    /// <inheritdoc />
    public ValueTask ScrollToYearAsync(string elementId) =>
        _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollToYear", elementId);

    /// <inheritdoc />
    public ValueTask ScrollToListItemAsync(string elementId) =>
        _jSRuntime.InvokeVoidAsync("mudScrollManager.scrollToListItem", elementId);

    /// <inheritdoc />
    public ValueTask LockScrollAsync(string selector = "body", string cssClass = "scroll-locked") =>
        _jSRuntime.InvokeVoidAsync("mudScrollManager.lockScroll", selector, cssClass);

    /// <inheritdoc />
    public ValueTask UnlockScrollAsync(string selector = "body", string cssClass = "scroll-locked") =>
        _jSRuntime.InvokeVoidAsyncIgnoreErrors("mudScrollManager.unlockScroll", selector, cssClass);
}
