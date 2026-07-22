using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents.Button;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class FabMenuTests : BunitTest
{
    [Test]
    public void RendersCorrectly()
    {
        var comp = Context.Render<FabMenuTest>();
        comp.FindAll(".mud-fab-menu").Count.Should().Be(1);
        comp.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(0);
        comp.FindAll(".mud-fab-menu-item").Count.Should().Be(3);
    }

    [Test]
    public async Task RendersCorrectlyOnClick()
    {
        var comp = Context.Render<FabMenuTest>();

        await comp.FindAll(".mud-fab-menu-button")[0].ClickAsync();
        await comp.WaitForAssertionAsync(() => { comp.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(1); });

        await comp.FindAll(".mud-fab-menu-item")[0].ClickAsync();
        await comp.WaitForAssertionAsync(() => { comp.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(0); });
    }

    [Test]
    public async Task RendersCorrectlyOnTouch()
    {
        var compNoHover = Context.Render<FabMenuTest>();

        compNoHover.FindAll(".mud-fab-menu-button")[0].TouchStart();
        await compNoHover.FindAll(".mud-fab-menu-button")[0].ClickAsync();
        await compNoHover.WaitForAssertionAsync(() => { compNoHover.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(1); });

        compNoHover.FindAll(".mud-fab-menu-button")[0].TouchStart();
        await compNoHover.FindAll(".mud-fab-menu-item")[0].ClickAsync();
        await compNoHover.WaitForAssertionAsync(() => { compNoHover.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(0); });

        var compHover = Context.Render<FabMenuTest>(parameters => parameters.Add(p => p.OpenOnMouseHover, true));

        compHover.FindAll(".mud-fab-menu-button")[0].TouchStart();
        await compHover.FindAll(".mud-fab-menu-button")[0].ClickAsync();
        await compHover.WaitForAssertionAsync(() => { compHover.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(1); });

        compHover.FindAll(".mud-fab-menu-button")[0].TouchStart();
        await compHover.FindAll(".mud-fab-menu-item")[0].ClickAsync();
        await compHover.WaitForAssertionAsync(() => { compHover.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(0); });
    }

    [Test]
    public async Task RendersCorrectlyOnHover()
    {
        var comp = Context.Render<FabMenuTest>(parameters => parameters.Add(p => p.OpenOnMouseHover, true));

        await comp.FindAll(".mud-fab-menu-container")[0].MouseEnterAsync(new MouseEventArgs());
        await comp.WaitForAssertionAsync(() => { comp.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(1); });

        await comp.FindAll(".mud-fab-menu-item")[0].ClickAsync();
        await comp.WaitForAssertionAsync(() => { comp.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(0); });

        await comp.FindAll(".mud-fab-menu-container")[0].MouseEnterAsync(new MouseEventArgs());
        await comp.WaitForAssertionAsync(() => { comp.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(1); });

        await comp.FindAll(".mud-fab-menu-container")[0].MouseLeaveAsync(new MouseEventArgs());
        await comp.WaitForAssertionAsync(() => { comp.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(0); });
    }

    [TestCase(Direction.Top, "mud-fab-menu-direction-top")]
    [TestCase(Direction.Bottom, "mud-fab-menu-direction-bottom")]
    [TestCase(Direction.Left, "mud-fab-menu-direction-left")]
    [TestCase(Direction.Right, "mud-fab-menu-direction-right")]
    [TestCase(Direction.Start, "mud-fab-menu-direction-start")]
    [TestCase(Direction.End, "mud-fab-menu-direction-end")]
    public void AppliesDirectionClass(Direction direction, string expectedClass)
    {
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.Direction, direction));

        comp.Find(".mud-fab-menu").ClassList.Contains(expectedClass).Should().BeTrue();
    }

    [TestCase(Origin.TopLeft, "mud-fab-anchor-top-left")]
    [TestCase(Origin.TopCenter, "mud-fab-anchor-top-center")]
    [TestCase(Origin.TopRight, "mud-fab-anchor-top-right")]
    [TestCase(Origin.CenterLeft, "mud-fab-anchor-center-left")]
    [TestCase(Origin.CenterCenter, "mud-fab-anchor-center-center")]
    [TestCase(Origin.CenterRight, "mud-fab-anchor-center-right")]
    [TestCase(Origin.BottomLeft, "mud-fab-anchor-bottom-left")]
    [TestCase(Origin.BottomCenter, "mud-fab-anchor-bottom-center")]
    [TestCase(Origin.BottomRight, "mud-fab-anchor-bottom-right")]
    public void AppliesAnchorClassWhenFixed(Origin anchor, string expectedClass)
    {
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.Fixed, true)
            .Add(p => p.Anchor, anchor));

        var container = comp.Find(".mud-fab-menu-container");
        container.ClassList.Contains("fixed").Should().BeTrue();
        container.ClassList.Contains(expectedClass).Should().BeTrue();
    }

    [Test]
    public void DoesNotApplyAnchorClassWhenNotFixed()
    {
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.Anchor, Origin.TopLeft));

        var container = comp.Find(".mud-fab-menu-container");
        container.ClassList.Contains("fixed").Should().BeFalse();
        container.ClassList.Contains("mud-fab-anchor-top-left").Should().BeFalse();
    }

    [Test]
    [Combinatorial]
    public void FabMenuItem_ShouldRenderAnchorIfHrefIsSet(
        [Values("", "ASDF", "_blank")] string target,
        [Values(null, "noopener", "nofollow")] string rel)
    {
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .AddChildContent<MudFabMenuItem>(item => item
                .Add(x => x.Href, "https://example.com")
                .Add(x => x.Target, target)
                .Add(x => x.Rel, rel)
                .Add(x => x.Label, "Link")));

        var item = comp.Find(".mud-fab-menu-item");

        item.TagName.Should().Be("A");
        item.GetAttribute("href").Should().Be("https://example.com");
        item.GetAttribute("target").Should().Be(target);

        var expectedRel = rel ?? (target == "_blank" ? "noopener" : null);
        item.GetAttribute("rel").Should().Be(expectedRel);
    }
}
