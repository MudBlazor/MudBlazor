using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;

namespace MudBlazor.UnitTests.Mocks;

#pragma warning disable CS0618 // Type or member is obsolete
public class MockKeyInterceptorService : IKeyInterceptor, IKeyInterceptorService
#pragma warning restore CS0618 // Type or member is obsolete
{
    public void Dispose()
    {
    }

    public Task Connect(string element, KeyInterceptorOptions options) => Task.CompletedTask;

    public Task Disconnect() => Task.CompletedTask;

    public Task UpdateKey(KeyOptions option) => Task.CompletedTask;

#pragma warning disable CS0067
    public event KeyboardEvent KeyDown;
    public event KeyboardEvent KeyUp;
#pragma warning restore CS0067

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public Task SubscribeAsync(IKeyInterceptorObserver observer, KeyInterceptorOptions options) => Task.CompletedTask;

    public Task SubscribeAsync(string elementId, KeyInterceptorOptions options, IKeyDownObserver keyDown = null, IKeyUpObserver keyUp = null) => Task.CompletedTask;

    public Task SubscribeAsync(string elementId, KeyInterceptorOptions options, Action<KeyboardEventArgs> keyDown = null, Action<KeyboardEventArgs> keyUp = null) => Task.CompletedTask;

    public Task SubscribeAsync(string elementId, KeyInterceptorOptions options, Func<KeyboardEventArgs, Task> keyDown = null, Func<KeyboardEventArgs, Task> keyUp = null) => Task.CompletedTask;

    public Task UpdateKeyAsync(IKeyInterceptorObserver observer, KeyOptions option) => Task.CompletedTask;

    public Task UpdateKeyAsync(string elementId, KeyOptions option) => Task.CompletedTask;

    public Task UnsubscribeAsync(IKeyInterceptorObserver observer) => Task.CompletedTask;

    public Task UnsubscribeAsync(string elementId) => Task.CompletedTask;
}
