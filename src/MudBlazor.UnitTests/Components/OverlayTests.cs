using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents.Overlay;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class OverlayTests : BunitTest
{
    [Test]
    public void ShouldNotRenderByDefault()
    {
        var comp = Context.RenderComponent<MudOverlay>();
        comp.Markup.Should().BeEmpty();
    }

    [Test]
    public void ShouldRenderWhenVisibleIsTrue()
    {
        var providerComp = Context.RenderComponent<MudPopoverProvider>();
        var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
        );

        providerComp.Markup.Should().NotBeEmpty();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task AutoClose_OnClick(bool autoClose)
    {
        var providerComp = Context.RenderComponent<MudPopoverProvider>();
        var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.AutoClose, autoClose)
        );

        await providerComp.Find("div.mud-overlay").ClickAsync(new());

        if (autoClose)
        {
            providerComp.Markup.Should().BeEmpty();
        }
        else
        {
            providerComp.Markup.Should().NotBeEmpty();
        }
    }

    [Test]
    public async Task AutoClose_OnClosedEvent()
    {
        var counter = 0;
        void CloseHandler() => counter++;
        var providerComp = Context.RenderComponent<MudPopoverProvider>();
        var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.AutoClose, true)
            .Add(p => p.OnClosed, CloseHandler)
        );

        await providerComp.Find("div.mud-overlay").ClickAsync(new());
        comp.Markup.Trim().Should().BeEmpty();
        counter.Should().Be(1);
    }

    [Test]
    public async Task AutoClose_VisibleBinding()
    {
        var comp = Context.RenderComponent<OverlayVisibleBindingWithAutoCloseTest>();
        IElement Button() => comp.Find("#showBtn");

        comp.Instance.Visible.Should().BeFalse();

        await Button().ClickAsync(new());
        comp.Instance.Visible.Should().BeTrue();

        await comp.Find("div.mud-overlay").ClickAsync(new());
        comp.Instance.Visible.Should().BeFalse();
    }

    [Test]
    public void ShouldApplyCorrectZIndex()
    {
        var providerComp = Context.RenderComponent<MudPopoverProvider>();
        var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.ZIndex, 10)
        );

        providerComp.Find("div.mud-overlay").Attributes["style"].Value.Should().Contain("z-index:10");
    }

    [Test]
    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]
    public void ShouldApplyBackgroundColor(bool darkBackground, bool lightBackground)
    {
        var providerComp = Context.RenderComponent<MudPopoverProvider>();
        var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
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
    [TestCase(true)]
    [TestCase(false)]
    public void ShouldApplyAbsoluteClass(bool absolute)
    {
        var providerComp = Context.RenderComponent<MudPopoverProvider>();
        var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
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
    public void ShouldRenderChildContent()
    {
        var providerComp = Context.RenderComponent<MudPopoverProvider>();
        var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .AddChildContent("<div class='child-content'>Hello World</div>")
        );

        providerComp.Find("div.child-content").TextContent.Should().Be("Hello World");
    }
}
