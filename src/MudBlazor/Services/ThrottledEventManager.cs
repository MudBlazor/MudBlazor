// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public interface IThrottledEventManager
    {
        Task<Guid> Subscribe<T>(string eventName, string id, int throotleInterval, Func<object, Task> callback);
        Task<bool> Unsubscribe(Guid key);
    }

    public class ThrottledEventManager : IThrottledEventManager, IAsyncDisposable, IDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly DotNetObjectReference<ThrottledEventManager> _dotNetRef;
        private bool _disposed = false;

        private Dictionary<Guid, (Type, Func<object, Task>)> _callbackResolver = new();

        public ThrottledEventManager(IJSRuntime runtime)
        {
            _jsRuntime = runtime;
            _dotNetRef = DotNetObjectReference.Create(this);
        }

        [JSInvokable]
        public async Task OnEventOccur(Guid key, string @eventData)
        {
            if (_callbackResolver.ContainsKey(key) == false) { return; }

            var callback = _callbackResolver[key];

            var @event = JsonSerializer.Deserialize(eventData, callback.Item1,new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
            });

            await callback.Item2?.Invoke(@event);
        }

        public async Task<Guid> Subscribe<T>(string eventName, string elementId, int throotleInterval, Func<object, Task> callback) 
        {
            Guid key = Guid.NewGuid();
            var type = typeof(T);

            _callbackResolver.Add(key, (type,callback));

            var properties = type.GetProperties().Select(x => char.ToLower(x.Name[0]) + x.Name.Substring(1)).ToArray();

            await _jsRuntime.InvokeVoidAsync("mudThrottledEventManager.subscribe", eventName, elementId, throotleInterval, key, properties, _dotNetRef);

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
