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
