// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.JSInterop;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Implementation of <see cref="IEventListener"/> for listening to JavaScript events.
    /// </summary>
    internal sealed class EventListener : IEventListener
    {
        private bool _disposed;
        private readonly IJSRuntime _jsRuntime;
        private readonly DotNetObjectReference<EventListener> _dotNetRef;

        private readonly Dictionary<Guid, (Type eventType, Func<object, Task>? callback)> _callbackResolver = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="EventListener"/> class.
        /// </summary>
        /// <param name="runtime">The JavaScript runtime.</param>
        [DynamicDependency(nameof(OnEventOccur))]
        public EventListener(IJSRuntime runtime)
        {
            _jsRuntime = runtime;
            _dotNetRef = DotNetObjectReference.Create(this);
        }

        /// <summary>
        /// Invoked by JavaScript when an event occurs.
        /// </summary>
        /// <param name="key">The unique event identifier.</param>
        /// <param name="eventData">The event data in JSON format.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [JSInvokable]
        public async Task OnEventOccur(Guid key, string eventData)
        {
            if (_callbackResolver.TryGetValue(key, out var element) && element.callback != null)
            {
                var @event = JsonSerializer.Deserialize(eventData, element.eventType, WebEventJsonContext.Default);
                if (@event is not null)
                {
                    await element.callback.Invoke(@event);
                }
            }
        }

        /// <inheritdoc />
        public async Task<Guid> Subscribe<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, string elementId, string projectionName, int throttleInterval, Func<object, Task> callback)
        {
            var (type, properties) = GetTypeInformation<T>();
            var key = RegisterCallBack(type, callback);

            await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudThrottledEventManager.subscribe", eventName, elementId, projectionName, throttleInterval, key, properties, _dotNetRef);

            return key;
        }

        /// <inheritdoc />
        public async Task<Guid> SubscribeGlobal<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, int throttleInterval, Func<object, Task> callback)
        {
            var typeInformation = GetTypeInformation<T>();
            var key = RegisterCallBack(typeInformation.Type, callback);

            await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudThrottledEventManager.subscribeGlobal", eventName, throttleInterval, key, typeInformation.Properties, _dotNetRef);

            return key;
        }

        /// <inheritdoc />
        public async Task<bool> Unsubscribe(Guid key)
        {
            if (!_callbackResolver.ContainsKey(key))
            {
                return false;
            }

            var result = await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudThrottledEventManager.unsubscribe", key);

            return result;
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                _disposed = true;

                foreach (var item in _callbackResolver)
                {
                    await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudThrottledEventManager.unsubscribe", item.Key);
                }

                _dotNetRef.Dispose();
            }
        }

        /// <summary>
        /// Registers a callback for the specified type.
        /// </summary>
        /// <param name="type">The type of the event.</param>
        /// <param name="callback">The callback to register.</param>
        /// <returns>A unique identifier for the callback.</returns>
        private Guid RegisterCallBack(Type type, Func<object, Task> callback)
        {
            var key = Guid.NewGuid();
            _callbackResolver.Add(key, (type, callback));

            return key;
        }

        /// <summary>
        /// Gets type information for the specified type.
        /// </summary>
        /// <typeparam name="T">The type to get information for.</typeparam>
        /// <returns>A tuple containing the type and its properties.</returns>
        private static (Type Type, string[] Properties) GetTypeInformation<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>()
        {
            var type = typeof(T);
            var properties = type.GetProperties().Select(x => char.ToLower(x.Name[0]) + x.Name.Substring(1)).ToArray();

            return (type, properties);
        }
    }
}
