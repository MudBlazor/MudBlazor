using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MudBlazor.Interop;

namespace MudBlazor.Services
{
    public class KeyInterceptor : IKeyInterceptor, IDisposable
    {
        private bool _isDisposed = false;

        private readonly DotNetObjectReference<KeyInterceptor> _dotNetRef;
        private readonly IJSRuntime _jsRuntime;
        private bool _isObserving;
        private ElementReference _element;

        public KeyInterceptor(IJSRuntime jsRuntime)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            _jsRuntime = jsRuntime;

        }

        public async Task Connect(ElementReference element, KeyInterceptorOptions options)
        {
            if (_isObserving)
                return;
            _element = element;
            try
            {
                await _jsRuntime.InvokeVoidAsync("mudKeyInterceptor.connect", _dotNetRef, element, options);
                _isObserving = true;
            }
            catch (TaskCanceledException) { /*ignore*/ }
        }

        public async Task Disconnect()
        {
            try
            {
                var task=_jsRuntime.InvokeVoidAsync($"mudKeyInterceptor.disconnect", _element);
                if (!_isDisposed)
                    await task;
            } catch (Exception) {  /*ignore*/ }
            _isObserving = false;
        }

        [JSInvokable]
        public void OnKeyDown(KeyboardEventArgs args)
        {
            KeyDown?.Invoke( args);
        }

        [JSInvokable]
        public void OnKeyUp(KeyboardEventArgs args)
        {
            KeyUp?.Invoke( args);
        }

        public event KeyboardEvent KeyDown;
        public event KeyboardEvent KeyUp;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _isDisposed)
                return;
            _isDisposed = true;
            _=Disconnect();
            _dotNetRef.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
