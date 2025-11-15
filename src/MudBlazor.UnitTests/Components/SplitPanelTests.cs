using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents.SplitPanel;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class SplitPanelTests : BunitTest
{
    [Test]
    public void RendersCorrectly()
    {
        var comp = Context.RenderComponent<SplitPanelTest>();
        comp.FindAll(".mud-split-panel").Count.Should().Be(1);

        var childPanels = comp.FindAll(".child-panel");
        childPanels.Count.Should().Be(2);
        childPanels[0].ToMarkup().Should().BeEquivalentTo(childPanels[1].ToMarkup());
    }

    [Test]
    public void RendersCorrectlyHorizontal()
    {
        var comp = Context.RenderComponent<SplitPanelTest>(
            ComponentParameter.CreateParameter("Horizontal", true));
        comp.FindAll(".mud-split-panel.flex-column").Count.Should().Be(1);

        var childPanels = comp.FindAll(".child-panel");
        childPanels.Count.Should().Be(2);
        childPanels[0].ToMarkup().Should().BeEquivalentTo(childPanels[1].ToMarkup());
    }
}
