namespace MudBlazor.UnitTests.Shared.Mocks
{
    public class MockScrollListenerFactory : IScrollListenerFactory
    {
        public IScrollListener Create(string? selector)
        {
            return Create(selector, 100);
        }

        public IScrollListener Create(string? selector, int reportRateMs)
        {
            return new MockScrollListener { Selector = selector, ReportRateMs = reportRateMs };
        }
    }

    /// <summary>
    /// Mock for scroll listener
    /// </summary>
    public class MockScrollListener : IScrollListener
    {
        public string? Selector { get; set; }
        public int ReportRateMs { get; set; }

        public event EventHandler<ScrollEventArgs>? OnScroll;

        public MockScrollListener()
        {
            OnScroll?.Invoke(this, new ScrollEventArgs());
        }

        public ValueTask<ScrollEventArgs> GetCurrentScrollDataAsync()
        {
            return ValueTask.FromResult(new ScrollEventArgs());
        }

        public ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>
    /// Mock for scroll manager
    /// </summary>
    public class MockScrollManager : IScrollManager
    {
        public string? Selector { get; set; }

        public ValueTask LockScrollAsync(string elementId, string cssClass) => ValueTask.CompletedTask;

        public Task ScrollTo(int left, int top, ScrollBehavior scrollBehavior) => Task.CompletedTask;

        public ValueTask ScrollToAsync(string? id, int left, int top, ScrollBehavior scrollBehavior) => ValueTask.CompletedTask;

        public ValueTask ScrollIntoViewAsync(string? selector, ScrollBehavior behavior) => ValueTask.CompletedTask;

        public Task ScrollToFragment(string id, ScrollBehavior behavior) => Task.CompletedTask;

        public ValueTask ScrollToFragmentAsync(string id, ScrollBehavior behavior) => ValueTask.CompletedTask;

        public ValueTask ScrollToListItemAsync(string elementId) => ValueTask.CompletedTask;

        public Task ScrollToTop(ScrollBehavior scrollBehavior = ScrollBehavior.Auto) => Task.CompletedTask;

        public ValueTask ScrollToTopAsync(string? id, ScrollBehavior scrollBehavior = ScrollBehavior.Auto) => ValueTask.CompletedTask;

        public ValueTask ScrollToBottomAsync(string id, ScrollBehavior scrollBehavior = ScrollBehavior.Auto) => ValueTask.CompletedTask;

        public ValueTask ScrollToYearAsync(string elementId) => ValueTask.CompletedTask;

        public ValueTask UnlockScrollAsync(string elementId, string cssClass) => ValueTask.CompletedTask;

        public ValueTask ScrollToVirtualizedItemAsync(string containerId, int itemIndex, double itemHeight, string targetItemId, ScrollBehavior scrollBehavior = ScrollBehavior.Auto) =>
            ValueTask.CompletedTask;
    }
}
