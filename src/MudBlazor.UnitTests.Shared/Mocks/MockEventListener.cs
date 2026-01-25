// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.UnitTests.Shared.Mocks;

#nullable enable
/// <summary>
/// Mock implementation of <see cref="IEventListenerService"/> for testing purposes.
/// </summary>
public class MockEventListenerService : IEventListenerService
{
    public Dictionary<Guid, Func<object, Task>> Callbacks { get; } = new();

    public Dictionary<Guid, string> ElementIdMapper { get; } = new();

    public ValueTask DisposeAsync()
    {
        Callbacks.Clear();
        ElementIdMapper.Clear();
        return ValueTask.CompletedTask;
    }

    public Task SubscribeAsync(IEventListenerObserver observer, string eventName, string elementId, string? projectionName, int throttleInterval, Type eventType, string[] eventProperties)
    {
        ElementIdMapper.Add(observer.SubscriptionId, elementId);
        Callbacks.Add(observer.SubscriptionId, obj => observer.NotifyEventOccurredAsync(obj));
        return Task.CompletedTask;
    }

    public Task SubscribeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Guid subscriptionId, string eventName, string elementId, string? projectionName, int throttleInterval, Func<object, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        ElementIdMapper.Add(subscriptionId, elementId);
        Callbacks.Add(subscriptionId, callback);
        return Task.CompletedTask;
    }

    public Task SubscribeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Guid subscriptionId, string eventName, string elementId, string? projectionName, int throttleInterval, Action<object> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        return SubscribeAsync<T>(subscriptionId, eventName, elementId, projectionName, throttleInterval, obj =>
        {
            callback(obj);
            return Task.CompletedTask;
        });
    }

    public Task SubscribeGlobalAsync(IEventListenerObserver observer, string eventName, int throttleInterval, Type eventType, string[] eventProperties)
    {
        ElementIdMapper.Add(observer.SubscriptionId, "document");
        Callbacks.Add(observer.SubscriptionId, obj => observer.NotifyEventOccurredAsync(obj));
        return Task.CompletedTask;
    }

    public Task SubscribeGlobalAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Guid subscriptionId, string eventName, int throttleInterval, Func<object, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        ElementIdMapper.Add(subscriptionId, "document");
        Callbacks.Add(subscriptionId, callback);
        return Task.CompletedTask;
    }

    public Task SubscribeGlobalAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Guid subscriptionId, string eventName, int throttleInterval, Action<object> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        return SubscribeGlobalAsync<T>(subscriptionId, eventName, throttleInterval, obj =>
        {
            callback(obj);
            return Task.CompletedTask;
        });
    }

    public Task UnsubscribeAsync(IEventListenerObserver observer)
    {
        return UnsubscribeAsync(observer.SubscriptionId);
    }

    public Task UnsubscribeAsync(Guid subscriptionId)
    {
        Callbacks.Remove(subscriptionId);
        ElementIdMapper.Remove(subscriptionId);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Helper method for tests to fire events.
    /// </summary>
    internal void FireEvent(MouseEventArgs args)
    {
        foreach (var item in Callbacks.Values)
        {
            item.Invoke(args);
        }
    }
}

/// <summary>
/// Mock implementation of <see cref="IEventListener"/> for backward compatibility in tests.
/// </summary>
/// <remarks>
/// This class is deprecated and should not be used in new tests. Use <see cref="MockEventListenerService"/> instead.
/// </remarks>
[Obsolete("Use MockEventListenerService instead. This class is kept for backward compatibility.")]
public class MockEventListenerFactory : IEventListenerFactory
{
    private readonly MockEventListener? _listener;

    public MockEventListenerFactory(MockEventListener listener)
    {
        _listener = listener;
    }

    public MockEventListenerFactory()
    {
    }

    public IEventListener Create() => _listener ?? new MockEventListener();
}

/// <summary>
/// Mock implementation of <see cref="IEventListener"/> for backward compatibility in tests.
/// </summary>
/// <remarks>
/// This class is deprecated and should not be used in new tests. Use <see cref="MockEventListenerService"/> instead.
/// </remarks>
[Obsolete("Use MockEventListenerService instead. This class is kept for backward compatibility.")]
public class MockEventListener : IEventListener
{
    public Dictionary<Guid, Func<object, Task>> Callbacks { get; } = new();

    public Dictionary<Guid, string> ElementIdMapper { get; } = new();

    public ValueTask DisposeAsync()
    {
        Callbacks.Clear();
        ElementIdMapper.Clear();
        return ValueTask.CompletedTask;
    }

    public Task<Guid> Subscribe<T>(string eventName, string elementId, string projection, int throttleInterval, Func<object, Task> callback)
    {
        var id = Guid.NewGuid();
        ElementIdMapper.Add(id, elementId);
        Callbacks.Add(id, callback);
        return Task.FromResult(id);
    }

    public Task<Guid> SubscribeGlobal<T>(string eventName, int throotleInterval, Func<object, Task> callback)
    {
        var id = Guid.NewGuid();
        ElementIdMapper.Add(id, "document");
        Callbacks.Add(id, callback);
        return Task.FromResult(id);
    }

    public Task<bool> Unsubscribe(Guid key)
    {
        var result = Callbacks.ContainsKey(key);
        if (result)
        {
            Callbacks.Remove(key);
            ElementIdMapper.Remove(key);
        }

        return Task.FromResult(result);
    }

    internal void FireEvent(MouseEventArgs args)
    {
        foreach (var item in Callbacks.Values)
        {
            item.Invoke(args);
        }
    }
}
