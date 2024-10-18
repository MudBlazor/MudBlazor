// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// Provides data for the <see cref="IScrollSpy.ScrollSectionSectionCentered"/> event.
/// </summary>
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
