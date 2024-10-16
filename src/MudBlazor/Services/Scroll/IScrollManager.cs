// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Inject with the AddMudBlazorScrollServices extension.
/// </summary>
public interface IScrollManager
{
    ValueTask ScrollToAsync(string id, int left, int top, ScrollBehavior scrollBehavior);

    ValueTask ScrollIntoViewAsync(string selector, ScrollBehavior behavior);

    ValueTask ScrollToTopAsync(string id, ScrollBehavior scrollBehavior = ScrollBehavior.Auto);

    ValueTask ScrollToYearAsync(string elementId);

    ValueTask ScrollToListItemAsync(string elementId);

    ValueTask LockScrollAsync(string selector = "body", string cssClass = "scroll-locked");

    ValueTask UnlockScrollAsync(string selector = "body", string cssClass = "scroll-locked");

    ValueTask ScrollToBottomAsync(string elementId, ScrollBehavior scrollBehavior = ScrollBehavior.Auto);
}
