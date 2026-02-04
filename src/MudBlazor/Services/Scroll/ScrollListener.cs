using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.JSInterop;

namespace MudBlazor
{
    /// <summary>
    /// Listens to scroll events on a specified element.
    /// </summary>
    internal sealed class ScrollListener : IScrollListener
    {
        private readonly string _listenerId = Identifier.Create("scrollListener");
        private bool _disposed;
        private readonly IJSRuntime _js;
        private EventHandler<ScrollEventArgs>? _onScroll;
        private DotNetObjectReference<ScrollListener>? _dotNetRef;

        /// <inheritdoc />
        public string? Selector { get; set; }

        /// <inheritdoc />
        public int ReportRateMs { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollListener"/> class with the specified JavaScript runtime.
        /// </summary>
        /// <param name="js">The JavaScript runtime.</param>
        [DynamicDependency(nameof(RaiseOnScroll))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ScrollEventArgs))]
        public ScrollListener(IJSRuntime js) : this(string.Empty, js) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollListener"/> class with the specified selector and JavaScript runtime.
        /// </summary>
        /// <param name="selector">The CSS selector for the element to listen for scroll events.</param>
        /// <param name="js">The JavaScript runtime.</param>
        /// <param name="reportRateMs">The rate at which the <see cref="IScrollListener"/> will report scroll position changes (in milliseconds).</param>
        public ScrollListener(string? selector, IJSRuntime js, int reportRateMs = 10)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(reportRateMs, 0);

            _js = js;
            Selector = selector;
            ReportRateMs = reportRateMs;
        }

        /// <inheritdoc />
        public event EventHandler<ScrollEventArgs> OnScroll
        {
            add => Subscribe(value);
            remove => Unsubscribe(value);
        }

        private async void Subscribe(EventHandler<ScrollEventArgs> value)
        {
            try
            {
                if (_onScroll == null)
                {
                    await Start();
                }

                _onScroll += value;
            }
            catch
            {
                Debug.WriteLine("Failed to subscribe to scroll event.");
            }
        }

        private async void Unsubscribe(EventHandler<ScrollEventArgs> value)
        {
            try
            {
                _onScroll -= value;
                if (_onScroll == null)
                {
                    await CancelAsync();
                }
            }
            catch
            {
                Debug.WriteLine("Failed to unsubscribe from scroll event.");
            }
        }

        /// <summary>
        /// Subscribes to the scroll event in JavaScript.
        /// </summary>
        private ValueTask<bool> Start()
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            return _js.InvokeVoidAsyncWithErrorHandling("mudScrollListener.listenForScroll", _dotNetRef, _listenerId, Selector, ReportRateMs);
        }

        /// <summary>
        /// Invoked in JavaScript in scroll-listener.js.
        /// </summary>
        /// <param name="e">The scroll event arguments.</param>
        [JSInvokable]
        public void RaiseOnScroll(ScrollEventArgs e)
        {
            _onScroll?.Invoke(this, e);
        }

        /// <summary>
        /// Unsubscribes from the scroll event in JavaScript.
        /// </summary>
        private async ValueTask CancelAsync()
        {
            await _js.InvokeVoidAsyncWithErrorHandling("mudScrollListener.cancelListener", _listenerId);
        }

        /// <inheritdoc />
        public ValueTask<ScrollEventArgs> GetCurrentScrollDataAsync()
        {
            return _js.InvokeAsync<ScrollEventArgs>("mudScrollListener.getCurrentScrollPosition", Selector);
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                _disposed = true;

                try
                {
                    await CancelAsync();
                }
                finally
                {
                    _dotNetRef?.Dispose();
                    _onScroll = null;
                }
            }
        }
    }
}
