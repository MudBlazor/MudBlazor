using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;

namespace MudBlazor.UnitTests.Shared.Mocks;

public class MockKeyInterceptorService : IKeyInterceptorService
{
    private readonly HashSet<string> _subscriptions = [];

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public Task SubscribeAsync(IKeyInterceptorObserver observer, KeyInterceptorOptions options)
    {
        _subscriptions.Add(observer.ElementId);
        return Task.CompletedTask;
    }

    public Task SubscribeAsync(string elementId, KeyInterceptorOptions options, Action<KeyMapBuilder> configure)
    {
        _subscriptions.Add(elementId);
        return Task.CompletedTask;
    }

    public Task SubscribeAsync(string elementId, KeyInterceptorOptions options, IKeyDownObserver? keyDown = null, IKeyUpObserver? keyUp = null)
    {
        _subscriptions.Add(elementId);
        return Task.CompletedTask;
    }

    public Task SubscribeAsync(string elementId, KeyInterceptorOptions options, Action<KeyboardEventArgs>? keyDown = null, Action<KeyboardEventArgs>? keyUp = null)
    {
        _subscriptions.Add(elementId);
        return Task.CompletedTask;
    }

    public Task SubscribeAsync(string elementId, KeyInterceptorOptions options, Func<KeyboardEventArgs, Task>? keyDown = null, Func<KeyboardEventArgs, Task>? keyUp = null)
    {
        _subscriptions.Add(elementId);
        return Task.CompletedTask;
    }

    public Task DispatchAsync(string elementId, KeyEventKind kind, KeyboardEventArgs args) => Task.CompletedTask;

    public bool IsSubscribed(string elementId) => _subscriptions.Contains(elementId);

    public Task UpdateKeyAsync(IKeyInterceptorObserver observer, KeyOptions option) => Task.CompletedTask;

    public Task UpdateKeyAsync(string elementId, KeyOptions option) => Task.CompletedTask;

    public Task UnsubscribeAsync(IKeyInterceptorObserver observer)
    {
        _subscriptions.Remove(observer.ElementId);
        return Task.CompletedTask;
    }

    public Task UnsubscribeAsync(string elementId)
    {
        _subscriptions.Remove(elementId);
        return Task.CompletedTask;
    }
}
