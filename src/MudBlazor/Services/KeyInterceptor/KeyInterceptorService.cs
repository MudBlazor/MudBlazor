// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor.Interop;
using MudBlazor.Services;
using MudBlazor.Utilities.ObserverManager;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents a service that intercepts key events for specified HTML elements.
/// </summary>
internal sealed class KeyInterceptorService : IKeyInterceptorService
{
    private bool _disposed;
    private readonly KeyInterceptorInterop _keyInterceptorInterop;
    private readonly ObserverManager<string, IKeyInterceptorObserver> _observerManager;
    private readonly Lazy<DotNetObjectReference<KeyInterceptorService>> _dotNetReferenceLazy;

    /// <summary>
    /// Gets the number of observers.
    /// </summary>
    /// <remarks>
    /// This property is not exposed in the public API of the <see cref="IKeyInterceptorService"/> interface and is intended for internal use only.
    /// </remarks>
    internal int ObserversCount => _observerManager.Count;

    [DynamicDependency(nameof(OnKeyDown))]
    [DynamicDependency(nameof(OnKeyUp))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(KeyboardEvent))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(KeyboardEventArgs))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(KeyOptions))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(KeyInterceptorOptions))]
    public KeyInterceptorService(ILogger<KeyInterceptorService> logger, IJSRuntime jsRuntime)
    {
        _keyInterceptorInterop = new KeyInterceptorInterop(jsRuntime);
        _observerManager = new ObserverManager<string, IKeyInterceptorObserver>(logger);
        _dotNetReferenceLazy = new Lazy<DotNetObjectReference<KeyInterceptorService>>(CreateDotNetObjectReference);
    }

    /// <inheritdoc />
    public async Task SubscribeAsync(IKeyInterceptorObserver observer, KeyInterceptorOptions options)
    {
        ArgumentNullException.ThrowIfNull(observer);

        if (_disposed)
        {
            return;
        }

        if (!_observerManager.Observers.ContainsKey(observer.ElementId))
        {
            var isConnected = await _keyInterceptorInterop.Connect(_dotNetReferenceLazy.Value, observer.ElementId, options);
            if (isConnected)
            {
                _observerManager.Subscribe(observer.ElementId, observer);
            }
        }
        else
        {
            _observerManager.Subscribe(observer.ElementId, observer);
        }
    }

    /// <inheritdoc />
    public Task SubscribeAsync(string elementId, KeyInterceptorOptions options, IKeyDownObserver? keyDown, IKeyUpObserver? keyUp)
    {
        return SubscribeAsync(new KeyObserver(elementId, keyDown, keyUp), options);
    }

    /// <inheritdoc />
    public Task SubscribeAsync(string elementId, KeyInterceptorOptions options, Action<KeyboardEventArgs>? keyDown, Action<KeyboardEventArgs>? keyUp)
    {
        return SubscribeAsync(new KeyObserver(elementId, KeyObserver.KeyDown(keyDown), KeyObserver.KeyUp(keyUp)), options);
    }

    /// <inheritdoc />
    public Task SubscribeAsync(string elementId, KeyInterceptorOptions options, Func<KeyboardEventArgs, Task>? keyDown, Func<KeyboardEventArgs, Task>? keyUp)
    {
        return SubscribeAsync(new KeyObserver(elementId, KeyObserver.KeyDown(keyDown), KeyObserver.KeyUp(keyUp)), options);
    }

    /// <inheritdoc />
    public Task UpdateKeyAsync(IKeyInterceptorObserver observer, KeyOptions option) => UpdateKeyAsync(observer.ElementId, option);

    /// <inheritdoc />
    public async Task UpdateKeyAsync(string elementId, KeyOptions option) => await _keyInterceptorInterop.UpdateKey(elementId, option);

    /// <inheritdoc />
    public Task UnsubscribeAsync(IKeyInterceptorObserver observer) => UnsubscribeAsync(observer.ElementId);

    /// <inheritdoc />
    public async Task UnsubscribeAsync(string elementId)
    {
        ArgumentNullException.ThrowIfNull(elementId);

        if (_disposed)
        {
            return;
        }

        _observerManager.Unsubscribe(elementId);

        await _keyInterceptorInterop.Disconnect(elementId);
    }

    /// <summary>
    /// Notifies observers when a key down event occurs for the specified HTML element and fires this method.
    /// This method is invoked from the JavaScript code.
    /// </summary>
    /// <param name="elementId">The unique identifier of the HTML element.</param>
    /// <param name="args">The <see cref="KeyboardEventArgs"/> representing the keyboard event arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method is not exposed in the public API of the <see cref="IKeyInterceptorService"/> interface and is intended to be used internally.
    /// </remarks>
    [JSInvokable]
    public Task OnKeyDown(string elementId, KeyboardEventArgs args)
    {
        return _observerManager.NotifyAsync(
            observer => observer.NotifyOnKeyDownAsync(args),
            predicate: (observerId, _) => observerId == elementId);
    }

    /// <summary>
    /// Notifies observers when a key up event occurs for the specified HTML element and fires this method.
    /// This method is invoked from the JavaScript code.
    /// </summary>
    /// <param name="elementId">The unique identifier of the HTML element.</param>
    /// <param name="args">The <see cref="KeyboardEventArgs"/> representing the keyboard event arguments.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method is not exposed in the public API of the <see cref="IKeyInterceptorService"/> interface and is intended to be used internally.
    /// </remarks>
    [JSInvokable]
    public Task OnKeyUp(string elementId, KeyboardEventArgs args)
    {
        return _observerManager.NotifyAsync(
            observer => observer.NotifyOnKeyUpAsync(args),
            predicate: (observerId, _) => observerId == elementId);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            _disposed = true;

            if (_dotNetReferenceLazy.IsValueCreated)
            {
                _dotNetReferenceLazy.Value.Dispose();
            }

            foreach (var elementId in _observerManager.Observers.Keys)
            {
                await _keyInterceptorInterop.Disconnect(elementId);
            }

            _observerManager.Clear();
        }
    }

    private DotNetObjectReference<KeyInterceptorService> CreateDotNetObjectReference() => DotNetObjectReference.Create(this);
}
