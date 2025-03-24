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

        if (_observerManager.IsSubscribed(observer.ElementId))
        {
            _observerManager.Subscribe(observer.ElementId, observer);
        }
        else
        {
            await _pointerEventsNoneInterop.ListenForPointerEventsAsync(_dotNetObjectReference.Value, observer.ElementId, options, _cancellationToken);
            _observerManager.Subscribe(observer.ElementId, observer);
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

        if (!_observerManager.IsSubscribed(elementId))
            return;

        _observerManager.Unsubscribe(elementId);

        await _pointerEventsNoneInterop.CancelListenerAsync(elementId, _cancellationToken);
    }

    [JSInvokable]
    public Task RaiseOnPointerDown(string[] elementIds)
    {
        return _observerManager
            .NotifyAsync(
                notification: observer => observer.NotifyOnPointerDownAsync(EventArgs.Empty),
                predicate: (id, _) => elementIds.Contains(id));
    }

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
                _dotNetObjectReference.Value.Dispose();

            await _pointerEventsNoneInterop.DisposeAsync(CancellationToken.None);

            _cancellationTokenSource.Dispose();
        }
    }
}
