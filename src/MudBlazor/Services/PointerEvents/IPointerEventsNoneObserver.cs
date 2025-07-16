// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable

/// <summary>
/// Represents an observer that listens for pointer down and pointer up events
/// on a specific HTML element with <c>pointer-events: none</c>.
/// </summary>
/// <remarks>
/// This observer is associated with a unique HTML element ID and is used by the
/// <see cref="IPointerEventsNoneService"/> to relay pointer interactions from JavaScript
/// to .NET, even though the element itself does not natively receive pointer events.
/// </remarks>
public interface IPointerEventsNoneObserver : IPointerDownObserver, IPointerUpObserver
{
    /// <summary>
    /// Gets the unique ID of the HTML element associated with this observer.
    /// </summary>
    string ElementId { get; }
}
