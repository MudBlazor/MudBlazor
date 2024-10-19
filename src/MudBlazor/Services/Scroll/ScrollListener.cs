using System.Diagnostics.CodeAnalysis;
using Microsoft.JSInterop;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Listens to scroll events on a specified element.
    /// </summary>
    internal sealed class ScrollListener : IScrollListener
    {
        private bool _disposed;
        private readonly IJSRuntime _js;
        private EventHandler<ScrollEventArgs>? _onScroll;
        private DotNetObjectReference<ScrollListener>? _dotNetRef;

        /// <inheritdoc />
        public string? Selector { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollListener"/> class with the specified JavaScript runtime.
        /// </summary>
        /// <param name="js">The JavaScript runtime.</param>
        [DynamicDependency(nameof(RaiseOnScroll))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ScrollEventArgs))]
        public ScrollListener(IJSRuntime js) : this(string.Empty, js)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollListener"/> class with the specified selector and JavaScript runtime.
        /// </summary>
        /// <param name="selector">The CSS selector for the element to listen for scroll events.</param>
        /// <param name="js">The JavaScript runtime.</param>
        public ScrollListener(string? selector, IJSRuntime js)
        {
            _js = js;
            Selector = selector;
        }

        /// <inheritdoc />
        public event EventHandler<ScrollEventArgs> OnScroll
        {
            add => Subscribe(value);
            remove => Unsubscribe(value);
        }

        private async void Subscribe(EventHandler<ScrollEventArgs> value)
        {
            if (_onScroll == null)
            {
                await Start();
            }
            _onScroll += value;
        }

        private void Unsubscribe(EventHandler<ScrollEventArgs> value)
        {
            _onScroll -= value;
            if (_onScroll == null)
            {
                Cancel().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Invoked in JavaScript, in scroll-listener.js.
        /// </summary>
        /// <param name="e">The scroll event arguments.</param>
        [JSInvokable]
        public void RaiseOnScroll(ScrollEventArgs e)
        {
            _onScroll?.Invoke(this, e);
        }

        /// <summary>
        /// Subscribes to the scroll event in JavaScript.
        /// </summary>
        private ValueTask<bool> Start()
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            return _js.InvokeVoidAsyncWithErrorHandling("mudScrollListener.listenForScroll", _dotNetRef, Selector);
        }

        /// <summary>
        /// Unsubscribes from the scroll event in JavaScript.
        /// </summary>
        private async ValueTask Cancel()
        {
            await _js.InvokeVoidAsyncWithErrorHandling("mudScrollListener.cancelListener", Selector);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _dotNetRef?.Dispose();

                _onScroll = null;
            }
        }
    }
}
