// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public class EventListener : IEventListener, IDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly DotNetObjectReference<EventListener> _dotNetRef;
        private bool _disposed;

        private readonly Dictionary<Guid, (Type eventType, Func<object, Task> callback)> _callbackResolver = new();

        [DynamicDependency(nameof(OnEventOccur))]
        public EventListener(IJSRuntime runtime)
        {
            _jsRuntime = runtime;
            _dotNetRef = DotNetObjectReference.Create(this);
        }

        [JSInvokable]
        public async Task OnEventOccur(Guid key, string @eventData)
        {
            if (_callbackResolver.TryGetValue(key, out var element) && element.callback != null)
            {
                var @event = JsonSerializer.Deserialize(eventData, element.eventType, WebEventJsonContext.Default);
                await element.callback.Invoke(@event);
            }
        }

        public async Task<Guid> Subscribe<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, string elementId, string projectionName, int throotleInterval, Func<object, Task> callback)
        {
            var (type, properties) = GetTypeInformation<T>();
            var key = RegisterCallBack(type, callback);

            await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudThrottledEventManager.subscribe", eventName, elementId, projectionName, throotleInterval, key, properties, _dotNetRef);

            return key;
        }

        public async Task<Guid> SubscribeGlobal<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(string eventName, int throotleInterval, Func<object, Task> callback)
        {
            var (type, properties) = GetTypeInformation<T>();
            var key = RegisterCallBack(type, callback);

            await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudThrottledEventManager.subscribeGlobal", eventName, throotleInterval, key, properties, _dotNetRef);

            return key;
        }

        public async Task<bool> Unsubscribe(Guid key)
        {
            if (_callbackResolver.ContainsKey(key) == false) { return false; }

            try
            {
                await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudThrottledEventManager.unsubscribe", key);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private (Type Type, string[] Properties) GetTypeInformation<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>()
        {
            var type = typeof(T);
            var properties = type.GetProperties().Select(x => char.ToLower(x.Name[0]) + x.Name.Substring(1)).ToArray();

            return (type, properties);
        }

        private Guid RegisterCallBack(Type type, Func<object, Task> callback)
        {
            var key = Guid.NewGuid();
            _callbackResolver.Add(key, (type, callback));

            return key;
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) { return; }

            foreach (var item in _callbackResolver)
            {
                try
                {
                    await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudThrottledEventManager.unsubscribe", item.Key);
                }
                catch (Exception)
                {
                    //ignore
                }
            }

            _dotNetRef.Dispose();
            _disposed = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dotNetRef.Dispose();

                    foreach (var item in _callbackResolver)
                    {
                        try
                        {
                            _jsRuntime.InvokeVoidAsync("mudThrottledEventManager.unsubscribe", item.Key);
                        }
                        catch (Exception)
                        {
                            //ignore
                        }
                    }
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
