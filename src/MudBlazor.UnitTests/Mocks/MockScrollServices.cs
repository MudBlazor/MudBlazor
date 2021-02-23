﻿using System;
using System.Threading.Tasks;

namespace MudBlazor.UnitTests.Mocks
{
    /// <summary>
    /// Mock for scroll listener
    /// </summary>
    public class MockScrollListener : IScrollListener
    {
        public string Selector { get; set; }

        public event EventHandler<ScrollEventArgs> OnScroll;

        public MockScrollListener()
        {
            OnScroll?.Invoke(this, new ScrollEventArgs());
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

        public ValueTask ScrollToListItemAsync(string elementId, int increment, bool onEdges) => ValueTask.CompletedTask;

        public Task ScrollToTop(ScrollBehavior scrollBehavior = ScrollBehavior.Auto) => Task.CompletedTask;

        public ValueTask ScrollToTopAsync(string id, ScrollBehavior scrollBehavior = ScrollBehavior.Auto) => ValueTask.CompletedTask;

        public ValueTask ScrollToYearAsync(string elementId) => ValueTask.CompletedTask;

        public ValueTask UnlockScrollAsync(string elementId, string cssClass) => ValueTask.CompletedTask;
    }
}
