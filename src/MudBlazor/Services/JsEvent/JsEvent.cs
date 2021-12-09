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
    /// Subscribe JS events of any element by html id
    /// </summary>
    public class JsEvent : IDisposable
    {
        private bool _isDisposed = false;

        private readonly DotNetObjectReference<JsEvent> _dotNetRef;
        private readonly IJSRuntime _jsRuntime;
        private string _elementId;
        private bool _isObserving;
        private HashSet<string> _subscribedEvents = new HashSet<string>();

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
            if (_isObserving)
                return;
            _elementId = elementId;
            try
            {
                await _jsRuntime.InvokeVoidAsync("mudJsEvent.connect", _dotNetRef, elementId, options);
                _isObserving = true;
            }
            catch (JSDisconnectedException) { }
            catch (TaskCanceledException) { }
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

        private async Task Subscribe(string eventName)
        {
            if (_elementId == null)
                throw new InvalidOperationException("Call Connect(...) before attaching events!");
            if (_subscribedEvents.Contains(eventName))
                return;
            try
            {
                await _jsRuntime.InvokeVoidAsync("mudJsEvent.subscribe", _elementId, eventName);
                _subscribedEvents.Add(eventName);
            }
            catch (JSDisconnectedException) { }
            catch (TaskCanceledException) { }
        }

        private async Task Unsubscribe(string eventName)
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

        private async Task UnsubscribeAll()
        {
            if (_elementId == null)
                return;
            try
            {
                foreach(var eventName in _subscribedEvents)
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
                    Subscribe("click").AndForget();
                    Subscribe("keyup").AndForget();
                }
                _caretPositionChangedHandlers.Add(value);
            }
            remove
            {
                if (_caretPositionChangedHandlers.Count == 0)
                    return;
                if (_caretPositionChangedHandlers.Count == 1)
                {
                    Unsubscribe("click").AndForget();
                    Unsubscribe("keyup").AndForget();
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
                    Subscribe("paste").AndForget();
                _pasteHandlers.Add(value);
            }
            remove
            {
                if (_pasteHandlers.Count == 0)
                    return;
                if (_pasteHandlers.Count == 1)
                    Unsubscribe("paste").AndForget();
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

        List<Action> _copyHandlers = new List<Action>();

        /// <summary>
        /// Subscribe this event to get notified about paste actions
        /// </summary>
        public event Action Copy
        {
            add
            {
                if (_copyHandlers.Count == 0)
                    Subscribe("copy").AndForget();
                _copyHandlers.Add(value);
            }
            remove
            {
                if (_copyHandlers.Count == 0)
                    return;
                if (_copyHandlers.Count == 1)
                    Unsubscribe("copy").AndForget();
                _copyHandlers.Remove(value);
            }
        }

        /// <summary>
        /// To be invoked only by JS
        /// </summary>
        [JSInvokable]
        public void OnCopy()
        {
            foreach (var handler in _copyHandlers)
            {
                handler.Invoke();
            }
        }

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
