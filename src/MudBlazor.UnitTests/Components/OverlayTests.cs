using System.Threading.Tasks;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
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
        var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
        );

        comp.Markup.Should().NotBeEmpty();
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task AutoClose_OnClick(bool autoClose)
    {
        var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.AutoClose, autoClose)
        );

        await comp.Find("div.mud-overlay").ClickAsync(new());

        if (autoClose)
        {
            comp.Markup.Should().BeEmpty();
        }
        else
        {
            comp.Markup.Should().NotBeEmpty();
        }
    }

    [Test]
    public async Task AutoClose_OnClosedEvent()
    {
        var counter = 0;
        void CloseHandler() => counter++;

        var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.AutoClose, true)
            .Add(p => p.OnClosed, CloseHandler)
        );

        await comp.Find("div.mud-overlay").ClickAsync(new());
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
        var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.ZIndex, 10)
        );

        comp.Find("div.mud-overlay").Attributes["style"].Value.Should().Contain("z-index:10");
    }

    [Test]
    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]
    public void ShouldApplyBackgroundColor(bool darkBackground, bool lightBackground)
    {
        var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.DarkBackground, darkBackground)
            .Add(p => p.LightBackground, lightBackground)
        );

        if (darkBackground || lightBackground)
        {
            if (darkBackground)
            {
                comp.Find("div.mud-overlay-scrim").ClassList.Should().Contain("mud-overlay-dark");
            }

            if (lightBackground)
            {
                comp.Find("div.mud-overlay-scrim").ClassList.Should().Contain("mud-overlay-light");
            }
        }
        else
        {
            comp.FindAll("div.mud-overlay-scrim").Count.Should().Be(0);
        }
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void ShouldApplyAbsoluteClass(bool absolute)
    {
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
            comp.Find("div.mud-overlay").ClassList.Should().NotContain("mud-overlay-absolute");
        }
    }

    [Test]
    public void ShouldRenderChildContent()
    {
        var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .AddChildContent("<div class='child-content'>Hello World</div>")
        );

        comp.Find("div.child-content").TextContent.Should().Be("Hello World");
    }
}
