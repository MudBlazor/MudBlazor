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
        var providerComp = Context.RenderComponent<MudPopoverProvider>();
        var comp = Context.RenderComponent<MudOverlay>();
        comp.Markup.Should().BeEmpty();
        providerComp.FindAll("div.mud-overlay").Count.Should().Be(0);
    }

    [Test]
    public void ShouldRenderWhenVisibleIsTrue()
    {
        var providerComp = Context.RenderComponent<MudPopoverProvider>();
        var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
        );

        providerComp.FindAll("div.mud-overlay").Count.Should().Be(1);
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
        var providerComp = Context.RenderComponent<MudPopoverProvider>();
        var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
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
        var providerComp = Context.RenderComponent<MudPopoverProvider>();
        var comp = Context.RenderComponent<OverlayVisibleBindingWithAutoCloseTest>();
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
    [TestCase(true, "", false, 0)] // Absolute is true
    [TestCase(false, "mud-overlay-dialog", false, 1)] // Dialog
    [TestCase(false, "mud-drawer-overlay", false, 2)] // Drawer
    [TestCase(false, "", true, 3)]  // Child content
    [TestCase(false, "", false, 4)] // no exception
    public void ShouldRender_SectionLocation(bool absolute, string expectedClass, bool hasChildContent, int testNum)
    {
        var childContent = "<div class='child-content'>Hello World</div>";
        var providerComp = Context.RenderComponent<MudPopoverProvider>();
        IRenderedComponent<MudOverlay> comp;
        if (hasChildContent)
        {
            comp = Context.RenderComponent<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .Add(p => p.Class, expectedClass)
            .Add(p => p.Absolute, absolute)
            .AddChildContent(childContent)
        );
        }
        else
        {
            comp = Context.RenderComponent<MudOverlay>(parameters => parameters
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
                countInProvider.Count.Should().Be(1);
                countInComp.Count.Should().Be(0);
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
        var comp = Context.RenderComponent<MudOverlay>(parameters => parameters
            .Add(p => p.Visible, true)
            .AddChildContent("<div class='child-content'>Hello World</div>")
        );

        comp.Find("div.child-content").TextContent.Should().Be("Hello World");
    }
}
