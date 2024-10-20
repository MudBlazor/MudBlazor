// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Services;

/// <summary>
/// Provides functionality to subscribe to JavaScript events of any HTML element by its ID.
/// </summary>
public interface IJsEvent : IAsyncDisposable
{
    /// <summary>
    /// Occurs when the caret position changes in an input element, either on click or keyup.
    /// </summary>
    event Action<int> CaretPositionChanged;

    /// <summary>
    /// Occurs when a paste action is performed.
    /// </summary>
    event Action<string> Paste;

    /// <summary>
    /// Occurs when a text selection action is performed.
    /// </summary>
    event Action<int, int> Select;

    /// <summary>
    /// Connects to the ancestor element of the element(s) that should be observed.
    /// </summary>
    /// <param name="elementId">The ID of the ancestor HTML element.</param>
    /// <param name="options">The options defining the descendants to be observed and the keystrokes to be monitored.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task Connect(string elementId, JsEventOptions options);

    /// <summary>
    /// Disconnects from the previously connected ancestor and its descendants.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task Disconnect();
}
