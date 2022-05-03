using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public interface IScrollListener
    {
        /// <summary>
        /// The CSS selector to which the scroll event will be attached
        /// </summary>
        string Selector { get; set; }

        event EventHandler<ScrollEventArgs> OnScroll;
        void Dispose(); // Transient services can't be IDisposable but the service must still be (manually) deallocated
    }

    internal class ScrollListener : IScrollListener
    {
        private readonly IJSRuntime _js;
        private DotNetObjectReference<ScrollListener> _dotNetRef;

        /// <summary>
        /// The CSS selector to which the scroll event will be attached
        /// </summary>
        public string Selector { get; set; } = null;

        public ScrollListener(IJSRuntime js)
        {
            _js = js;
        }

        private EventHandler<ScrollEventArgs> _onScroll;
        private bool _isDisposed;

        /// <summary>
        /// OnScroll event. Fired when a element is scrolled
        /// </summary>
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
        /// invoked in JS, in scroll-listener.js
        /// </summary>
        /// <param name="e">The scroll event args</param>
        [JSInvokable]
        public void RaiseOnScroll(ScrollEventArgs e)
        {
            _onScroll?.Invoke(this, e);
        }

        /// <summary>
        /// Subscribe to scroll event in JS
        /// </summary>        
        private ValueTask Start()
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            return _js.InvokeVoidAsync
                ("mudScrollListener.listenForScroll",
                           _dotNetRef,
                           Selector);
        }

        /// <summary>
        /// Unsubscribe to scroll event in 
        /// </summary>
        private async ValueTask Cancel()
        {
            try
            {
                await _js.InvokeVoidAsync(
                    "mudScrollListener.cancelListener",
                               Selector);
            }
            catch { /* ignore */ }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed || !disposing)
                return;

            Cancel().AndForget(TaskOption.Safe);
            _dotNetRef?.Dispose();
            _isDisposed = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
