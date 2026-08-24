using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class SplitButtonTests : BunitTest
{
    /// <summary>
    /// The split button renders a grouped pair of segments: the primary action and the menu toggle.
    /// </summary>
    [Test]
    public void RendersPrimaryAndToggleSegmentsInAGroup()
    {
        Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudSplitButton>(parameters => parameters
            .Add(p => p.Label, "Reply"));

        comp.Find(".mud-button-group-root").GetAttribute("role").Should().Be("group");
        comp.FindAll(".mud-split-button-primary").Count.Should().Be(1);
        comp.FindAll(".mud-split-button-toggle").Count.Should().Be(1);
        comp.Find(".mud-split-button-primary").TextContent.Trim().Should().Be("Reply");
    }

    /// <summary>
    /// Clicking the primary segment invokes OnClick exactly once and leaves the menu closed.
    /// </summary>
    [Test]
    public async Task PrimaryClickInvokesOnClick()
    {
        var clicks = 0;
        Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudSplitButton>(parameters => parameters
            .Add(p => p.Label, "Reply")
            .Add(p => p.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => clicks++))
            .Add(p => p.ChildContent, MenuItems("Reply All")));

        await comp.Find(".mud-split-button-primary").ClickAsync(new MouseEventArgs());

        clicks.Should().Be(1);
        comp.FindAll(".mud-popover-open").Count.Should().Be(0);
    }

    /// <summary>
    /// Clicking the toggle opens the menu and never raises the primary action's OnClick.
    /// </summary>
    [Test]
    public async Task ToggleClickOpensMenuWithoutInvokingOnClick()
    {
        var clicks = 0;
        bool? reportedOpen = null;
        var provider = Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudSplitButton>(parameters => parameters
            .Add(p => p.Label, "Reply")
            .Add(p => p.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => clicks++))
            .Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, v => reportedOpen = v))
            .Add(p => p.ChildContent, MenuItems("Reply All", "Forward")));

        await comp.Find(".mud-menu-icon-button-activator").ClickAsync(new MouseEventArgs());

        clicks.Should().Be(0);
        reportedOpen.Should().BeTrue();
        provider.WaitForAssertion(() => provider.FindAll(".mud-menu-item").Count.Should().Be(2));
    }

    /// <summary>
    /// Open is two-way bound: opening via the parameter renders the menu, and choosing an item reports it closed.
    /// </summary>
    [Test]
    public async Task OpenBindsTwoWay()
    {
        var open = true;
        var provider = Context.Render<MudPopoverProvider>();
        Context.Render<MudSplitButton>(parameters => parameters
            .Add(p => p.Label, "Reply")
            .Add(p => p.Open, true)
            .Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, v => open = v))
            .Add(p => p.ChildContent, MenuItems("Reply All")));

        provider.WaitForAssertion(() => provider.FindAll(".mud-menu-item").Count.Should().Be(1));

        await provider.Find(".mud-menu-item").ClickAsync(new MouseEventArgs());

        await provider.WaitForAssertionAsync(() => open.Should().BeFalse());
    }

    /// <summary>
    /// Disabled disables both segments.
    /// </summary>
    [Test]
    public void DisabledDisablesBothSegments()
    {
        Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudSplitButton>(parameters => parameters
            .Add(p => p.Label, "Reply")
            .Add(p => p.Disabled, true)
            .Add(p => p.ChildContent, MenuItems("Reply All")));

        comp.Find(".mud-split-button-primary").HasAttribute("disabled").Should().BeTrue();
        comp.Find(".mud-menu-icon-button-activator").HasAttribute("disabled").Should().BeTrue();
    }

    /// <summary>
    /// ToggleDisabled disables only the menu toggle, leaving the primary action usable.
    /// </summary>
    [Test]
    public void ToggleDisabledDisablesOnlyTheToggle()
    {
        Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudSplitButton>(parameters => parameters
            .Add(p => p.Label, "Reply")
            .Add(p => p.ToggleDisabled, true)
            .Add(p => p.ChildContent, MenuItems("Reply All")));

        comp.Find(".mud-split-button-primary").HasAttribute("disabled").Should().BeFalse();
        comp.Find(".mud-menu-icon-button-activator").HasAttribute("disabled").Should().BeTrue();
    }

    /// <summary>
    /// ToggleIcon replaces the default drop-down arrow on the toggle segment.
    /// </summary>
    [Test]
    public void ToggleIconOverridesTheDefaultArrow()
    {
        Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudSplitButton>(parameters => parameters
            .Add(p => p.Label, "Reply")
            .Add(p => p.ToggleIcon, Icons.Material.Filled.MoreVert)
            .Add(p => p.ChildContent, MenuItems("Reply All")));

        comp.FindComponent<MudIconButton>().Instance.Icon
            .Should().Be(Icons.Material.Filled.MoreVert);
    }

    /// <summary>
    /// Builds a menu-item fragment for the split button's ChildContent.
    /// </summary>
    private static RenderFragment MenuItems(params string[] labels) => builder =>
    {
        var sequence = 0;
        foreach (var label in labels)
        {
            builder.OpenComponent<MudMenuItem>(sequence++);
            builder.AddAttribute(sequence++, nameof(MudMenuItem.Label), label);
            builder.CloseComponent();
        }
    };
}
