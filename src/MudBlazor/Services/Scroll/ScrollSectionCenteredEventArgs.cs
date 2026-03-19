// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Event payload emitted when the scroll spy identifies a new centered section.
/// </summary>
/// <remarks>
/// Consumers typically use this in navigation or highlighting logic to keep UI state in sync with the user's scroll position.
/// </remarks>
public class ScrollSectionCenteredEventArgs
{
    /// <summary>
    /// Gets the ID of the centered scroll section.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScrollSectionCenteredEventArgs"/> class with the specified section ID.
    /// </summary>
    /// <param name="id">The ID of the centered scroll section.</param>
    public ScrollSectionCenteredEventArgs(string id)
    {
        Id = id;
    }
}
