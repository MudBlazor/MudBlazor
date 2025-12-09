using Bunit;
using FluentAssertions;
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
    public void RendersCorrectlyOnClick()
    {
        var comp = Context.Render<FabMenuTest>();

        comp.FindAll(".mud-fab-menu-button")[0].Click();
        comp.WaitForAssertion(() => { comp.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(1); });

        comp.FindAll(".mud-fab-menu-item")[0].Click();
        comp.WaitForAssertion(() => { comp.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(0); });
    }

    [Test]
    public void RendersCorrectlyOnTouch()
    {
        var compNoHover = Context.Render<FabMenuTest>();

        compNoHover.FindAll(".mud-fab-menu-button")[0].TouchStart();
        compNoHover.FindAll(".mud-fab-menu-button")[0].Click();
        compNoHover.WaitForAssertion(() => { compNoHover.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(1); });

        compNoHover.FindAll(".mud-fab-menu-button")[0].TouchStart();
        compNoHover.FindAll(".mud-fab-menu-item")[0].Click();
        compNoHover.WaitForAssertion(() => { compNoHover.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(0); });

        var compHover = Context.Render<FabMenuTest>(parameters => parameters.Add(p => p.OpenOnMouseHover, true));

        compHover.FindAll(".mud-fab-menu-button")[0].TouchStart();
        compHover.FindAll(".mud-fab-menu-button")[0].Click();
        compHover.WaitForAssertion(() => { compHover.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(1); });

        compHover.FindAll(".mud-fab-menu-button")[0].TouchStart();
        compHover.FindAll(".mud-fab-menu-item")[0].Click();
        compHover.WaitForAssertion(() => { compHover.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(0); });
    }

    [Test]
    public void RendersCorrectlyOnHover()
    {
        var comp = Context.Render<FabMenuTest>(parameters => parameters.Add(p => p.OpenOnMouseHover, true));

        comp.FindAll(".mud-fab-menu-container")[0].TriggerEvent("onmouseenter", new MouseEventArgs());
        comp.WaitForAssertion(() => { comp.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(1); });

        comp.FindAll(".mud-fab-menu-item")[0].Click();
        comp.WaitForAssertion(() => { comp.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(0); });

        comp.FindAll(".mud-fab-menu-container")[0].TriggerEvent("onmouseenter", new MouseEventArgs());
        comp.WaitForAssertion(() => { comp.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(1); });

        comp.FindAll(".mud-fab-menu-container")[0].TriggerEvent("onmouseleave", new MouseEventArgs());
        comp.WaitForAssertion(() => { comp.FindAll(".mud-fab-menu.mud-fab-menu-open").Count.Should().Be(0); });
    }
}
