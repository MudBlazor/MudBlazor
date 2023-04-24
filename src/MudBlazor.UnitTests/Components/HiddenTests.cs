
#pragma warning disable CS1998 // async without await

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MudBlazor.Services;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class HiddenTests : BunitTest
    {
        [Test]
        [TestCase(false, false, false)]
        [TestCase(false, true, true)]
        [TestCase(true, false, true)]
        [TestCase(true, true, false)]
        public void Content_Visible(bool mediaResult, bool invert, bool isHidden)
        {
            var listenerMock = new Mock<IBreakpointService>();
            listenerMock.Setup(x => x.SubscribeAsync(It.IsAny<Action<Breakpoint>>())).ReturnsAsync(new BreakpointServiceSubscribeResult(Guid.NewGuid(), Breakpoint.Md)).Verifiable();
            listenerMock.Setup(x => x.IsMediaSize(Breakpoint.Lg, Breakpoint.Md)).Returns(mediaResult).Verifiable();

            Context.Services.AddSingleton(sp => listenerMock.Object);

            var comp = Context.RenderComponent<SimpleMudHiddenTest>(p =>
            {
                p.Add(x => x.Breakpoint, Breakpoint.Lg);
                p.Add(x => x.Invert, invert);
            });

            if (isHidden == true)
            {
                Assert.Throws<ElementNotFoundException>(() => comp.Find("p"));
            }
            else
            {
                comp.Find("p").TextContent.Should().Be("MudHidden content");
            }

            listenerMock.Verify();
        }

        [Test]
        public void SizeChanged()
        {
            Action<Breakpoint> callback = null;

            var listenerMock = new Mock<IBreakpointService>();
            listenerMock.Setup(x => x.SubscribeAsync(It.IsAny<Action<Breakpoint>>()))
                .ReturnsAsync(new BreakpointServiceSubscribeResult(Guid.NewGuid(), Breakpoint.Md))
                .Callback<Action<Breakpoint>>(x => callback = x)
                .Verifiable();

            listenerMock.Setup(x => x.IsMediaSize(Breakpoint.Lg, Breakpoint.Md)).Returns(false).Verifiable();
            listenerMock.Setup(x => x.IsMediaSize(Breakpoint.Lg, Breakpoint.Xs)).Returns(true).Verifiable();
            listenerMock.Setup(x => x.IsMediaSize(Breakpoint.Lg, Breakpoint.Xl)).Returns(false).Verifiable();

            Context.Services.AddSingleton(sp => listenerMock.Object);

            var comp = Context.RenderComponent<SimpleMudHiddenTest>(p =>
            {
                p.Add(x => x.Breakpoint, Breakpoint.Lg);
                p.Add(x => x.Invert, false);
            });

            comp.Find("p").TextContent.Should().Be("MudHidden content");

            comp.InvokeAsync(() => callback.Invoke(Breakpoint.Xs));

            Assert.Throws<ElementNotFoundException>(() => comp.Find("p"));

            comp.InvokeAsync(() => callback.Invoke(Breakpoint.Xl));
            comp.Find("p").TextContent.Should().Be("MudHidden content");

            comp.Instance.HiddenChangedHistory.Should().HaveCount(3).And.BeEquivalentTo(new[] { false, true, false });

            listenerMock.Verify();
        }

        [Test]
        public void InvertChangedAfterInitializing()
        {
            var listenerMock = new Mock<IBreakpointService>();
            listenerMock.Setup(x => x.SubscribeAsync(It.IsAny<Action<Breakpoint>>())).ReturnsAsync(new BreakpointServiceSubscribeResult(Guid.NewGuid(), Breakpoint.Md)).Verifiable();
            listenerMock.Setup(x => x.IsMediaSize(Breakpoint.Lg, Breakpoint.Md)).Returns(false).Verifiable();

            Context.Services.AddSingleton(sp => listenerMock.Object);

            var comp = Context.RenderComponent<SimpleMudHiddenTest>(p =>
            {
                p.Add(x => x.Breakpoint, Breakpoint.Lg);
                p.Add(x => x.Invert, false);
            });

            comp.Find("p").TextContent.Should().Be("MudHidden content");

            comp.SetParametersAndRender(p => p.Add(x => x.Invert, true));

            Assert.Throws<ElementNotFoundException>(() => comp.Find("p"));

            listenerMock.Verify(x => x.IsMediaSize(Breakpoint.Lg, Breakpoint.Md), Times.Exactly(4));
            listenerMock.Verify();
        }

        [Test]
        public void ReferenceBreakpointChangedAfterInitializing()
        {
            var listenerMock = new Mock<IBreakpointService>();
            listenerMock.Setup(x => x.SubscribeAsync(It.IsAny<Action<Breakpoint>>())).ReturnsAsync(new BreakpointServiceSubscribeResult(Guid.NewGuid(), Breakpoint.Md)).Verifiable();
            listenerMock.Setup(x => x.IsMediaSize(Breakpoint.Lg, Breakpoint.Md)).Returns(false).Verifiable();
            listenerMock.Setup(x => x.IsMediaSize(Breakpoint.Xs, Breakpoint.Md)).Returns(true).Verifiable();

            Context.Services.AddSingleton(sp => listenerMock.Object);

            var comp = Context.RenderComponent<SimpleMudHiddenTest>(p =>
            {
                p.Add(x => x.Breakpoint, Breakpoint.Lg);
                p.Add(x => x.Invert, false);
            });

            comp.Find("p").TextContent.Should().Be("MudHidden content");

            comp.SetParametersAndRender(p => p.Add(x => x.Breakpoint, Breakpoint.Xs));

            Assert.Throws<ElementNotFoundException>(() => comp.Find("p"));

            listenerMock.Verify(x => x.IsMediaSize(Breakpoint.Lg, Breakpoint.Md), Times.Exactly(2));
            listenerMock.Verify(x => x.IsMediaSize(Breakpoint.Xs, Breakpoint.Md), Times.Exactly(2));
            listenerMock.Verify();
        }

        [Test]
        public void SizeChangedToNone()
        {
            Action<Breakpoint> callback = null;

            var listenerMock = new Mock<IBreakpointService>();
            listenerMock.Setup(x => x.SubscribeAsync(It.IsAny<Action<Breakpoint>>()))
                .ReturnsAsync(new BreakpointServiceSubscribeResult(Guid.NewGuid(), Breakpoint.Md))
                .Callback<Action<Breakpoint>>(x => callback = x)
                .Verifiable();

            listenerMock.Setup(x => x.IsMediaSize(Breakpoint.Lg, Breakpoint.Md)).Returns(false).Verifiable();

            Context.Services.AddSingleton(sp => listenerMock.Object);

            var comp = Context.RenderComponent<SimpleMudHiddenTest>(p =>
            {
                p.Add(x => x.Breakpoint, Breakpoint.Lg);
                p.Add(x => x.Invert, false);
            });

            comp.Find("p").TextContent.Should().Be("MudHidden content");

            comp.InvokeAsync(() => callback.Invoke(Breakpoint.None));
            comp.Find("p").TextContent.Should().Be("MudHidden content");

            comp.Instance.HiddenChangedHistory.Should().ContainSingle().And.BeEquivalentTo(new[] { false });

            listenerMock.Verify();
        }

        [Test]
        public void WithinMudBreakpointProvider()
        {
            var listenerMock = new Mock<IBreakpointService>();
            listenerMock.Setup(x => x.SubscribeAsync(It.IsAny<Action<Breakpoint>>()))
                .ReturnsAsync(new BreakpointServiceSubscribeResult(Guid.NewGuid(), Breakpoint.Md))
                .Verifiable();

            listenerMock.Setup(x => x.IsMediaSize(Breakpoint.Lg, Breakpoint.Md)).Returns(false).Verifiable();

            Context.Services.AddSingleton(sp => listenerMock.Object);

            var comp = Context.RenderComponent<BreakpointProviderWithMudHiddenTest>(p =>
            {
                p.Add(x => x.Breakpoint, Breakpoint.Lg);
            });

            var items = comp.FindAll("p");

            items.Should().HaveCount(4);

            for (int i = 0; i < 4; i++)
            {
                var item = items[i];
                item.TextContent.Should().Be($"MudHidden content {i + 1}");
            }

            listenerMock.Verify(x => x.SubscribeAsync(It.IsAny<Action<Breakpoint>>()), Times.Once());
            listenerMock.Verify(x => x.IsMediaSize(Breakpoint.Lg, Breakpoint.Md), Times.Exactly(8));
            listenerMock.Verify();
        }

        [Test]
        public void WithinMudBreakpointProvider_UpdateBreakpointValue()
        {
            Action<Breakpoint> callback = null;

            var listenerMock = new Mock<IBreakpointService>();
            listenerMock.Setup(x => x.SubscribeAsync(It.IsAny<Action<Breakpoint>>()))
                .ReturnsAsync(new BreakpointServiceSubscribeResult(Guid.NewGuid(), Breakpoint.Md))
                .Callback<Action<Breakpoint>>(x => callback = x)
                .Verifiable();

            listenerMock.Setup(x => x.IsMediaSize(Breakpoint.Lg, Breakpoint.Md)).Returns(false).Verifiable();
            listenerMock.Setup(x => x.IsMediaSize(Breakpoint.Lg, Breakpoint.Xs)).Returns(true).Verifiable();
            listenerMock.Setup(x => x.IsMediaSize(Breakpoint.Lg, Breakpoint.Sm)).Returns(false).Verifiable();
            Context.Services.AddSingleton(sp => listenerMock.Object);

            var comp = Context.RenderComponent<BreakpointProviderWithMudHiddenTest>(p =>
            {
                p.Add(x => x.Breakpoint, Breakpoint.Lg);
            });

            var items = comp.FindAll("p");

            items.Should().HaveCount(4);

            for (int i = 0; i < 4; i++)
            {
                var item = items[i];
                item.TextContent.Should().Be($"MudHidden content {i + 1}");
            }

            comp.InvokeAsync(() => callback(Breakpoint.Xs));
            items = comp.FindAll("p");
            items.Should().BeEmpty();

            comp.InvokeAsync(() => callback(Breakpoint.Sm));
            items = comp.FindAll("p");

            items.Should().HaveCount(4);

            for (int i = 0; i < 4; i++)
            {
                var item = items[i];
                item.TextContent.Should().Be($"MudHidden content {i + 1}");
            }

            comp.Instance.BreakpointChangedHistory.Should().HaveCount(3).And.BeEquivalentTo(new[] { Breakpoint.Md, Breakpoint.Xs, Breakpoint.Sm });

            listenerMock.Verify(x => x.SubscribeAsync(It.IsAny<Action<Breakpoint>>()), Times.Once());
            listenerMock.Verify(x => x.IsMediaSize(Breakpoint.Lg, Breakpoint.Md), Times.Exactly(8));
            listenerMock.Verify(x => x.IsMediaSize(Breakpoint.Lg, Breakpoint.Xs), Times.Exactly(16));
            listenerMock.Verify(x => x.IsMediaSize(Breakpoint.Lg, Breakpoint.Sm), Times.Exactly(16));

            listenerMock.Verify();
        }

        [Test]
        public void TestSemaphore_RenderInParallel()
        {
            Mock<IBrowserWindowSizeProvider> sizeMock = new Mock<IBrowserWindowSizeProvider>(MockBehavior.Strict);
            sizeMock.Setup(x => x.GetBrowserWindowSize()).ReturnsAsync(new BrowserWindowSize { Width = 1920 });

            Mock<IJSRuntime> _jsruntimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);

            _jsruntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.listenForResize", It.IsAny<object[]>()))
                .ReturnsAsync(Mock.Of<IJSVoidResult>(), TimeSpan.FromMilliseconds(200)).Verifiable();
            _jsruntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudResizeListenerFactory.cancelListeners", It.IsAny<object[]>()))
    .ReturnsAsync(Mock.Of<IJSVoidResult>);

            BreakpointService service = new BreakpointService(_jsruntimeMock.Object, sizeMock.Object);

            Context.Services.AddSingleton<IBreakpointService, BreakpointService>(sp => service);

            var comp = Context.RenderComponent<RenderMultipleHiddenInParallel>();

            comp.WaitForAssertion(() => comp.FindAll(".xl").Should().HaveCount(10), TimeSpan.FromSeconds(1));
            comp.WaitForAssertion(() => comp.FindAll(".lg-and-up").Should().HaveCount(10), TimeSpan.FromSeconds(1));
            comp.WaitForAssertion(() => comp.FindAll(".md-and-up").Should().HaveCount(10), TimeSpan.FromSeconds(1));
            comp.WaitForAssertion(() => comp.FindAll(".sm-and-up").Should().HaveCount(10), TimeSpan.FromSeconds(1));
        }
    }
}
