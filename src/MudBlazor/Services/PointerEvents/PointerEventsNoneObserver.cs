// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable

/// <summary>
/// Represents a pointer events observer that handles pointer down and pointer up events for a specific HTML element.
/// </summary>
public class PointerEventsNoneObserver : IPointerEventsNoneObserver
{
    private readonly string _elementId;
    private readonly IPointerDownObserver _pointerDownObserver;
    private readonly IPointerUpObserver _pointerUpObserver;
    private static readonly PointerEventsObserverIgnore _ignore = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="PointerEventsNoneObserver"/> class
    /// </summary>
    /// <param name="elementId">The unique identifier of the HTML element to observe.</param>
    /// <param name="pointerDownObserver">The observer for pointer down events.</param>
    /// <param name="pointerUpObserver">The observer for pointer up events.</param>
    internal PointerEventsNoneObserver(string elementId, IPointerDownObserver? pointerDownObserver, IPointerUpObserver? pointerUpObserver)
    {
        _elementId = elementId;
        _pointerDownObserver = pointerDownObserver ?? _ignore;
        _pointerUpObserver = pointerUpObserver ?? _ignore;
    }

    /// <inheritdoc />
    string IPointerEventsNoneObserver.ElementId => _elementId;

    /// <inheritdoc />
    Task IPointerDownObserver.NotifyOnPointerDownAsync(EventArgs args) => _pointerDownObserver.NotifyOnPointerDownAsync(args);

    /// <inheritdoc />
    Task IPointerUpObserver.NotifyOnPointerUpAsync(EventArgs args) => _pointerUpObserver.NotifyOnPointerUpAsync(args);

    /// <summary>
    /// Gets a <see cref="IPointerDownObserver"/> that ignores pointer down events.
    /// </summary>
    /// <returns>An instance of <see cref="IPointerDownObserver"/> that ignores pointer down events.</returns>
    public static IPointerDownObserver PointerDownIgnore() => _ignore;

    /// <summary>
    /// Gets a <see cref="IPointerUpObserver"/> that ignores pointer up events.
    /// </summary>
    /// <returns>An instance of <see cref="IPointerDownObserver"/> that ignores pointer down events.</returns>
    public static IPointerUpObserver PointerUpIgnore() => _ignore;

    private sealed class PointerEventsObserverIgnore : IPointerDownObserver, IPointerUpObserver;
}
