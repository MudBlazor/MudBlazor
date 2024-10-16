// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

public interface IScrollSpy : IAsyncDisposable
{
    event EventHandler<ScrollSectionCenteredEventArgs> ScrollSectionSectionCentered;

    /// <summary>
    /// Start spying for scroll events for elements with the specified classes
    /// </summary>
    /// <param name="containerSelector">the CSS selector to identify the scroll container</param>
    /// <param name="sectionClassSelector">the CSS class (without .) to identify the section containers to spy on</param>
    /// <returns></returns>
    public Task StartSpying(string containerSelector, string sectionClassSelector);

    /// <summary>
    /// Center the viewport to DOM element with the given Id 
    /// </summary>
    /// <param name="id">The Id of the DOM element, that should be centered</param>
    /// <returns></returns>
    Task ScrollToSection(string id);

    /// <summary>
    /// Center the viewport to the DOM element represented by the fragment inside the uri
    /// </summary>
    /// <param name="uri">The uri which contains the fragment. If no fragment it scrolls to the top of the page</param>
    /// <returns></returns>
    Task ScrollToSection(Uri uri);

    /// <summary>
    /// Does the same as ScrollToSection but without the scrolling. This can be used to initially set an value
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task SetSectionAsActive(string id);

    /// <summary>
    /// Get the current position of the centered section
    /// </summary>
    string CenteredSection { get; }
}
