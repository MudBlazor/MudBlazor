using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.UnitTests.Mocks
{
 /// <summary>
 /// Mock for scroll listener
 /// </summary>
    public class MockScrollListener : IScrollListener
    {
        public string Selector { get ; set ; }

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
        public string Selector { get ; set ; }

        public Task ScrollTo(int left, int top, ScrollBehavior scrollBehavior)
        {
            return Task.CompletedTask;
        }

        public Task ScrollToFragment(string id, ScrollBehavior behavior)
        {
            return Task.CompletedTask;
        }

        public Task ScrollToTop(ScrollBehavior scrollBehavior = ScrollBehavior.Auto)
        {
            return Task.CompletedTask;
        }
    }
}
