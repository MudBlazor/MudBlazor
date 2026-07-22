// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MudBlazor.Interop;

namespace MudBlazor.Services
{
    public interface IJsEvent : IDisposable
    {
        Task Connect(string elementId, JsEventOptions options);
        Task Disconnect();
        event Action<int> CaretPositionChanged;
        event Action<string> Paste;
        event Action<int, int> Select;
    }

    /// <summary>
    /// Subscribe JS events of any element by html id
    /// </summary>
    public class JsEvent : IJsEvent
    {
        private bool _isDisposed = false;

        private readonly DotNetObjectReference<JsEvent> _dotNetRef;
        private readonly IJSRuntime _jsRuntime;
        private string _elementId;
        private bool _isObserving;
        internal HashSet<string> _subscribedEvents = new HashSet<string>();

        [DynamicDependency(nameof(OnCaretPositionChanged))]
        [DynamicDependency(nameof(OnPaste))]
        [DynamicDependency(nameof(OnSelect))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsEventOptions))]
        public JsEvent(IJSRuntime jsRuntime)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            _jsRuntime = jsRuntime;
        }

        /// <summary>
        /// Connect to the ancestor element of the element(s) that should be observed
        /// </summary>
        /// <param name="elementId">Ancestor html element id</param>
        /// <param name="options">Define here the descendant(s) by setting TargetClass and the keystrokes to be monitored</param>
        public async Task Connect(string elementId, JsEventOptions options)
        {
            if (_isObserving || _isDisposed)
                return;
            _elementId = elementId;
            _isObserving = await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudJsEvent.connect", _dotNetRef, elementId, options); ;
        }

        /// <summary>
        /// Disconnect from the previously connected ancestor and its descendants
        /// </summary>
        public async Task Disconnect()
        {
            if (_elementId == null)
                return;
            await UnsubscribeAll();
            try
            {
                await _jsRuntime.InvokeVoidAsync($"mudJsEvent.disconnect", _elementId);
            }
            catch (Exception) {  /*ignore*/ }
            _isObserving = false;
        }

        internal void Subscribe(string eventName)
        {
            if (_elementId == null)
                throw new InvalidOperationException("Call Connect(...) before attaching events!");
            if (_subscribedEvents.Contains(eventName) || _isDisposed)
                return;
            try
            {
                _jsRuntime.InvokeVoidAsync("mudJsEvent.subscribe", _elementId, eventName).CatchAndLog();
                _subscribedEvents.Add(eventName);
            }
            catch (JSDisconnectedException) { }
            catch (TaskCanceledException) { }
        }

        internal async Task Unsubscribe(string eventName)
        {
            if (_elementId == null)
                return;
            try
            {
                await _jsRuntime.InvokeVoidAsync($"mudJsEvent.unsubscribe", _elementId, eventName);
            }
            catch (Exception) {  /*ignore*/ }
            _subscribedEvents.Remove(eventName);
        }

        internal async Task UnsubscribeAll()
        {
            if (_elementId == null)
                return;
            try
            {
                foreach (var eventName in _subscribedEvents)
                    await _jsRuntime.InvokeVoidAsync($"mudJsEvent.unsubscribe", _elementId, eventName);
            }
            catch (Exception) {  /*ignore*/ }
            _subscribedEvents.Clear();
        }

        List<Action<int>> _caretPositionChangedHandlers = new List<Action<int>>();

        /// <summary>
        /// Subscribe this event to get notified about caret changes in an input on click and on keyup
        /// </summary>
        public event Action<int> CaretPositionChanged
        {
            add
            {
                if (_caretPositionChangedHandlers.Count == 0)
                {
                    Subscribe("click");
                    Subscribe("keyup");
                }
                _caretPositionChangedHandlers.Add(value);
            }
            remove
            {
                if (_caretPositionChangedHandlers.Count == 0)
                    return;
                if (_caretPositionChangedHandlers.Count == 1)
                {
                    Unsubscribe("click").CatchAndLog();
                    Unsubscribe("keyup").CatchAndLog();
                }
                _caretPositionChangedHandlers.Remove(value);
            }
        }

        /// <summary>
        /// To be invoked only by JS
        /// </summary>
        [JSInvokable]
        public void OnCaretPositionChanged(int caretPosition)
        {
            foreach (var handler in _caretPositionChangedHandlers)
            {
                handler.Invoke(caretPosition);
            }
        }

        List<Action<string>> _pasteHandlers = new List<Action<string>>();

        /// <summary>
        /// Subscribe this event to get notified about paste actions
        /// </summary>
        public event Action<string> Paste
        {
            add
            {
                if (_pasteHandlers.Count == 0)
                    Subscribe("paste");
                _pasteHandlers.Add(value);
            }
            remove
            {
                if (_pasteHandlers.Count == 0)
                    return;
                if (_pasteHandlers.Count == 1)
                    Unsubscribe("paste").CatchAndLog();
                _pasteHandlers.Remove(value);
            }
        }

        /// <summary>
        /// To be invoked only by JS
        /// </summary>
        [JSInvokable]
        public void OnPaste(string text)
        {
            foreach (var handler in _pasteHandlers)
            {
                handler.Invoke(text);
            }
        }

        List<Action<int, int>> _selectHandlers = new List<Action<int, int>>();

        /// <summary>
        /// Subscribe this event to get notified about paste actions
        /// </summary>
        public event Action<int, int> Select
        {
            add
            {
                if (_selectHandlers.Count == 0)
                    Subscribe("select");
                _selectHandlers.Add(value);
            }
            remove
            {
                if (_selectHandlers.Count == 0)
                    return;
                if (_selectHandlers.Count == 1)
                    Unsubscribe("select").CatchAndLog();
                _selectHandlers.Remove(value);
            }
        }

        /// <summary>
        /// To be invoked only by JS
        /// </summary>
        [JSInvokable]
        public void OnSelect(int start, int end)
        {
            foreach (var handler in _selectHandlers)
            {
                handler.Invoke(start, end);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _isDisposed)
                return;
            _isDisposed = true;
            Disconnect().CatchAndLog();
            _dotNetRef.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

    }
}
