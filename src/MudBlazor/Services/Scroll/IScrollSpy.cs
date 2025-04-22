// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// Interface for spying on scroll events and managing scroll behavior for specified elements.
/// </summary>
public interface IScrollSpy : IAsyncDisposable
{
    /// <summary>
    /// Gets the current position of the centered section.
    /// </summary>
    string? CenteredSection { get; }

    /// <summary>
    /// Occurs when a scroll section is centered.
    /// </summary>
    event EventHandler<ScrollSectionCenteredEventArgs>? ScrollSectionSectionCentered;

    /// <summary>
    /// Starts spying for scroll events for elements with the specified classes.
    /// </summary>
    /// <param name="containerSelector">The CSS selector to identify the scroll container.</param>
    /// <param name="sectionClassSelector">The CSS class (without .) to identify the section containers to spy on.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task StartSpying(string containerSelector, string sectionClassSelector);

    /// <summary>
    /// Centers the viewport to the DOM element with the given ID.
    /// </summary>
    /// <param name="id">The ID of the DOM element that should be centered.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ScrollToSection(string id);

    /// <summary>
    /// Centers the viewport to the DOM element represented by the fragment inside the URI.
    /// </summary>
    /// <param name="uri">The URI which contains the fragment. If no fragment is present, it scrolls to the top of the page.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ScrollToSection(Uri uri);

    /// <summary>
    /// Sets the section as active without scrolling. This can be used to initially set a value.
    /// </summary>
    /// <param name="id">The ID of the section to set as active.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetSectionAsActive(string id);
}
