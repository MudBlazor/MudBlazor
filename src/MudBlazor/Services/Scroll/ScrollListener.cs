using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace MudBlazor
{


    public interface IScrollListener : IDisposable
    {
        /// <summary>
        /// The CSS selector to which the scroll event will be attached
        /// </summary>
        string Selector { get; set; }

        event EventHandler<ScrollEventArgs> OnScroll;
    }

    internal class ScrollListener : IScrollListener
    {
        private readonly IJSRuntime _js;
        private DotNetObjectReference<ScrollListener> _dotNetRef;

        /// <summary>
        /// The CSS selector to which the scroll event will be attached
        /// </summary>
        public string Selector { get; set; } = null;

        [DynamicDependency(nameof(RaiseOnScroll))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ScrollEventArgs))]
        public ScrollListener(IJSRuntime js) : this(string.Empty, js)
        {
        }

        public ScrollListener(string selector, IJSRuntime js)
        {
            _js = js;
            Selector = selector;
        }

        private EventHandler<ScrollEventArgs> _onScroll;

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
        private ValueTask<bool> Start()
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            return _js.InvokeVoidAsyncWithErrorHandling
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

        public void Dispose()
        {
            _dotNetRef?.Dispose();
        }
    }
}
