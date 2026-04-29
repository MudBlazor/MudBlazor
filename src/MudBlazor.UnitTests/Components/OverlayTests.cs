using System.Threading.Tasks;
using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MudBlazor.UnitTests.TestComponents.Overlay;

namespace MudBlazor.UnitTests.Components;

public class OverlayTests : BunitTest
{
    [Test]
    public void ShouldNotRenderByDefault()
    {
        var providerComp = Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudOverlay>();
        comp.Markup.Should().BeEmpty();
        providerComp.FindAll("div.mud-overlay").Count.Should().Be(0);
    }

    [Test]
    public void ShouldRenderWhenVisibleIsTrue()
    {
        var providerComp = Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
        );

        providerComp.FindAll("div.mud-overlay").Count.Should().Be(1);
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public async Task AutoClose_OnClick(bool autoClose)
    {
        var providerComp = Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.AutoClose, autoClose)
        );

        await providerComp.Find("div.mud-overlay").ClickAsync(new());

        if (autoClose)
        {
            providerComp.FindAll("div.mud-overlay").Count.Should().Be(0);
        }
        else
        {
            providerComp.FindAll("div.mud-overlay").Count.Should().Be(1);
        }
    }

    [Test]
    public async Task AutoClose_OnClosedEvent()
    {
        var counter = 0;
        void CloseHandler() => counter++;
        var providerComp = Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.AutoClose, true)
            .Add(p => p.OnClosed, CloseHandler)
        );

        await providerComp.Find("div.mud-overlay").ClickAsync(new());
        providerComp.FindAll("div.mud-overlay").Count.Should().Be(0);
        counter.Should().Be(1);
    }

    [Test]
    public async Task AutoClose_VisibleBinding()
    {
        var providerComp = Context.Render<MudPopoverProvider>();
        var comp = Context.Render<OverlayVisibleBindingWithAutoCloseTest>();
        IElement Button() => comp.Find("#showBtn");

        comp.Instance.Visible.Should().BeFalse();

        await Button().ClickAsync(new());
        comp.Instance.Visible.Should().BeTrue();

        await providerComp.Find("div.mud-overlay").ClickAsync(new());
        comp.Instance.Visible.Should().BeFalse();
    }

    [Test]
    public void ShouldApplyCorrectZIndex()
    {
        var providerComp = Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.ZIndex, 10)
        );

        providerComp.Find("div.mud-overlay").Attributes["style"].Value.Should().Contain("z-index:10");
    }

    [Test]
    [Arguments(true, true)]
    [Arguments(true, false)]
    [Arguments(false, true)]
    [Arguments(false, false)]
    public void ShouldApplyBackgroundColor(bool darkBackground, bool lightBackground)
    {
        var providerComp = Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.DarkBackground, darkBackground)
            .Add(p => p.LightBackground, lightBackground)
        );

        if (darkBackground || lightBackground)
        {
            if (darkBackground)
            {
                providerComp.Find("div.mud-overlay-scrim").ClassList.Should().Contain("mud-overlay-dark");
            }

            if (lightBackground)
            {
                providerComp.Find("div.mud-overlay-scrim").ClassList.Should().Contain("mud-overlay-light");
            }
        }
        else
        {
            providerComp.FindAll("div.mud-overlay-scrim").Count.Should().Be(0);
        }
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public void ShouldApplyAbsoluteClass(bool absolute)
    {
        var providerComp = Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.Absolute, absolute)
        );

        if (absolute)
        {
            comp.Find("div.mud-overlay").ClassList.Should().Contain("mud-overlay-absolute");
        }
        else
        {
            providerComp.Find("div.mud-overlay").ClassList.Should().NotContain("mud-overlay-absolute");
        }
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public void ShouldApplyCorrectPointerEvents(bool modal)
    {
        var providerComp = Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.Modal, modal)
        );

        if (modal)
        {
            providerComp.Find("div.mud-overlay").Attributes["style"].Value.Should().NotContain("pointer-events:none");
        }
        else
        {
            providerComp.Find("div.mud-overlay").Attributes["style"].Value.Should().Contain("pointer-events:none");
        }
    }

    [Test]
    public void ShouldHaveId()
    {
        var providerComp = Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
        );
        providerComp.Find("div.mud-overlay").Attributes["id"].Value.Should().NotBeNullOrEmpty();
    }

    [Test]
    [Arguments(true, "", false, 0)] // Absolute is true
    [Arguments(false, "mud-skip-overlay-section", false, 1)] // Dialog
    [Arguments(false, "", true, 3)]  // Child content
    [Arguments(false, "", false, 4)] // no exception
    public void ShouldRender_SectionLocation(bool absolute, string expectedClass, bool hasChildContent, int testNum)
    {
        var childContent = "<div class='child-content'>Hello World</div>";
        var providerComp = Context.Render<MudPopoverProvider>();
        IRenderedComponent<MudOverlay> comp;
        if (hasChildContent)
        {
            comp = Context.Render<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.Class, expectedClass)
            .Add(p => p.Absolute, absolute)
            .AddChildContent(childContent)
        );
        }
        else
        {
            comp = Context.Render<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.Class, expectedClass)
            .Add(p => p.Absolute, absolute)
        );
        }

        var countInProvider = providerComp.FindAll("div.mud-overlay");
        var countInComp = comp.FindAll("div.mud-overlay");

        switch (testNum)
        {
            case 0:
                countInProvider.Count.Should().Be(0);
                countInComp.Count.Should().Be(1);
                comp.Instance.RenderOutsideOfSection.Should().BeTrue();
                break;
            case 1:
                countInProvider.Count.Should().Be(0);
                countInComp.Count.Should().Be(1);
                comp.Instance.RenderOutsideOfSection.Should().BeTrue();
                break;
            case 2:
                countInProvider.Count.Should().Be(0);
                countInComp.Count.Should().Be(1);
                comp.Instance.RenderOutsideOfSection.Should().BeFalse();
                break;
            case 3:
                countInProvider.Count.Should().Be(0);
                countInComp.Count.Should().Be(1);
                comp.Instance.RenderOutsideOfSection.Should().BeTrue();
                comp.Find("div.child-content").TextContent.Should().Be("Hello World");
                break;
            case 4:
                countInProvider.Count.Should().Be(1);
                countInComp.Count.Should().Be(0);
                comp.Instance.RenderOutsideOfSection.Should().BeFalse();
                break;
        }
    }

    [Test]
    public void ShouldRenderChildContent()
    {
        var comp = Context.Render<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .AddChildContent("<div class='child-content'>Hello World</div>")
        );

        comp.Find("div.child-content").TextContent.Should().Be("Hello World");
    }

    [Test]
    [Arguments(true, true, false, true)]
    [Arguments(true, false, false, false)]
    [Arguments(true, false, true, false)]
    [Arguments(true, true, true, false)]
    [Arguments(false, true, false, false)]
    [Arguments(false, false, false, false)]
    [Arguments(false, false, true, false)]
    [Arguments(false, true, true, false)]
    public void CallsSubscribeAsyncOnPointerEventsNoneServiceWhenExpected(bool visible, bool autoClose, bool modal, bool callsStart)
    {
        Context.Services.Remove(ServiceDescriptor.Scoped<IPointerEventsNoneService, PointerEventsNoneService>());
        var serviceMock = new Mock<IPointerEventsNoneService>();
        serviceMock
            .Setup(s => s.SubscribeAsync(It.IsAny<IPointerEventsNoneObserver>(), It.IsAny<PointerEventsNoneOptions>()))
            .Returns(Task.CompletedTask)
            .Verifiable();
        Context.Services.AddScoped(_ => serviceMock.Object);

        var comp = Context.Render<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, visible)
            .Add(p => p.AutoClose, autoClose)
            .Add(p => p.Modal, modal)
        );

        serviceMock.Verify(s => s.SubscribeAsync(It.IsAny<IPointerEventsNoneObserver>(), It.IsAny<PointerEventsNoneOptions>()), callsStart ? Times.Once() : Times.Never());
    }

    [Test]
    public void Overlay_ShouldHaveElementId_AndMatchRenderedDivId()
    {
        // Arrange
        var providerComp = Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
        );

        // Act
        var elementId = ((IPointerEventsNoneObserver)comp.Instance).ElementId;
        var overlayDiv = providerComp.Find("div.mud-overlay");

        // Assert
        elementId.Should().NotBeNullOrWhiteSpace();
        overlayDiv.Id.Should().Be(elementId);
    }

    [Test]
    [Arguments(true, true)]
    [Arguments(true, false)]
    [Arguments(false, true)]
    [Arguments(false, false)]
    public async Task Overlay_HandleLockScrollChanges(bool absolute, bool lockscroll)
    {
        var scrollManagerMock = new Mock<IScrollManager>();
        Context.Services.AddSingleton(scrollManagerMock.Object);

        var visible = true;

        // === Initial: Visible = true, should lock scroll if conditions match ===
        var comp = Context.Render<MudOverlay>(parameters => parameters
            .Add(p => p.Absolute, absolute)
            .Bind(p => p.Visible, visible, p => visible = p)
            .Add(p => p.LockScroll, lockscroll)
        );

        var mudOverlay = comp.Instance;

        // Initial unlock state without JSRuntime
        scrollManagerMock.Verify(s => s.UnlockScrollAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());

        if (!absolute && lockscroll)
        {
            scrollManagerMock.Verify(s => s.LockScrollAsync("body", mudOverlay.LockScrollClass), Times.Once());
        }
        else
        {
            scrollManagerMock.Verify(s => s.LockScrollAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        // === Manually re-trigger HandleLockScrollChange (should not change counts) ===
        await mudOverlay.HandleLockScrollChange();

        if (!absolute && lockscroll)
        {
            scrollManagerMock.Verify(s => s.LockScrollAsync("body", mudOverlay.LockScrollClass), Times.Once());
        }
        else
        {
            scrollManagerMock.Verify(s => s.LockScrollAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        // === Toggle visible to false, expect unlock ===
        visible = false;
        await comp.SetParametersAndRenderAsync(p => p.Add(p => p.Visible, visible));

        if (!absolute && lockscroll)
        {
            scrollManagerMock.Verify(s => s.UnlockScrollAsync("body", mudOverlay.LockScrollClass), Times.Once());
        }
        else
        {
            scrollManagerMock.Verify(s => s.UnlockScrollAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        // open it
        visible = true;
        await comp.SetParametersAndRenderAsync(p => p.Add(p => p.Visible, visible));

        // close it by method
        await mudOverlay.CloseOverlayAsync();

        if (!absolute && lockscroll)
        {
            scrollManagerMock.Verify(s => s.UnlockScrollAsync("body", mudOverlay.LockScrollClass), Times.Exactly(2));
        }
        else
        {
            scrollManagerMock.Verify(s => s.UnlockScrollAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        // === Dispose component ===
        await mudOverlay.DisposeAsync();

        if (!absolute && lockscroll)
        {
            scrollManagerMock.Verify(s => s.UnlockScrollAsync("body", mudOverlay.LockScrollClass), Times.AtLeast(2));
        }
        else
        {
            scrollManagerMock.Verify(s => s.UnlockScrollAsync(It.IsAny<string>(), It.IsAny<string>()), Times.AtMostOnce());
        }
    }

    [Test]
    public async Task Overlay_StartClosed()
    {
        var jsRuntimeMock = new Mock<IJSRuntime>(MockBehavior.Loose);

        Context.Services.AddSingleton(typeof(IJSRuntime), jsRuntimeMock.Object);
        // verifies lockScroll was called once and 2 arguments were supplied
        jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>("mudScrollManager.lockScroll", It.Is<object[]>(y => y.Length == 2)))
            .ReturnsAsync(Mock.Of<IJSVoidResult>())
            .Verifiable();

        // Expect unlockScroll to NOT be called
        jsRuntimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>(
                "mudScrollManager.unlockScroll",
                It.IsAny<object[]>()))
            .Throws(new Exception("unlockScroll should not be called!"));

        var dialog = Context.Render<MudDialogProvider>();
        var comp = Context.Render<OverlayScrollLockedTest>();
        // click the button opening dialog
        var button = comp.Find("button");
        await button.ClickAsync();
        // verify dialog is open
        await comp.WaitForAssertionAsync(() => dialog.FindComponent<MudOverlay>().Should().NotBeNull());

        // verify lockScroll was called
        jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>(
                "mudScrollManager.lockScroll",
                It.IsAny<object[]>()),
            Times.Once);

        // verify unlockScroll was NOT called
        jsRuntimeMock.Verify(
            x => x.InvokeAsync<IJSVoidResult>(
                "mudScrollManager.unlockScroll",
                It.IsAny<object[]>()),
            Times.Never);
    }
}
