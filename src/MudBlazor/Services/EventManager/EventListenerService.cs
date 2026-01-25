// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor.Utilities.ObserverManager;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents a service for listening to JavaScript events.
/// </summary>
/// <remarks>
/// This service supports multiple concurrent subscriptions and can be used to listen to DOM events
/// on specific elements or globally on the document. Subscriptions are automatically cleaned up when
/// the service is disposed.
/// </remarks>
internal sealed class EventListenerService : IEventListenerService
{
    private bool _disposed;
    private readonly IJSRuntime _jsRuntime;
    private readonly Lazy<DotNetObjectReference<EventListenerService>> _dotNetReferenceLazy;
    private readonly ObserverManager<EventListenerSubscription, IEventListenerObserver> _observerManager;

    /// <summary>
    /// Gets the number of active subscriptions.
    /// </summary>
    /// <remarks>
    /// This property is not exposed in the public API of the <see cref="IEventListenerService"/> interface and is intended for internal use only.
    /// </remarks>
    internal int SubscriptionCount => _observerManager.Count;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventListenerService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="jsRuntime">The JavaScript runtime.</param>
    [DynamicDependency(nameof(OnEventOccur))]
    public EventListenerService(ILogger<EventListenerService> logger, IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _observerManager = new ObserverManager<EventListenerSubscription, IEventListenerObserver>(logger);
        _dotNetReferenceLazy = new Lazy<DotNetObjectReference<EventListenerService>>(CreateDotNetObjectReference);
    }

    /// <summary>
    /// Invoked by JavaScript when an event occurs.
    /// </summary>
    /// <param name="subscriptionId">The unique subscription identifier.</param>
    /// <param name="eventData">The event data in JSON format.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// This method is not exposed in the public API of the <see cref="IEventListenerService"/> interface and is intended to be used internally from JavaScript.
    /// </remarks>
    [JSInvokable]
    public async Task OnEventOccur(Guid subscriptionId, string eventData)
    {
        // Find the subscription to get the event type
        var subscription = _observerManager
            .FindObserverIdentities((key, _) => key.SubscriptionId == subscriptionId)
            .FirstOrDefault();

        if (subscription != null)
        {
            var @event = JsonSerializer.Deserialize(eventData, subscription.EventType, WebEventJsonContext.Default);
            if (@event is not null)
            {
                await _observerManager.NotifyAsync(subscription, observer => observer.NotifyEventOccurredAsync(@event));
            }
        }
    }

    /// <inheritdoc />
    public async Task SubscribeAsync(IEventListenerObserver observer, string eventName, string elementId, string? projectionName, int throttleInterval, Type eventType, string[] eventProperties)
    {
        ArgumentNullException.ThrowIfNull(observer);

        if (_disposed)
        {
            return;
        }

        var subscription = new EventListenerSubscription(observer.SubscriptionId, eventType);

        if (!_observerManager.TryGetOrAddSubscription(subscription, observer, out _))
        {
            await _jsRuntime.InvokeVoidAsyncWithErrorHandling(
                "mudThrottledEventManager.subscribe",
                eventName,
                elementId,
                projectionName,
                throttleInterval,
                observer.SubscriptionId,
                eventProperties,
                _dotNetReferenceLazy.Value);
        }
    }

    /// <inheritdoc />
    public Task SubscribeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Guid subscriptionId, string eventName, string elementId, string? projectionName, int throttleInterval, Func<object, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        var (type, properties) = GetTypeInformation<T>();
        var observer = new EventListenerLambdaTaskObserver(subscriptionId, callback);

        return SubscribeAsync(observer, eventName, elementId, projectionName, throttleInterval, type, properties);
    }

    /// <inheritdoc />
    public Task SubscribeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Guid subscriptionId, string eventName, string elementId, string? projectionName, int throttleInterval, Action<object> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        var (type, properties) = GetTypeInformation<T>();
        var observer = new EventListenerLambdaObserver(subscriptionId, callback);

        return SubscribeAsync(observer, eventName, elementId, projectionName, throttleInterval, type, properties);
    }

    /// <inheritdoc />
    public async Task SubscribeGlobalAsync(IEventListenerObserver observer, string eventName, int throttleInterval, Type eventType, string[] eventProperties)
    {
        ArgumentNullException.ThrowIfNull(observer);

        if (_disposed)
        {
            return;
        }

        var subscription = new EventListenerSubscription(observer.SubscriptionId, eventType);

        if (!_observerManager.TryGetOrAddSubscription(subscription, observer, out _))
        {
            await _jsRuntime.InvokeVoidAsyncWithErrorHandling(
                "mudThrottledEventManager.subscribeGlobal",
                eventName,
                throttleInterval,
                observer.SubscriptionId,
                eventProperties,
                _dotNetReferenceLazy.Value);
        }
    }

    /// <inheritdoc />
    public Task SubscribeGlobalAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Guid subscriptionId, string eventName, int throttleInterval, Func<object, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        var (type, properties) = GetTypeInformation<T>();
        var observer = new EventListenerLambdaTaskObserver(subscriptionId, callback);

        return SubscribeGlobalAsync(observer, eventName, throttleInterval, type, properties);
    }

    /// <inheritdoc />
    public Task SubscribeGlobalAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Guid subscriptionId, string eventName, int throttleInterval, Action<object> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        var (type, properties) = GetTypeInformation<T>();
        var observer = new EventListenerLambdaObserver(subscriptionId, callback);

        return SubscribeGlobalAsync(observer, eventName, throttleInterval, type, properties);
    }

    /// <inheritdoc />
    public Task UnsubscribeAsync(IEventListenerObserver observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        return UnsubscribeAsync(observer.SubscriptionId);
    }

    /// <inheritdoc />
    public async Task UnsubscribeAsync(Guid subscriptionId)
    {
        if (_disposed)
        {
            return;
        }

        var subscription = _observerManager
            .FindObserverIdentities((key, _) => key.SubscriptionId == subscriptionId)
            .FirstOrDefault();

        if (subscription != null)
        {
            _observerManager.Unsubscribe(subscription);
            await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudThrottledEventManager.unsubscribe", subscriptionId);
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            _disposed = true;

            foreach (var observer in _observerManager)
            {
                await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudThrottledEventManager.unsubscribe", observer.SubscriptionId);
            }

            _observerManager.Clear();

            if (_dotNetReferenceLazy.IsValueCreated)
            {
                _dotNetReferenceLazy.Value.Dispose();
            }
        }
    }

    /// <summary>
    /// Gets type information for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to get information for.</typeparam>
    /// <returns>A tuple containing the type and its properties.</returns>
    private static (Type Type, string[] Properties) GetTypeInformation<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>()
    {
        var type = typeof(T);
        var properties = type.GetProperties().Select(x => char.ToLower(x.Name[0]) + x.Name[1..]).ToArray();

        return (type, properties);
    }

    private DotNetObjectReference<EventListenerService> CreateDotNetObjectReference() => DotNetObjectReference.Create(this);
}
