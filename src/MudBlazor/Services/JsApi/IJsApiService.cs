// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Provides JavaScript API services for various browser operations.
/// </summary>
public interface IJsApiService
{
    /// <summary>
    /// Copies the specified text to the clipboard.
    /// </summary>
    /// <param name="text">The text to copy to the clipboard.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask CopyToClipboardAsync(string text);

    /// <summary>
    /// Opens the specified URL in a new browser tab.
    /// </summary>
    /// <param name="url">The URL to open in a new tab.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask OpenInNewTabAsync(string url);

    /// <summary>
    /// Opens the specified link in the specified target.
    /// </summary>
    /// <param name="link">The link to open.</param>
    /// <param name="target">The target where the link should be opened (e.g., "_blank", "_self").</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask Open(string link, string target);

    /// <summary>
    /// Updates the specified style property of an HTML element.
    /// </summary>
    /// <param name="elementId">The ID of the HTML element.</param>
    /// <param name="propertyName">The name of the style property to update.</param>
    /// <param name="value">The new value for the style property.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    internal ValueTask UpdateStyleProperty(string elementId, string propertyName, object value);
}
