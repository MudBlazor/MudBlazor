using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;

namespace MudBlazor.UnitTests.Shared.Mocks;

public class MockKeyInterceptorService : IKeyInterceptorService
{
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public Task SubscribeAsync(IKeyInterceptorObserver observer, KeyInterceptorOptions options) => Task.CompletedTask;

    public Task SubscribeAsync(string elementId, KeyInterceptorOptions options, IKeyDownObserver? keyDown = null, IKeyUpObserver? keyUp = null) => Task.CompletedTask;

    public Task SubscribeAsync(string elementId, KeyInterceptorOptions options, Action<KeyboardEventArgs>? keyDown = null, Action<KeyboardEventArgs>? keyUp = null) => Task.CompletedTask;

    public Task SubscribeAsync(string elementId, KeyInterceptorOptions options, Func<KeyboardEventArgs, Task>? keyDown = null, Func<KeyboardEventArgs, Task>? keyUp = null) => Task.CompletedTask;

    public Task UpdateKeyAsync(IKeyInterceptorObserver observer, KeyOptions option) => Task.CompletedTask;

    public Task UpdateKeyAsync(string elementId, KeyOptions option) => Task.CompletedTask;

    public Task UnsubscribeAsync(IKeyInterceptorObserver observer) => Task.CompletedTask;

    public Task UnsubscribeAsync(string elementId) => Task.CompletedTask;
}
