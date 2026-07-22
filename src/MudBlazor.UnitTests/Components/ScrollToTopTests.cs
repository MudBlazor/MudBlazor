using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class ScrollToTopTests : BunitTest
{
    [Test]
    public void Render_CreatesScrollListenerOnceForConfiguredSelector()
    {
        var scrollListenerFactory = new TestScrollListenerFactory();
        var component = RenderScrollToTop(scrollListenerFactory, parameters => parameters
            .Add(x => x.Selector, "#scrollable")
            .Add(x => x.Class, "initial-class"));

        scrollListenerFactory.CreateCalls.Should().Be(1);
        scrollListenerFactory.CreatedSelectors.Should().ContainSingle().Which.Should().Be("#scrollable");
        scrollListenerFactory.Listener.SubscriberCount.Should().Be(1);

        component.Render();

        scrollListenerFactory.CreateCalls.Should().Be(1);
    }

    [Test]
    public async Task ScrollEvent_UsesScrollTopToToggleVisibilityAndRaiseCallback()
    {
        var scrollListenerFactory = new TestScrollListenerFactory();
        ScrollEventArgs receivedScrollArgs = null;
        var component = RenderScrollToTop(scrollListenerFactory, parameters => parameters
            .Add(x => x.TopOffset, 50)
            .Add(x => x.OnScroll, EventCallback.Factory.Create<ScrollEventArgs>(this, args => receivedScrollArgs = args)));

        component.Find("span").ClassList.Should().Contain("hidden");
        component.Instance.Visible.Should().BeFalse();

        var visibleEventArgs = new ScrollEventArgs
        {
            NodeName = "DIV",
            ScrollTop = 75
        };

        scrollListenerFactory.Listener.RaiseScroll(visibleEventArgs);

        await component.WaitForAssertionAsync(() =>
        {
            component.Instance.Visible.Should().BeTrue();
            component.Find("span").ClassList.Should().Contain("visible");
            receivedScrollArgs.Should().BeSameAs(visibleEventArgs);
        });

        scrollListenerFactory.Listener.RaiseScroll(new ScrollEventArgs
        {
            NodeName = "DIV",
            ScrollTop = 20
        });

        await component.WaitForAssertionAsync(() =>
        {
            component.Instance.Visible.Should().BeFalse();
            component.Find("span").ClassList.Should().Contain("hidden");
        });
    }

    [Test]
    public async Task ScrollEvent_UsesDocumentOffsetAndCustomCssClasses()
    {
        var scrollListenerFactory = new TestScrollListenerFactory();
        var component = RenderScrollToTop(scrollListenerFactory, parameters => parameters
            .Add(x => x.Selector, "   ")
            .Add(x => x.TopOffset, 80)
            .Add(x => x.VisibleCssClass, "fade-in")
            .Add(x => x.HiddenCssClass, "fade-out"));

        scrollListenerFactory.CreatedSelectors.Should().ContainSingle().Which.Should().BeNull();
        component.Find("span").ClassList.Should().Contain("fade-out");
        component.Find("span").ClassList.Should().NotContain("hidden");

        scrollListenerFactory.Listener.RaiseScroll(new ScrollEventArgs
        {
            NodeName = "#document",
            FirstChildBoundingClientRect = new Interop.BoundingClientRect
            {
                Top = -120
            }
        });

        await component.WaitForAssertionAsync(() =>
        {
            component.Instance.Visible.Should().BeTrue();
            component.Find("span").ClassList.Should().Contain("fade-in");
            component.Find("span").ClassList.Should().NotContain("fade-out");
            component.Find("span").ClassList.Should().NotContain("visible");
        });
    }

    [Test]
    public async Task Click_ScrollsToTopUsingListenerSelectorAndRaisesOnClick()
    {
        var scrollListenerFactory = new TestScrollListenerFactory();
        MouseEventArgs receivedClickArgs = null;
        var scrollManager = new TestScrollManager();
        var component = RenderScrollToTop(
            scrollListenerFactory,
            parameters => parameters
                .Add(x => x.Selector, "#scroll-host")
                .Add(x => x.ScrollBehavior, ScrollBehavior.Auto)
                .Add(x => x.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, args => receivedClickArgs = args)),
            scrollManager);

        await component.Find("span.mud-scroll-to-top").ClickAsync();

        scrollManager.ScrollToTopCalls.Should().ContainSingle();
        scrollManager.ScrollToTopCalls[0].Selector.Should().Be("#scroll-host");
        scrollManager.ScrollToTopCalls[0].ScrollBehavior.Should().Be(ScrollBehavior.Auto);
        receivedClickArgs.Should().NotBeNull();
    }

    [Test]
    public async Task DisposeAsync_UnsubscribesAndDisposesListener()
    {
        var scrollListenerFactory = new TestScrollListenerFactory();
        var component = RenderScrollToTop(scrollListenerFactory);

        scrollListenerFactory.Listener.SubscriberCount.Should().Be(1);

        await component.Instance.DisposeAsync();

        scrollListenerFactory.Listener.SubscriberCount.Should().Be(0);
        scrollListenerFactory.Listener.DisposeCount.Should().Be(1);
    }

    private IRenderedComponent<MudScrollToTop> RenderScrollToTop(
        TestScrollListenerFactory scrollListenerFactory,
        Action<ComponentParameterCollectionBuilder<MudScrollToTop>> configure = null,
        TestScrollManager scrollManager = null)
    {
        scrollManager ??= new TestScrollManager();
        Context.Services.AddSingleton<IScrollListenerFactory>(scrollListenerFactory);
        Context.Services.AddSingleton<IScrollManager>(scrollManager);

        return Context.Render<MudScrollToTop>(parameters =>
        {
            parameters.AddChildContent("Back to top");
            configure?.Invoke(parameters);
        });
    }

    private sealed class TestScrollListenerFactory : IScrollListenerFactory
    {
        public TestScrollListener Listener { get; } = new();
        public List<string> CreatedSelectors { get; } = new();
        public int CreateCalls { get; private set; }

        public IScrollListener Create(string selector)
        {
            return Create(selector, 10);
        }

        public IScrollListener Create(string selector, int reportRateMs)
        {
            CreateCalls++;
            CreatedSelectors.Add(selector);
            Listener.Selector = selector;
            Listener.ReportRateMs = reportRateMs;
            return Listener;
        }
    }

    private sealed class TestScrollListener : IScrollListener
    {
        private EventHandler<ScrollEventArgs> _onScroll;

        public string Selector { get; set; }

        public int ReportRateMs { get; set; }

        public int SubscriberCount => _onScroll?.GetInvocationList().Length ?? 0;

        public int DisposeCount { get; private set; }

        public event EventHandler<ScrollEventArgs> OnScroll
        {
            add => _onScroll += value;
            remove => _onScroll -= value;
        }

        public ValueTask<ScrollEventArgs> GetCurrentScrollDataAsync() => ValueTask.FromResult(new ScrollEventArgs());

        public void RaiseScroll(ScrollEventArgs args)
        {
            _onScroll?.Invoke(this, args);
        }

        public ValueTask DisposeAsync()
        {
            DisposeCount++;
            return ValueTask.CompletedTask;
        }
    }

    private sealed class TestScrollManager : IScrollManager
    {
        public List<(string Selector, ScrollBehavior ScrollBehavior)> ScrollToTopCalls { get; } = new();

        public ValueTask LockScrollAsync(string selector = "body", string cssClass = "scroll-locked") => ValueTask.CompletedTask;

        public ValueTask ScrollIntoViewAsync(string selector, ScrollBehavior behavior) => ValueTask.CompletedTask;

        public ValueTask ScrollToAsync(string id, int left, int top, ScrollBehavior scrollBehavior) => ValueTask.CompletedTask;

        public ValueTask ScrollToBottomAsync(string elementId, ScrollBehavior scrollBehavior = ScrollBehavior.Auto) => ValueTask.CompletedTask;

        public ValueTask ScrollToListItemAsync(string elementId) => ValueTask.CompletedTask;

        public ValueTask ScrollToTopAsync(string id, ScrollBehavior scrollBehavior = ScrollBehavior.Auto)
        {
            ScrollToTopCalls.Add((id, scrollBehavior));
            return ValueTask.CompletedTask;
        }

        public ValueTask ScrollToVirtualizedItemAsync(string containerId, int itemIndex, double itemHeight, string targetItemId, ScrollBehavior scrollBehavior = ScrollBehavior.Auto) => ValueTask.CompletedTask;

        public ValueTask ScrollToYearAsync(string elementId) => ValueTask.CompletedTask;

        public ValueTask UnlockScrollAsync(string selector = "body", string cssClass = "scroll-locked") => ValueTask.CompletedTask;
    }
}
