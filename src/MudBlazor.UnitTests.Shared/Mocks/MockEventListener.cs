// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

    public Task<Guid> SubscribeAsync<T>(string eventName, string elementId, string projectionName, int throttleInterval, Func<object, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        var id = Guid.NewGuid();
        ElementIdMapper.Add(id, elementId);
        Callbacks.Add(id, callback);
        return Task.FromResult(id);
    }

    public Task<Guid> SubscribeAsync<T>(string eventName, string elementId, string projectionName, int throttleInterval, Action<object> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        return SubscribeAsync<T>(eventName, elementId, projectionName, throttleInterval, obj =>
        {
            callback(obj);
            return Task.CompletedTask;
        });
    }

    public Task<Guid> SubscribeGlobalAsync<T>(string eventName, int throttleInterval, Func<object, Task> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        var id = Guid.NewGuid();
        ElementIdMapper.Add(id, "document");
        Callbacks.Add(id, callback);
        return Task.FromResult(id);
    }

    public Task<Guid> SubscribeGlobalAsync<T>(string eventName, int throttleInterval, Action<object> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        return SubscribeGlobalAsync<T>(eventName, throttleInterval, obj =>
        {
            callback(obj);
            return Task.CompletedTask;
        });
    }

    public Task<bool> UnsubscribeAsync(Guid key)
    {
        var result = Callbacks.ContainsKey(key);
        if (result)
        {
            Callbacks.Remove(key);
            ElementIdMapper.Remove(key);
        }

        return Task.FromResult(result);
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
