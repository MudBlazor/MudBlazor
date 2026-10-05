using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using MudBlazor.UnitTests.TestComponents.ButtonGroup;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class ButtonGroupTests : BunitTest
{
    /// <summary>
    /// A group with no parameters renders a horizontal element with the group role, text variant classes, and overridden button styles.
    /// </summary>
    [Test]
    public void DefaultRendering()
    {
        var comp = Context.Render<MudButtonGroup>();

        var root = comp.Find("div");
        root.GetAttribute("role").Should().Be("group");
        root.ClassList.Should().Contain(["mud-button-group-root", "mud-button-group-horizontal", "mud-button-group-override-styles", "mud-button-group-text", "mud-button-group-text-default", "mud-button-group-text-size-medium"]);
        root.ClassList.Should().NotContain(["mud-button-group-vertical", "mud-button-group-rtl", "mud-button-group-disable-elevation", "mud-width-full"]);
    }

    /// <summary>
    /// Appearance parameters set the classes the stylesheet uses to style the group and, through OverrideStyles, the buttons inside it.
    /// </summary>
    [Test]
    public void AppearanceParametersSetClasses()
    {
        var comp = Context.Render<MudButtonGroup>(parameters => parameters
            .Add(p => p.Variant, Variant.Outlined)
            .Add(p => p.Color, Color.Primary)
            .Add(p => p.Size, Size.Small)
            .Add(p => p.Vertical, true)
            .Add(p => p.DropShadow, false)
            .Add(p => p.OverrideStyles, false));

        var classes = comp.Find("div").ClassList;
        classes.Should().Contain(["mud-button-group-outlined", "mud-button-group-outlined-primary", "mud-button-group-outlined-size-small", "mud-button-group-vertical", "mud-button-group-disable-elevation"]);
        classes.Should().NotContain(["mud-button-group-horizontal", "mud-button-group-override-styles"]);
    }

    /// <summary>
    /// A right-to-left layout flips the group so its rounded corners and dividers stay on the correct sides.
    /// </summary>
    [Test]
    public void RightToLeftSetsRtlClass()
    {
        var comp = Context.Render<CascadingValue<bool>>(parameters => parameters
            .Add(p => p.Name, "RightToLeft")
            .Add(p => p.Value, true)
            .AddChildContent<MudButtonGroup>());

        comp.Find(".mud-button-group-root").ClassList.Should().Contain("mud-button-group-rtl");
    }

    /// <summary>
    /// A caller's class, style, and attributes reach the group element.
    /// </summary>
    [Test]
    public void ForwardsClassStyleAndAttributes()
    {
        var comp = Context.Render<MudButtonGroup>(parameters => parameters
            .Add(p => p.Class, "my-group")
            .Add(p => p.Style, "gap:4px")
            .AddUnmatched("aria-label", "Text formatting"));

        var root = comp.Find("div");
        root.ClassList.Should().Contain(["mud-button-group-root", "my-group"]);
        root.GetAttribute("style").Should().Be("gap:4px");
        root.GetAttribute("aria-label").Should().Be("Text formatting");
    }

    /// <summary>
    /// A full-width group stretches every button when none of them is full width on its own (#9710).
    /// </summary>
    [Test]
    public void FullWidthGroupStretchesAllButtons()
    {
        var comp = Context.Render<ButtonGroupWithThreeButtons>(parameters => parameters
            .Add(p => p.ButtonGroupFullWidth, true));

        comp.Find(".mud-button-group-root").ClassList.Should().Contain("mud-width-full");
        StretchedButtons(comp).Should().Equal(true, true, true);
    }

    /// <summary>
    /// Once any button is full width, a full-width group stretches only that button.
    /// </summary>
    [Test]
    public void FullWidthGroupStretchesOnlyFullWidthButton()
    {
        var comp = Context.Render<ButtonGroupWithThreeButtons>(parameters => parameters
            .Add(p => p.ButtonGroupFullWidth, true)
            .Add(p => p.Button2FullWidth, true));

        StretchedButtons(comp).Should().Equal(false, true, false);
    }

    /// <summary>
    /// A group that is not full width leaves its buttons at their natural width.
    /// </summary>
    [Test]
    public void GroupWithoutFullWidthDoesNotStretchButtons()
    {
        var comp = Context.Render<ButtonGroupWithThreeButtons>(parameters => parameters
            .Add(p => p.ButtonGroupFullWidth, false));

        comp.Find(".mud-button-group-root").ClassList.Should().NotContain("mud-width-full");
        StretchedButtons(comp).Should().Equal(false, false, false);
    }

    /// <summary>
    /// Removing the only full-width button makes the group stretch the remaining buttons again.
    /// </summary>
    [Test]
    public async Task RemovingFullWidthButtonStretchesRemainingButtons()
    {
        var comp = Context.Render<ButtonGroupWithThreeButtons>(parameters => parameters
            .Add(p => p.ButtonGroupFullWidth, true)
            .Add(p => p.Button1FullWidth, true));

        await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Button1Displayed, false));

        StretchedButtons(comp).Should().Equal(true, true);
    }

    /// <summary>
    /// Adding a full-width button makes the group stop stretching the other buttons.
    /// </summary>
    [Test]
    public async Task AddingFullWidthButtonUnstretchesOtherButtons()
    {
        var comp = Context.Render<ButtonGroupWithThreeButtons>(parameters => parameters
            .Add(p => p.ButtonGroupFullWidth, true)
            .Add(p => p.Button3Displayed, false)
            .Add(p => p.Button3FullWidth, true));

        await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.Button3Displayed, true));

        StretchedButtons(comp).Should().Equal(false, false, true);
    }

    /// <summary>
    /// Returns whether each rendered button in the group is stretched, in document order.
    /// </summary>
    private static IEnumerable<bool> StretchedButtons(IRenderedComponent<ButtonGroupWithThreeButtons> comp)
    {
        return comp.FindAll(".mud-button-root").Select(button => button.ClassList.Contains("mud-width-full")).ToList();
    }
}
