using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Utility class for opting out of rerendering in Blazor when an EventCallback is invoked.
/// By default, components inherit from ComponentBase, which automatically invokes StateHasChanged
/// after the component's event handlers are invoked. In some cases, it might be unnecessary or
/// undesirable to trigger a rerender after an event handler is invoked. For example, an event
/// handler might not modify the component state.
/// https://learn.microsoft.com/aspnet/core/blazor/performance?view=aspnetcore-6.0#avoid-rerendering-after-handling-events-without-state-changes
/// </summary>
public static class EventUtil
{
    /// <summary>
    /// Converts the provided <see cref="Action"/> callback into a non-rendering event handler.
    /// </summary>
    /// <param name="component">The component that handles exceptions.</param>
    /// <param name="callback">The action callback to be converted.</param>
    /// <returns>A non-rendering event handler.</returns>
    public static Action AsNonRenderingEventHandler(this ComponentBase component, Action callback)
        => new SyncReceiver(component, callback).Invoke;

    /// <summary>
    /// Converts the provided <see cref="Action{TValue}"/> callback into a non-rendering event handler.
    /// </summary>
    /// <typeparam name="TValue">The type of the callback argument.</typeparam>
    /// <param name="callback">The action callback to be converted.</param>
    /// <param name="component">The component that handles exceptions.</param>
    /// <returns>A non-rendering event handler.</returns>
    public static Action<TValue> AsNonRenderingEventHandler<TValue>(this ComponentBase component, Action<TValue> callback)
        => new SyncReceiver<TValue>(component, callback).Invoke;

    /// <summary>
    /// Converts the provided <see cref="Func{Task}"/> callback into a non-rendering event handler.
    /// </summary>
    /// <param name="callback">The asynchronous callback to be converted.</param>
    /// <param name="component">The component that handles exceptions.</param>
    /// <returns>A non-rendering event handler.</returns>
    public static Func<Task> AsNonRenderingEventHandler(this ComponentBase component, Func<Task> callback)
        => new AsyncReceiver(component, callback).Invoke;

    /// <summary>
    /// Converts the provided <see cref="Func{TValue, Task}"/> callback into a non-rendering event handler.
    /// </summary>
    /// <typeparam name="TValue">The type of the callback argument.</typeparam>
    /// <param name="callback">The asynchronous callback to be converted.</param>
    /// <param name="component">The component that handles exceptions.</param>
    /// <returns>A non-rendering event handler.</returns>
    public static Func<TValue, Task> AsNonRenderingEventHandler<TValue>(this ComponentBase component, Func<TValue, Task> callback)
        => new AsyncReceiver<TValue>(component, callback).Invoke;

    private sealed class SyncReceiver(ComponentBase component, Action callback) : ReceiverBase(component)
    {
        public void Invoke() => callback();
    }

    private sealed class SyncReceiver<T>(ComponentBase component, Action<T> callback) : ReceiverBase(component)
    {
        public void Invoke(T arg) => callback(arg);
    }

    private sealed class AsyncReceiver(ComponentBase component, Func<Task> callback) : ReceiverBase(component)
    {
        public Task Invoke() => callback();
    }

    private sealed class AsyncReceiver<T>(ComponentBase component, Func<T, Task> callback) : ReceiverBase(component)
    {
        public Task Invoke(T arg) => callback(arg);
    }

    private abstract class ReceiverBase(ComponentBase component) : IHandleEvent
    {
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_renderHandle")]
        private static extern ref RenderHandle RenderHandle(ComponentBase component);

        public async Task HandleEventAsync(EventCallbackWorkItem item, object? arg)
        {
            try
            {
                await item.InvokeAsync(arg);
            }
            catch (Exception ex)
            {
                await RenderHandle(component).DispatchExceptionAsync(ex);
            }
        }
    }
}
