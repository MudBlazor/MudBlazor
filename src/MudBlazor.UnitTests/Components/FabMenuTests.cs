using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents.Button;
using System.Threading.Tasks;

namespace MudBlazor.UnitTests.Components;
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

    [Test]
    [Arguments(Direction.Top, "mud-fab-menu-direction-top")]
    [Arguments(Direction.Bottom, "mud-fab-menu-direction-bottom")]
    [Arguments(Direction.Left, "mud-fab-menu-direction-left")]
    [Arguments(Direction.Right, "mud-fab-menu-direction-right")]
    [Arguments(Direction.Start, "mud-fab-menu-direction-start")]
    [Arguments(Direction.End, "mud-fab-menu-direction-end")]
    public void AppliesDirectionClass(Direction direction, string expectedClass)
    {
        var comp = Context.Render<MudFabMenu>(parameters => parameters
            .Add(p => p.Direction, direction));

        comp.Find(".mud-fab-menu").ClassList.Contains(expectedClass).Should().BeTrue();
    }

    [Test]
    [Arguments(Origin.TopLeft, "mud-fab-anchor-top-left")]
    [Arguments(Origin.TopCenter, "mud-fab-anchor-top-center")]
    [Arguments(Origin.TopRight, "mud-fab-anchor-top-right")]
    [Arguments(Origin.CenterLeft, "mud-fab-anchor-center-left")]
    [Arguments(Origin.CenterCenter, "mud-fab-anchor-center-center")]
    [Arguments(Origin.CenterRight, "mud-fab-anchor-center-right")]
    [Arguments(Origin.BottomLeft, "mud-fab-anchor-bottom-left")]
    [Arguments(Origin.BottomCenter, "mud-fab-anchor-bottom-center")]
    [Arguments(Origin.BottomRight, "mud-fab-anchor-bottom-right")]
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
}