// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor.Interop;
using MudBlazor.Utilities.ObserverManager;

namespace MudBlazor;

#nullable enable

/// <inheritdoc />
/// <remarks>
/// This implementation uses JavaScript interop to globally listen for pointer events such as pointer down and up.
/// Since elements with <c>pointer-events: none</c> do not receive events normally, the interop captures these events
/// and checks if they occurred over any registered element IDs. Matching observers are then notified in C#.
///
/// This allows you to make elements with disabled pointer interaction still participate in interaction logic,
/// such as overlays or custom render layers.
/// </remarks>
internal sealed class PointerEventsNoneService : IPointerEventsNoneService
{
    private bool _disposed;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CancellationToken _cancellationToken;
    private readonly PointerEventsNoneInterop _pointerEventsNoneInterop;
    private readonly ObserverManager<string, IPointerEventsNoneObserver> _observerManager;
    private readonly Lazy<DotNetObjectReference<PointerEventsNoneService>> _dotNetObjectReference;

    /// <summary>
    /// Gets the number of observers.
    /// </summary>
    /// <remarks>
    /// This property is not exposed in the public API of the <see cref="IPointerEventsNoneService"/> interface and is intended for internal use only.
    /// </remarks>
    internal int ObserversCount => _observerManager.Count;

    [DynamicDependency(nameof(RaiseOnPointerDown))]
    [DynamicDependency(nameof(RaiseOnPointerUp))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(PointerEventsNoneOptions))]
    public PointerEventsNoneService(ILogger<PointerEventsNoneService> logger, IJSRuntime jSRuntime)
    {
        _cancellationTokenSource = new();
        _cancellationToken = _cancellationTokenSource.Token;
        _pointerEventsNoneInterop = new(jSRuntime);
        _observerManager = new(logger);
        _dotNetObjectReference = new(() => DotNetObjectReference.Create(this));
    }

    /// <inheritdoc />
    public async Task SubscribeAsync(IPointerEventsNoneObserver observer, PointerEventsNoneOptions options)
    {
        ArgumentNullException.ThrowIfNull(observer);

        if (!_observerManager.TryGetOrAddSubscription(observer.ElementId, observer, out var newObserver))
        {
            await _pointerEventsNoneInterop.ListenForPointerEventsAsync(_dotNetObjectReference.Value, newObserver.ElementId, options, _cancellationToken);
        }
    }

    /// <inheritdoc />
    public Task SubscribeAsync(string elementId, PointerEventsNoneOptions options, IPointerDownObserver? pointerDown = null, IPointerUpObserver? pointerUp = null)
    {
        return SubscribeAsync(new PointerEventsNoneObserver(elementId, pointerDown, pointerUp), options);
    }

    /// <inheritdoc />
    public Task UnsubscribeAsync(IPointerEventsNoneObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        if (_disposed)
        {
            return Task.CompletedTask;
        }

        return UnsubscribeAsync(observer.ElementId);
    }

    /// <inheritdoc />
    public async Task UnsubscribeAsync(string elementId)
    {
        ArgumentNullException.ThrowIfNull(elementId);

        if (_disposed)
        {
            return;
        }

        _observerManager.Unsubscribe(elementId);

        await _pointerEventsNoneInterop.CancelListenerAsync(elementId, _cancellationToken);
    }

    /// <summary>
    /// Notifies observers when a pointer down event occurs over one or more of the specified HTML elements.
    /// This method is invoked from JavaScript via interop.
    /// </summary>
    /// <param name="elementIds">An array of element IDs for which the pointer down event was detected.</param>
    /// <returns>A task representing the asynchronous notification operation.</returns>
    /// <remarks>
    /// This method is not exposed in the public API of the <see cref="IPointerEventsNoneService"/> interface and is intended for internal use only.
    /// </remarks>
    [JSInvokable]
    public Task RaiseOnPointerDown(string[] elementIds)
    {
        return _observerManager
            .NotifyAsync(
                notification: observer => observer.NotifyOnPointerDownAsync(EventArgs.Empty),
                predicate: (id, _) => elementIds.Contains(id));
    }

    /// <summary>
    /// Notifies observers when a pointer up event occurs on any of the specified HTML elements.
    /// This method is invoked from JavaScript via interop.
    /// </summary>
    /// <param name="elementIds">An array of HTML element IDs that received the pointer up event.</param>
    /// <returns>A task representing the asynchronous notification operation.</returns>
    /// <remarks>
    /// This method is not exposed in the public API of the <see cref="IPointerEventsNoneService"/> interface and is intended for internal use only.
    /// It is called by the JavaScript layer when a pointer up event is detected globally over elements with <c>pointer-events: none</c>.
    /// </remarks>
    [JSInvokable]
    public Task RaiseOnPointerUp(string[] elementIds)
    {
        return _observerManager
            .NotifyAsync(
                notification: observer => observer.NotifyOnPointerUpAsync(EventArgs.Empty),
                predicate: (id, _) => elementIds.Contains(id));
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            _disposed = true;

            await _cancellationTokenSource.CancelAsync();

            _observerManager.Clear();

            if (_dotNetObjectReference.IsValueCreated)
            {
                _dotNetObjectReference.Value.Dispose();
            }

            await _pointerEventsNoneInterop.DisposeAsync(CancellationToken.None);

            _cancellationTokenSource.Dispose();
        }
    }
}
