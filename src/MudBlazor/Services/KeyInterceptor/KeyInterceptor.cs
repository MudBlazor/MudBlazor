// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    /// <summary>
    /// This transient service binds itself to a parent element to observe the keys of one of its children.
    /// It can call preventDefault or stopPropagation directly on the JavaScript side for single key strokes / key combinations as per configuration.
    /// Furthermore, you can precisely subscribe single keystrokes or combinations and only the subscribed ones will be forwarded into .NET
    /// </summary>
    public class KeyInterceptor : IKeyInterceptor, IDisposable
    {
        private bool _isDisposed = false;

        private readonly DotNetObjectReference<KeyInterceptor> _dotNetRef;
        private readonly IJSRuntime _jsRuntime;
        private bool _isObserving;
        private string _elementId;

        public KeyInterceptor(IJSRuntime jsRuntime)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Connect to the ancestor element of the element(s) that should be observed
        /// </summary>
        /// <param name="elementId">Ancestor html element id</param>
        /// <param name="options">Define here the descendant(s) by setting TargetClass and the keystrokes to be monitored / suppressed</param>
        public async Task Connect(string elementId, KeyInterceptorOptions options)
        {
            if (_isObserving)
                return;
            _elementId = elementId;
            try
            {
                await _jsRuntime.InvokeVoidAsync("mudKeyInterceptor.connect", _dotNetRef, elementId, options);
                _isObserving = true;
            }
            catch (JSDisconnectedException) { }
            catch (TaskCanceledException) { }
        }

        /// <summary>
        /// Update behavior of a registered keyoption
        /// The keystrike to update has to be monitored previously
        /// </summary>
        /// <param name="option">Define KeyOption to update</param>
        public async Task UpdateKey(KeyOptions option)
        {
            await _jsRuntime.InvokeVoidAsync($"mudKeyInterceptor.updatekey", _elementId, option);
        }

        /// <summary>
        /// Disconnect from the previously connected ancestor and its descendants
        /// </summary>
        public async Task Disconnect()
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync($"mudKeyInterceptor.disconnect", _elementId);
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
            Disconnect().AndForget();
            _dotNetRef.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
