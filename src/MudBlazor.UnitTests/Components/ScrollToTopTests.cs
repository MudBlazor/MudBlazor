using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor.Interop;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class ScrollToTopTests : BunitTest
{
    [Test]
    public async Task InitializesScrollListenerOnce_WithConfiguredSelector()
    {
        var (component, listener, _, factory) = RenderScrollToTopWithTestDoubles(parameters => parameters
            .Add(x => x.Selector, "#target"));

        listener.RaiseScroll(new ScrollEventArgs { NodeName = "DIV", ScrollTop = 500 });

        await component.WaitForAssertionAsync(() =>
        {
            component.Find("span").ClassList.Should().Contain("visible");
        });

        factory.CreateSelectors.Should().ContainSingle().Which.Should().Be("#target");
        factory.CreateWithReportRateCallCount.Should().Be(0);
        listener.Selector.Should().Be("#target");
    }

    [Test]
    public void InitializesScrollListener_WithNullSelector_WhenSelectorIsWhitespace()
    {
        var (_, listener, _, factory) = RenderScrollToTopWithTestDoubles(parameters => parameters
            .Add(x => x.Selector, "   "));

        factory.CreateSelectors.Should().ContainSingle().Which.Should().BeNull();
        listener.Selector.Should().BeNull();
    }

    [Test]
    public async Task ScrollEvent_UsesDefaultVisibilityClasses_WhenCrossingTopOffset()
    {
        var (component, listener, _, _) = RenderScrollToTopWithTestDoubles(parameters => parameters
            .Add(x => x.TopOffset, 100));

        component.Find("span").ClassList.Should().Contain("hidden");
        component.Find("span").ClassList.Should().NotContain("visible");

        listener.RaiseScroll(new ScrollEventArgs { NodeName = "DIV", ScrollTop = 100 });

        await component.WaitForAssertionAsync(() =>
        {
            component.Find("span").ClassList.Should().Contain("visible");
            component.Find("span").ClassList.Should().NotContain("hidden");
        });

        listener.RaiseScroll(new ScrollEventArgs { NodeName = "DIV", ScrollTop = 99 });

        await component.WaitForAssertionAsync(() =>
        {
            component.Find("span").ClassList.Should().Contain("hidden");
            component.Find("span").ClassList.Should().NotContain("visible");
        });
    }

    [Test]
    public async Task ScrollEvent_UsesDocumentTopOffset_AndInvokesOnScroll()
    {
        ScrollEventArgs receivedArgs = null;
        var (component, listener, _, _) = RenderScrollToTopWithTestDoubles(parameters => parameters
            .Add(x => x.TopOffset, 100)
            .Add(x => x.OnScroll, (ScrollEventArgs args) => receivedArgs = args));

        var scrollEventArgs = new ScrollEventArgs
        {
            NodeName = "#document",
            FirstChildBoundingClientRect = new BoundingClientRect { Top = -125 }
        };

        listener.RaiseScroll(scrollEventArgs);

        await component.WaitForAssertionAsync(() =>
        {
            receivedArgs.Should().BeSameAs(scrollEventArgs);
            component.Find("span").ClassList.Should().Contain("visible");
        });
    }

    [Test]
    public async Task ScrollEvent_IgnoresDocumentEvents_WithoutBoundingClientRect()
    {
        ScrollEventArgs receivedArgs = null;
        var (component, listener, _, _) = RenderScrollToTopWithTestDoubles(parameters => parameters
            .Add(x => x.TopOffset, 100)
            .Add(x => x.OnScroll, (ScrollEventArgs args) => receivedArgs = args));

        var scrollEventArgs = new ScrollEventArgs
        {
            NodeName = "#document"
        };

        listener.RaiseScroll(scrollEventArgs);

        await component.WaitForAssertionAsync(() =>
        {
            receivedArgs.Should().BeSameAs(scrollEventArgs);
            component.Find("span").ClassList.Should().Contain("hidden");
            component.Find("span").ClassList.Should().NotContain("visible");
        });
    }

    [Test]
    public async Task ScrollEvent_UsesConfiguredVisibilityClasses()
    {
        var (component, listener, _, _) = RenderScrollToTopWithTestDoubles(parameters => parameters
            .Add(x => x.TopOffset, 100)
            .Add(x => x.VisibleCssClass, "is-visible")
            .Add(x => x.HiddenCssClass, "is-hidden"));

        component.Find("span").ClassList.Should().Contain("is-hidden");
        component.Find("span").ClassList.Should().NotContain("hidden");

        listener.RaiseScroll(new ScrollEventArgs { NodeName = "DIV", ScrollTop = 101 });

        await component.WaitForAssertionAsync(() =>
        {
            component.Find("span").ClassList.Should().Contain("is-visible");
            component.Find("span").ClassList.Should().NotContain("visible");
            component.Find("span").ClassList.Should().NotContain("is-hidden");
        });
    }

    [Test]
    public async Task Click_ScrollsToTop_WithConfiguredBehavior_AndInvokesOnClick()
    {
        var clicked = false;
        var (component, _, scrollManagerMock, _) = RenderScrollToTopWithTestDoubles(parameters => parameters
            .Add(x => x.Selector, "#target")
            .Add(x => x.ScrollBehavior, ScrollBehavior.Auto)
            .Add(x => x.OnClick, (MouseEventArgs _) => clicked = true));

        await component.Find("span").ClickAsync();

        scrollManagerMock.Verify(x => x.ScrollToTopAsync("#target", ScrollBehavior.Auto), Times.Once);
        clicked.Should().BeTrue();
    }

    [Test]
    public async Task Click_UsesNullSelector_WhenNoSelectorWasConfigured()
    {
        var (component, _, scrollManagerMock, _) = RenderScrollToTopWithTestDoubles(parameters => parameters
            .Add(x => x.Selector, " "));

        await component.Find("span").ClickAsync();

        scrollManagerMock.Verify(x => x.ScrollToTopAsync(null, ScrollBehavior.Smooth), Times.Once);
    }

    [Test]
    public async Task Dispose_UnsubscribesAndDisposesScrollListener()
    {
        var (component, listener, _, _) = RenderScrollToTopWithTestDoubles();

        listener.SubscriptionCount.Should().Be(1);

        await component.Instance.DisposeAsync();

        listener.UnsubscriptionCount.Should().Be(1);
        listener.DisposeCallCount.Should().Be(1);
    }

    /// <summary>
    /// Renders <see cref="MudScrollToTop"/> with fake scroll infrastructure.
    /// </summary>
    private (IRenderedComponent<MudScrollToTop> Component, FakeScrollListener Listener, Mock<IScrollManager> ScrollManagerMock, FakeScrollListenerFactory Factory) RenderScrollToTopWithTestDoubles(Action<ComponentParameterCollectionBuilder<MudScrollToTop>> configure = null)
    {
        var listener = new FakeScrollListener();
        var factory = new FakeScrollListenerFactory(listener);
        var scrollManagerMock = new Mock<IScrollManager>();

        Context.Services.AddSingleton<IScrollListenerFactory>(factory);
        Context.Services.AddSingleton<IScrollManager>(scrollManagerMock.Object);

        var component = Context.Render<MudScrollToTop>(parameters =>
        {
            parameters.Add(x => x.ChildContent, (RenderFragment)(builder => builder.AddContent(0, "Scroll")));
            configure?.Invoke(parameters);
        });

        return (component, listener, scrollManagerMock, factory);
    }

    private sealed class FakeScrollListenerFactory(FakeScrollListener scrollListener) : IScrollListenerFactory
    {
        private bool _created;

        public List<string> CreateSelectors { get; } = [];

        public int CreateWithReportRateCallCount { get; private set; }

        public IScrollListener Create(string selector)
        {
            EnsureSingleUse();
            CreateSelectors.Add(selector);
            scrollListener.Selector = selector;
            return scrollListener;
        }

        public IScrollListener Create(string selector, int reportRateMs)
        {
            EnsureSingleUse();
            CreateWithReportRateCallCount++;
            CreateSelectors.Add(selector);
            scrollListener.Selector = selector;
            scrollListener.ReportRateMs = reportRateMs;
            return scrollListener;
        }

        private void EnsureSingleUse()
        {
            if (_created)
            {
                throw new InvalidOperationException("FakeScrollListenerFactory only supports one listener per test.");
            }

            _created = true;
        }
    }

    private sealed class FakeScrollListener : IScrollListener
    {
        private EventHandler<ScrollEventArgs> _onScroll;

        public string Selector { get; set; }

        public int ReportRateMs { get; set; }

        public int SubscriptionCount { get; private set; }

        public int UnsubscriptionCount { get; private set; }

        public int DisposeCallCount { get; private set; }

        public event EventHandler<ScrollEventArgs> OnScroll
        {
            add
            {
                _onScroll += value;
                SubscriptionCount++;
            }
            remove
            {
                _onScroll -= value;
                UnsubscriptionCount++;
            }
        }

        public ValueTask<ScrollEventArgs> GetCurrentScrollDataAsync()
        {
            return ValueTask.FromResult(new ScrollEventArgs());
        }

        public void RaiseScroll(ScrollEventArgs args)
        {
            _onScroll?.Invoke(this, args);
        }

        public ValueTask DisposeAsync()
        {
            DisposeCallCount++;
            return ValueTask.CompletedTask;
        }
    }
}
