

#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.JSInterop;
namespace MudBlazor
{
    public interface IScrollListener : IAsyncDisposable, IDisposable
    {
        /// <summary>
        /// The CSS selector to which the scroll event will be attached
        /// </summary>
        string? Selector { get; set; }

        ValueTask SubscribeOnScrollAsync(Func<ScrollEventArgs, Task> onNext);
    }

    internal class ScrollListener : IScrollListener
    {
        private readonly Subject<ScrollEventArgs> _scrollEventSubject;
        private readonly IJSRuntime _js;
        private DotNetObjectReference<ScrollListener>? _dotNetRef;

        /// <summary>
        /// The CSS selector to which the scroll event will be attached
        /// </summary>
        public string? Selector { get; set; }

        [DynamicDependency(nameof(RaiseOnScroll))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ScrollEventArgs))]
        public ScrollListener(IJSRuntime js)
            : this(null, js)
        {
        }

        public ScrollListener(string? selector, IJSRuntime js)
        {
            _js = js;
            Selector = selector;
            _scrollEventSubject = new Subject<ScrollEventArgs>();
        }

        /// <summary>
        /// invoked in JS, in scroll-listener.js
        /// </summary>
        /// <param name="e">The scroll event args</param>
        [JSInvokable]
        public void RaiseOnScroll(ScrollEventArgs e)
        {
            //In normal scenario RaiseOnScroll shouldn't be raised when Subject is disposed
            //But let's add a protection in case for some reason cancelListener fails
            if (!_scrollEventSubject.IsDisposed)
            {
                _scrollEventSubject.OnNext(e);
            }
        }

        /// <summary>
        /// Subscribe to scroll event in JS
        /// </summary>        
        public ValueTask SubscribeOnScrollAsync(Func<ScrollEventArgs, Task> onNext)
        {
            _dotNetRef = DotNetObjectReference.Create(this);
            _scrollEventSubject.Select(onNext).Subscribe();
            return _js.InvokeVoidAsync("mudScrollListener.listenForScroll", _dotNetRef, Selector);
        }

        /// <summary>
        /// Unsubscribe to scroll event in 
        /// </summary>
        private async ValueTask UnsubscribeAsync()
        {
            try
            {
                await _js.InvokeVoidAsync("mudScrollListener.cancelListener", Selector);
            }
            catch (Exception)
            {
                /* ignore */
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnsubscribeAsync().AndForget();
                _scrollEventSubject.Dispose();
                _dotNetRef?.Dispose();
            }
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            await UnsubscribeAsync();
            _scrollEventSubject.Dispose();
            _dotNetRef?.Dispose();
        }
    }
}
