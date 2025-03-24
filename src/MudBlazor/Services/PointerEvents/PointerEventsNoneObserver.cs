// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable

public class PointerEventsNoneObserver : IPointerEventsNoneObserver
{
    private readonly string _elementId;
    private readonly IPointerDownObserver _pointerDownObserver;
    private readonly IPointerUpObserver _pointerUpObserver;
    private static readonly PointerEventsObserverIgnore _ignore = new();

    public PointerEventsNoneObserver(string elementId, IPointerDownObserver? pointerDownObserver, IPointerUpObserver? pointerUpObserver)
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

    public static IPointerDownObserver PointerDownIgnore() => _ignore;

    public static IPointerUpObserver PointerUpIgnore() => _ignore;

    private class PointerEventsObserverIgnore : IPointerDownObserver, IPointerUpObserver;
}
