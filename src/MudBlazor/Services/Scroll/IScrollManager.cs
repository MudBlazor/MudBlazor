// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// Provides methods for managing scroll behavior.
/// </summary>
public interface IScrollManager
{
    /// <summary>
    /// Scrolls to the specified position within the element with the given ID.
    /// </summary>
    /// <param name="id">The ID of the element to scroll.</param>
    /// <param name="left">The horizontal scroll position.</param>
    /// <param name="top">The vertical scroll position.</param>
    /// <param name="scrollBehavior">The scroll behavior (e.g., smooth or auto).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask ScrollToAsync(string? id, int left, int top, ScrollBehavior scrollBehavior);

    /// <summary>
    /// Scrolls the element matching the specified selector into view.
    /// </summary>
    /// <param name="selector">The CSS selector of the element to scroll into view.</param>
    /// <param name="behavior">The scroll behavior (e.g., smooth or auto).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask ScrollIntoViewAsync(string? selector, ScrollBehavior behavior);

    /// <summary>
    /// Scrolls to the top of the element with the given ID.
    /// </summary>
    /// <param name="id">The ID of the element to scroll.</param>
    /// <param name="scrollBehavior">The scroll behavior (e.g., smooth or auto). Defaults to auto.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask ScrollToTopAsync(string? id, ScrollBehavior scrollBehavior = ScrollBehavior.Auto);

    /// <summary>
    /// Scrolls to the year element with the given ID.
    /// </summary>
    /// <param name="elementId">The ID of the year element to scroll.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask ScrollToYearAsync(string elementId);

    /// <summary>
    /// Scrolls to the list item element with the given ID.
    /// </summary>
    /// <param name="elementId">The ID of the list item element to scroll.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask ScrollToListItemAsync(string elementId);

    /// <summary>
    /// Locks the scroll for the element matching the specified selector by adding a CSS class.
    /// </summary>
    /// <param name="selector">The CSS selector of the element to lock scroll. Defaults to "body".</param>
    /// <param name="cssClass">The CSS class to add to lock scroll. Defaults to "scroll-locked".</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask LockScrollAsync(string selector = "body", string cssClass = "scroll-locked");

    /// <summary>
    /// Unlocks the scroll for the element matching the specified selector by removing a CSS class.
    /// </summary>
    /// <param name="selector">The CSS selector of the element to unlock scroll. Defaults to "body".</param>
    /// <param name="cssClass">The CSS class to remove to unlock scroll. Defaults to "scroll-locked".</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask UnlockScrollAsync(string selector = "body", string cssClass = "scroll-locked");

    /// <summary>
    /// Scrolls to the bottom of the element with the given ID.
    /// </summary>
    /// <param name="elementId">The ID of the element to scroll.</param>
    /// <param name="scrollBehavior">The scroll behavior (e.g., smooth or auto). Defaults to auto.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask ScrollToBottomAsync(string elementId, ScrollBehavior scrollBehavior = ScrollBehavior.Auto);
}
