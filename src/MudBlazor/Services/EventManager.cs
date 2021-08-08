// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public interface IEventListener
    {
        /// <summary>
        /// Listing to a javascript event
        /// </summary>
        /// <typeparam name="T">The type of the event args for instance MouseEventArgs for mousemove</typeparam>
        /// <param name="eventName">Name of the DOM event without "on"</param>
        /// <param name="elementId">The value of the id field of the DOM element</param>
        /// <param name="projectionName">The name of a JS function (relative to window) that used to project the event before it is send back to .NET. Can be null, if no projection is needed </param>
        /// <param name="throotleInterval">The delay between the last time the event occurred and the callback is fired. Set to zero, if no delay is requested</param>
        /// <param name="callback">The method that is invoked, if the DOM element is fired. Object will be of type T</param>
        /// <returns>A unique identifier for the event subscription. Should be used to cancel the subscription</returns>
        Task<Guid> Subscribe<T>(string eventName, string elementId, string projectionName, int throotleInterval, Func<object, Task> callback);

        /// <summary>
        /// Cancel (unsubscribe) the listening to a DOM event, previous connected by Subscribe
        /// </summary>
        /// <param name="key">The unique event identifier</param>
        /// <returns>true for if the event listener was detached, false if not</returns>
        Task<bool> Unsubscribe(Guid key);
    }

    public class EventListener : IEventListener, IAsyncDisposable, IDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly DotNetObjectReference<EventListener> _dotNetRef;
        private bool _disposed = false;

        private Dictionary<Guid, (Type eventType, Func<object, Task> callback)> _callbackResolver = new();

        public EventListener(IJSRuntime runtime)
        {
            _jsRuntime = runtime;
            _dotNetRef = DotNetObjectReference.Create(this);
        }

        [JSInvokable]
        public async Task OnEventOccur(Guid key, string @eventData)
        {
            if (_callbackResolver.ContainsKey(key) == false) { return; }

            var element = _callbackResolver[key];

            var @event = JsonSerializer.Deserialize(eventData, element.eventType, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            });

            if (element.callback != null)
            {
                await element.callback.Invoke(@event);
            }
        }

        public async Task<Guid> Subscribe<T>(string eventName, string elementId, string projectionName, int throotleInterval, Func<object, Task> callback)
        {
            var key = Guid.NewGuid();
            var type = typeof(T);

            _callbackResolver.Add(key, (type, callback));

            var properties = type.GetProperties().Select(x => char.ToLower(x.Name[0]) + x.Name.Substring(1)).ToArray();

            await _jsRuntime.InvokeVoidAsync("mudThrottledEventManager.subscribe", eventName, elementId, projectionName, throotleInterval, key, properties, _dotNetRef);

            return key;
        }

        public async Task<bool> Unsubscribe(Guid key)
        {
            if (_callbackResolver.ContainsKey(key) == false) { return false; }

            try
            {
                await _jsRuntime.InvokeVoidAsync("mudThrottledEventManager.unsubscribe", key);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region disposing

        public async ValueTask DisposeAsync()
        {
            if (_disposed == true) { return; }

            foreach (var item in _callbackResolver)
            {
                try
                {
                    await _jsRuntime.InvokeVoidAsync("mudThrottledEventManager.unsubscribe", item.Key);
                }
                catch (Exception)
                {
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

        #endregion

    }
}
