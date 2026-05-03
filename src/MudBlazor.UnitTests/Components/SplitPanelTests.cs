using AwesomeAssertions;
using Bunit;
using MudBlazor.UnitTests.TestComponents.SplitPanel;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class SplitPanelTests : BunitTest
{
    [Test]
    public void RendersCorrectly()
    {
        var comp = Context.Render<SplitPanelTest>();
        comp.FindAll(".mud-split-panel").Count.Should().Be(1);

        var childPanels = comp.FindAll(".child-panel");
        childPanels.Count.Should().Be(2);
        childPanels[0].ToMarkup().Should().BeEquivalentTo(childPanels[1].ToMarkup());
    }

    [Test]
    public void VisualParametersRenderCorrectly()
    {
        var comp = Context.Render<MudSplitPanel>(parameters => parameters
            .Add(p => p.FirstPanel, builder => builder.AddContent(0, "First Panel"))
            .Add(p => p.SecondPanel, builder => builder.AddContent(0, "Second Panel"))
            .Add(p => p.Horizontal, true)
            .Add(p => p.UseAsOverlay, true)
            .Add(p => p.Elevation, 1)
            .Add(p => p.Padding, 2)
            .Add(p => p.Rounded, true)
            .Add(p => p.Transparent, true)
        );
        comp.FindAll(".mud-split-panel.flex-column.absolute").Count.Should().Be(1);

        var childPanels = comp.FindAll(".child-panel");
        foreach (var panel in childPanels)
        {
            panel.ClassList.Should().Contain("mud-elevation-1");
            panel.ClassList.Should().Contain("transparent");
            panel.ClassList.Should().Contain("pa-2");
            panel.ClassList.Should().Contain("rounded");
        }

        comp.FindAll(".divider.horizontal").Count.Should().Be(1);
    }

    [Test]
    public async Task ExecutesBuildDestroyJsCalls()
    {
        Context.Render<SplitPanelTest>();
        var invocation = Context.JSInterop.VerifyInvoke("mudSplitPanel.build");
        invocation.Arguments.Count.Should().Be(6);

        await Context.DisposeComponentsAsync();
        invocation = Context.JSInterop.VerifyInvoke("mudSplitPanel_destroy");
        invocation.Arguments.Count.Should().Be(1);
    }

    [Test]
    public async Task ExecutesUpdateJsCall()
    {
        var comp = Context.Render<SplitPanelTest>();
        await comp.SetParametersAndRenderAsync(parameters => parameters.Add(c => c.Horizontal, true));

        var invocation = Context.JSInterop.VerifyInvoke("mudSplitPanel_update");
        invocation.Arguments.Count.Should().Be(5);
    }

    [Test]
    public async Task ExecutesResetDividerPositionJsCall()
    {
        var comp = Context.Render<SplitPanelTest>();
        await comp.Instance.ResetDividerPositionAsync();

        var invocation = Context.JSInterop.VerifyInvoke("mudSplitPanel_resetDividerPosition");
        invocation.Arguments.Count.Should().Be(1);
    }

    [Test]
    public async Task ExecutesSetDividerPositionJsCall()
    {
        var comp = Context.Render<SplitPanelTest>();
        await comp.Instance.SetDividerPositionAsync(123);

        var invocation = Context.JSInterop.VerifyInvoke("mudSplitPanel_setDividerPosition");
        invocation.Arguments.Count.Should().Be(2);
    }

    [Test]
    public async Task ExecutesGetDividerPositionJsCall()
    {
        var comp = Context.Render<SplitPanelTest>();
        await comp.Instance.GetDividerPositionAsync();

        var invocation = Context.JSInterop.VerifyInvoke("mudSplitPanel_getDividerPosition");
        invocation.Arguments.Count.Should().Be(1);
    }
}
