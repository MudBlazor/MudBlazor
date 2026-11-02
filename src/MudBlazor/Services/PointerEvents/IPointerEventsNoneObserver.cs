// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;


/// <summary>
/// Receives pointer-down and pointer-up events for an HTML element styled <c>pointer-events: none</c>, relayed from JavaScript by the <see cref="IPointerEventsNoneService"/>.
/// </summary>
/// <remarks>
/// This observer is associated with a unique HTML element ID and is used by the
/// <see cref="IPointerEventsNoneService"/> to relay pointer interactions from JavaScript to .NET, even though the element itself does not natively receive pointer events.
/// </remarks>
public interface IPointerEventsNoneObserver : IPointerDownObserver, IPointerUpObserver
{
    /// <summary>
    /// Gets the unique ID of the HTML element associated with this observer.
    /// </summary>
    string ElementId { get; }
}
