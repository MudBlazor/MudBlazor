// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.JSInterop;

namespace MudBlazor.Services
{
    /// <summary>
    /// Subscribes to JavaScript events of any HTML element by its ID.
    /// </summary>
    internal sealed class JsEvent : IJsEvent
    {
        private bool _disposed;
        private string _elementId;
        private bool _isObserving;
        private readonly IJSRuntime _jsRuntime;
        private readonly DotNetObjectReference<JsEvent> _dotNetRef;
        internal HashSet<string> _subscribedEvents = new();
        private readonly List<Action<string>> _pasteHandlers = new();
        private readonly List<Action<int, int>> _selectHandlers = new();
        private readonly List<Action<int>> _caretPositionChangedHandlers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="JsEvent"/> class.
        /// </summary>
        /// <param name="jsRuntime">The JavaScript runtime to use for invoking JavaScript functions.</param>
        [DynamicDependency(nameof(OnCaretPositionChanged))]
        [DynamicDependency(nameof(OnPaste))]
        [DynamicDependency(nameof(OnSelect))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(JsEventOptions))]
        public JsEvent(IJSRuntime jsRuntime)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            _jsRuntime = jsRuntime;
        }

        /// <inheritdoc />
        public async Task Connect(string elementId, JsEventOptions options)
        {
            if (_isObserving || _disposed)
            {
                return;
            }

            _elementId = elementId;
            _isObserving = await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudJsEvent.connect", _dotNetRef, elementId, options);
        }

        /// <inheritdoc />
        public async Task Disconnect()
        {
            if (_elementId is null)
            {
                return;
            }

            await UnsubscribeAll();
            await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudJsEvent.disconnect", _elementId);

            _isObserving = false;
        }

        /// <summary>
        /// Subscribes to the specified JavaScript event.
        /// </summary>
        /// <param name="eventName">The name of the event to subscribe to.</param>
        internal void Subscribe(string eventName)
        {
            if (_elementId is null)
            {
                throw new InvalidOperationException("Call Connect(...) before attaching events!");
            }

            if (_subscribedEvents.Contains(eventName) || _disposed)
            {
                return;
            }

            _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudJsEvent.subscribe", _elementId, eventName).CatchAndLog();
            _subscribedEvents.Add(eventName);
        }

        /// <summary>
        /// Unsubscribes from the specified JavaScript event.
        /// </summary>
        /// <param name="eventName">The name of the event to unsubscribe from.</param>
        internal async Task Unsubscribe(string eventName)
        {
            if (_elementId is null)
            {
                return;
            }

            await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudJsEvent.unsubscribe", _elementId, eventName);
            _subscribedEvents.Remove(eventName);
        }

        /// <summary>
        /// Unsubscribes from all JavaScript events.
        /// </summary>
        internal async Task UnsubscribeAll()
        {
            if (_elementId is null)
            {
                return;
            }

            foreach (var eventName in _subscribedEvents)
            {
                await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudJsEvent.unsubscribe", _elementId, eventName);
            }

            _subscribedEvents.Clear();
        }

        /// <inheritdoc />
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
                {
                    return;
                }

                if (_caretPositionChangedHandlers.Count == 1)
                {
                    Unsubscribe("click").CatchAndLog();
                    Unsubscribe("keyup").CatchAndLog();
                }

                _caretPositionChangedHandlers.Remove(value);
            }
        }

        /// <inheritdoc />
        public event Action<string> Paste
        {
            add
            {
                if (_pasteHandlers.Count == 0)
                {
                    Subscribe("paste");
                }

                _pasteHandlers.Add(value);
            }
            remove
            {
                if (_pasteHandlers.Count == 0)
                {
                    return;
                }

                if (_pasteHandlers.Count == 1)
                {
                    Unsubscribe("paste").CatchAndLog();
                }

                _pasteHandlers.Remove(value);
            }
        }

        /// <inheritdoc />
        public event Action<int, int> Select
        {
            add
            {
                if (_selectHandlers.Count == 0)
                {
                    Subscribe("select");
                }

                _selectHandlers.Add(value);
            }
            remove
            {
                if (_selectHandlers.Count == 0)
                {
                    return;
                }

                if (_selectHandlers.Count == 1)
                {
                    Unsubscribe("select").CatchAndLog();
                }

                _selectHandlers.Remove(value);
            }
        }

        /// <summary>
        /// Invoked by JavaScript when a text selection action is performed.
        /// </summary>
        /// <param name="start">The start position of the selection.</param>
        /// <param name="end">The end position of the selection.</param>
        [JSInvokable]
        public void OnSelect(int start, int end)
        {
            foreach (var handler in _selectHandlers)
            {
                handler.Invoke(start, end);
            }
        }

        /// <summary>
        /// Invoked by JavaScript when a paste action is performed.
        /// </summary>
        /// <param name="text">The text that was pasted.</param>
        [JSInvokable]
        public void OnPaste(string text)
        {
            foreach (var handler in _pasteHandlers)
            {
                handler.Invoke(text);
            }
        }

        /// <summary>
        /// Invoked by JavaScript when the caret position changes in an input element.
        /// </summary>
        /// <param name="caretPosition">The new caret position.</param>
        [JSInvokable]
        public void OnCaretPositionChanged(int caretPosition)
        {
            foreach (var handler in _caretPositionChangedHandlers)
            {
                handler.Invoke(caretPosition);
            }
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                _disposed = true;
                await Disconnect();
                _dotNetRef.Dispose();
            }
        }
    }
}
