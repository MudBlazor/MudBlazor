using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents.Button;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class FabMenuTests : BunitTest
{
    [Test]
    public void RendersCorrectly()
    {
        var comp = Context.RenderComponent<FabMenuTest>();
        comp.FindAll(".mud-fab-menu").Count.Should().Be(1);
        comp.FindAll(".mud-fab-menu.open").Count.Should().Be(0);
        comp.FindAll(".mud-fab-menu-item").Count.Should().Be(3);

        comp.FindAll(".mud-fab-menu-button")[0].Click();
        comp.WaitForAssertion(() => { comp.FindAll(".mud-fab-menu.open").Count.Should().Be(1); });

        comp.FindAll(".mud-fab-menu-item")[0].Click();
        comp.WaitForAssertion(() => { comp.FindAll(".mud-fab-menu.open").Count.Should().Be(0); });
    }
}
