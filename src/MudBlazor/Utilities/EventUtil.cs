using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;

namespace MudBlazor;

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

        // Only the renderer knows a component's parent, and UnsafeAccessor has to name its type because reflection is not trim-safe.
#pragma warning disable BL0006
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_renderer")]
        private static extern ref Renderer? GetRenderer(ref RenderHandle renderHandle);

        // This reads the field behind GetComponentState because Mono on .NET 8 cannot bind that overloaded method.
        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_componentStateByComponent")]
        private static extern ref Dictionary<IComponent, ComponentState> GetComponentStates(Renderer renderer);
#pragma warning restore BL0006

        public async Task HandleEventAsync(EventCallbackWorkItem item, object? arg)
        {
            // The handler can remove the component from the tree, so find its parent while the renderer still knows it.
            var parent = FindParent(component);

            try
            {
                await item.InvokeAsync(arg);
            }
            catch (Exception ex)
            {
                if (await TryDispatchExceptionAsync(component, ex))
                {
                    return;
                }

                // The renderer ignores a canceled handler, so a cancellation is rethrown as before instead of reaching an ancestor.
                if (ex is not OperationCanceledException)
                {
                    // A removed component can no longer route the exception, so it goes to the nearest ancestor still in the tree.
                    for (var state = parent; state is not null; state = state.LogicalParentComponentState)
                    {
                        if (state.Component is ComponentBase ancestor && await TryDispatchExceptionAsync(ancestor, ex))
                        {
                            return;
                        }
                    }
                }

                ExceptionDispatchInfo.Capture(ex).Throw();
            }

            static ComponentState? FindParent(ComponentBase component)
            {
                // This runs before every handler, so a change to these framework internals must cost only the error routing, not the click.
                try
                {
                    var renderer = GetRenderer(ref RenderHandle(component));
                    return renderer is not null && GetComponentStates(renderer).TryGetValue(component, out var state)
                        ? state.LogicalParentComponentState
                        : null;
                }
                catch
                {
                    return null;
                }
            }

            static async Task<bool> TryDispatchExceptionAsync(ComponentBase target, Exception exception)
            {
                try
                {
                    await RenderHandle(target).DispatchExceptionAsync(exception);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
