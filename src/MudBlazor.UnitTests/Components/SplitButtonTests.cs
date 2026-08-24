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
        var provider = Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudSplitButton>(parameters => parameters
            .Add(p => p.Label, "Reply")
            .Add(p => p.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, () => clicks++))
            .Add(p => p.ChildContent, MenuItems("Reply All")));

        await comp.Find(".mud-split-button-primary").ClickAsync(new MouseEventArgs());

        clicks.Should().Be(1);
        // The menu renders into the provider's tree, so the split button's own tree can never show it either way.
        provider.FindAll(".mud-menu-item").Should().BeEmpty();
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
    /// The toggle announces that it opens a menu, and reports it collapsed until it is opened.
    /// </summary>
    [Test]
    public void ToggleExposesMenuPopupSemantics()
    {
        Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudSplitButton>(parameters => parameters
            .Add(p => p.Label, "Reply")
            .Add(p => p.ChildContent, MenuItems("Reply All")));

        var toggle = comp.Find(".mud-menu-icon-button-activator");

        // ARIA treats aria-haspopup="true" as equivalent to "menu"; this matches MudMenu's other activators.
        toggle.GetAttribute("aria-haspopup").Should().Be("true");
        toggle.GetAttribute("aria-expanded").Should().Be("false");
    }

    /// <summary>
    /// aria-expanded follows the menu's open state.
    /// </summary>
    [Test]
    public async Task ToggleAriaExpandedReflectsOpenState()
    {
        Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudSplitButton>(parameters => parameters
            .Add(p => p.Label, "Reply")
            .Add(p => p.ChildContent, MenuItems("Reply All")));

        await comp.Find(".mud-menu-icon-button-activator").ClickAsync(new MouseEventArgs());

        comp.WaitForAssertion(() =>
            comp.Find(".mud-menu-icon-button-activator").GetAttribute("aria-expanded").Should().Be("true"));
    }

    /// <summary>
    /// The icon-only toggle carries a default accessible name.
    /// </summary>
    [Test]
    public void ToggleHasADefaultAccessibleName()
    {
        Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudSplitButton>(parameters => parameters
            .Add(p => p.Label, "Reply")
            .Add(p => p.ChildContent, MenuItems("Reply All")));

        comp.Find(".mud-menu-icon-button-activator").GetAttribute("aria-label").Should().Be("More actions");
    }

    /// <summary>
    /// A caller-supplied ToggleAriaLabel replaces the default accessible name.
    /// </summary>
    [Test]
    public void ToggleAriaLabelOverridesTheDefault()
    {
        Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudSplitButton>(parameters => parameters
            .Add(p => p.Label, "Reply")
            .Add(p => p.ToggleAriaLabel, "More reply options")
            .Add(p => p.ChildContent, MenuItems("Reply All")));

        comp.Find(".mud-menu-icon-button-activator").GetAttribute("aria-label").Should().Be("More reply options");
    }

    /// <summary>
    /// Escape closes an open menu.
    /// </summary>
    [Test]
    public async Task EscapeClosesTheMenu()
    {
        var open = true;
        var provider = Context.Render<MudPopoverProvider>();
        Context.Render<MudSplitButton>(parameters => parameters
            .Add(p => p.Label, "Reply")
            .Add(p => p.Open, true)
            .Add(p => p.OpenChanged, EventCallback.Factory.Create<bool>(this, v => open = v))
            .Add(p => p.ChildContent, MenuItems("Reply All")));

        provider.WaitForAssertion(() => provider.FindAll("[data-testid='menu-wrapper']").Count.Should().Be(1));

        await provider.Find("[data-testid='menu-wrapper']").KeyDownAsync(new KeyboardEventArgs { Key = "Escape" });

        await provider.WaitForAssertionAsync(() => open.Should().BeFalse());
    }

    /// <summary>
    /// Size reaches the primary segment, so its icons scale with the rest of the button.
    /// </summary>
    [Test]
    public void SizeAppliesToThePrimarySegment()
    {
        Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudSplitButton>(parameters => parameters
            .Add(p => p.Label, "Save")
            .Add(p => p.StartIcon, Icons.Material.Filled.Save)
            .Add(p => p.Size, Size.Large)
            .Add(p => p.ChildContent, MenuItems("Save as draft")));

        comp.FindComponent<MudButton>().Instance.Size.Should().Be(Size.Large);
        comp.Find(".mud-split-button-primary .mud-icon-root").ClassList
            .Should().Contain("mud-icon-size-large");
    }

    /// <summary>
    /// A disabled toggle keeps its menu shut even when Open is set programmatically.
    /// </summary>
    [Test]
    public void ToggleDisabledKeepsTheMenuShutWhenOpenIsSet()
    {
        var provider = Context.Render<MudPopoverProvider>();
        Context.Render<MudSplitButton>(parameters => parameters
            .Add(p => p.Label, "Publish")
            .Add(p => p.ToggleDisabled, true)
            .Add(p => p.Open, true)
            .Add(p => p.ChildContent, MenuItems("Unreachable")));

        provider.FindAll(".mud-menu-item").Should().BeEmpty();
    }

    /// <summary>
    /// A blank ToggleIcon falls back to the default arrow rather than dropping the toggle segment.
    /// </summary>
    [Test]
    public void BlankToggleIconFallsBackToTheDefaultArrow()
    {
        Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudSplitButton>(parameters => parameters
            .Add(p => p.Label, "Reply")
            .Add(p => p.ToggleIcon, string.Empty)
            .Add(p => p.ChildContent, MenuItems("Reply All")));

        comp.FindAll(".mud-menu-icon-button-activator").Count.Should().Be(1);
        comp.FindComponent<MudIconButton>().Instance.Icon
            .Should().Be(Icons.Material.Filled.ArrowDropDown);
    }

    /// <summary>
    /// Flattening the button does not flatten its menu, which needs elevation to read against the page.
    /// </summary>
    [Test]
    public void DropShadowDoesNotFlattenTheMenu()
    {
        Context.Render<MudPopoverProvider>();
        var comp = Context.Render<MudSplitButton>(parameters => parameters
            .Add(p => p.Label, "Reply")
            .Add(p => p.DropShadow, false)
            .Add(p => p.ChildContent, MenuItems("Reply All")));

        comp.FindComponent<MudButtonGroup>().Instance.DropShadow.Should().BeFalse();
        comp.FindComponent<MudMenu>().Instance.DropShadow.Should().BeTrue();
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
