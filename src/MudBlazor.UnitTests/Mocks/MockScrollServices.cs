using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace MudBlazor.UnitTests.Mocks
{
    public class MockScrollListenerFactory : IScrollListenerFactory
    {
        public IScrollListener Create(string selector) =>
            new MockScrollListener()
            {
                Selector = selector,
            };
    }

    /// <summary>
    /// Mock for scroll listener
    /// </summary>
    public class MockScrollListener : IScrollListener
    {
        private readonly Subject<ScrollEventArgs> _scrollEventSubject;

        public string Selector { get; set; }

        public ValueTask SubscribeOnScrollAsync(Func<ScrollEventArgs, Task> onNext)
        {
            _scrollEventSubject.Select(onNext).Subscribe();
            return ValueTask.CompletedTask;
        }

        public MockScrollListener()
        {
            _scrollEventSubject = new Subject<ScrollEventArgs>();
        }

        public void Dispose()
        {
            _scrollEventSubject.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            _scrollEventSubject.Dispose();
            return ValueTask.CompletedTask;
        }
    }


    /// <summary>
    /// Mock for scroll manager
    /// </summary>
    public class MockScrollManager : IScrollManager
    {
        public string Selector { get; set; }

        public ValueTask LockScrollAsync(string elementId, string cssClass) => ValueTask.CompletedTask;

        public Task ScrollTo(int left, int top, ScrollBehavior scrollBehavior) => Task.CompletedTask;

        public ValueTask ScrollToAsync(string id, int left, int top, ScrollBehavior scrollBehavior) => ValueTask.CompletedTask;

        public Task ScrollToFragment(string id, ScrollBehavior behavior) => Task.CompletedTask;

        public ValueTask ScrollToFragmentAsync(string id, ScrollBehavior behavior) => ValueTask.CompletedTask;

        public ValueTask ScrollToListItemAsync(string elementId) => ValueTask.CompletedTask;

        public Task ScrollToTop(ScrollBehavior scrollBehavior = ScrollBehavior.Auto) => Task.CompletedTask;
        
        public ValueTask ScrollToTopAsync(string id, ScrollBehavior scrollBehavior = ScrollBehavior.Auto) => ValueTask.CompletedTask;

        public ValueTask ScrollToBottomAsync(string id, ScrollBehavior scrollBehavior = ScrollBehavior.Auto) => ValueTask.CompletedTask;

        public ValueTask ScrollToYearAsync(string elementId) => ValueTask.CompletedTask;

        public ValueTask UnlockScrollAsync(string elementId, string cssClass) => ValueTask.CompletedTask;
    }
}
