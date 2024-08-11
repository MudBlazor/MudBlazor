using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
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
        /// <param name="callback">The action callback to be converted.</param>
        /// <returns>A non-rendering event handler.</returns>
        public static Action AsNonRenderingEventHandler(Action callback)
            => new SyncReceiver(callback).Invoke;

        /// <summary>
        /// Converts the provided <see cref="Action{TValue}"/> callback into a non-rendering event handler.
        /// </summary>
        /// <typeparam name="TValue">The type of the callback argument.</typeparam>
        /// <param name="callback">The action callback to be converted.</param>
        /// <returns>A non-rendering event handler.</returns>
        public static Action<TValue> AsNonRenderingEventHandler<TValue>(Action<TValue> callback)
            => new SyncReceiver<TValue>(callback).Invoke;

        /// <summary>
        /// Converts the provided <see cref="Func{Task}"/> callback into a non-rendering event handler.
        /// </summary>
        /// <param name="callback">The asynchronous callback to be converted.</param>
        /// <returns>A non-rendering event handler.</returns>
        public static Func<Task> AsNonRenderingEventHandler(Func<Task> callback)
            => new AsyncReceiver(callback).Invoke;

        /// <summary>
        /// Converts the provided <see cref="Func{TValue, Task}"/> callback into a non-rendering event handler.
        /// </summary>
        /// <typeparam name="TValue">The type of the callback argument.</typeparam>
        /// <param name="callback">The asynchronous callback to be converted.</param>
        /// <returns>A non-rendering event handler.</returns>
        public static Func<TValue, Task> AsNonRenderingEventHandler<TValue>(Func<TValue, Task> callback)
            => new AsyncReceiver<TValue>(callback).Invoke;

        private record SyncReceiver(Action Callback) : ReceiverBase
        {
            public void Invoke() => Callback();
        }

        private record SyncReceiver<T>(Action<T> Callback) : ReceiverBase
        {
            public void Invoke(T arg) => Callback(arg);
        }

        private record AsyncReceiver(Func<Task> Callback) : ReceiverBase
        {
            public Task Invoke() => Callback();
        }

        private record AsyncReceiver<T>(Func<T, Task> Callback) : ReceiverBase
        {
            public Task Invoke(T arg) => Callback(arg);
        }

        private record ReceiverBase : IHandleEvent
        {
            public Task HandleEventAsync(EventCallbackWorkItem item, object? arg)
                => item.InvokeAsync(arg);
        }
    }
}
