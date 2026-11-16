using AwesomeAssertions;
using Bunit;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

/// <summary>
/// Covers what <see cref="MudFab"/> renders on top of the shared behavior in <see cref="BaseButtonTests{TButton}"/>.
/// </summary>
[TestFixture]
public class FabTests : BunitTest
{
    /// <summary>
    /// A FAB with no appearance parameters renders as a large, default-colored filled button with a ripple.
    /// </summary>
    [Test]
    public void DefaultAppearance()
    {
        var comp = Context.Render<MudFab>();

        var classes = comp.Find("button").ClassList;
        classes.Should().Contain(["mud-button-root", "mud-fab", "mud-fab-filled", "mud-fab-filled-default", "mud-fab-default", "mud-fab-size-large", "mud-ripple"]);
        classes.Should().NotContain(["mud-fab-extended", "mud-fab-disable-elevation"]);
    }

    /// <summary>
    /// Each variant renders its own variant and color classes and none of the others.
    /// Only the filled variant also emits the legacy mud-fab-{color} class, which older stylesheets target.
    /// </summary>
    [TestCase(Variant.Filled, "mud-fab-filled", "mud-fab-filled-primary", true)]
    [TestCase(Variant.Outlined, "mud-fab-outlined", "mud-fab-outlined-primary", false)]
    [TestCase(Variant.Text, "mud-fab-text", "mud-fab-text-primary", false)]
    public void VariantSetsClasses(Variant variant, string variantClass, string colorClass, bool hasLegacyColorClass)
    {
        var comp = Context.Render<MudFab>(parameters => parameters
            .Add(p => p.Variant, variant)
            .Add(p => p.Color, Color.Primary));

        var classes = comp.Find("button").ClassList;
        classes.Should().Contain([variantClass, colorClass]);
        classes.Where(c => c is "mud-fab-filled" or "mud-fab-outlined" or "mud-fab-text").Should().Equal(variantClass);
        classes.Contains("mud-fab-primary").Should().Be(hasLegacyColorClass);
    }

    /// <summary>
    /// A disabled FAB keeps its variant class, so variant-specific disabled styles such as the transparent text background still apply.
    /// </summary>
    [TestCase(Variant.Filled, "mud-fab-filled")]
    [TestCase(Variant.Outlined, "mud-fab-outlined")]
    [TestCase(Variant.Text, "mud-fab-text")]
    public void DisabledKeepsVariantClass(Variant variant, string variantClass)
    {
        var comp = Context.Render<MudFab>(parameters => parameters
            .Add(p => p.Variant, variant)
            .Add(p => p.Disabled, true));

        var button = comp.Find("button");
        button.HasAttribute("disabled").Should().BeTrue();
        button.ClassList.Should().Contain(variantClass);
    }

    /// <summary>
    /// Unlike other buttons, a disabled FAB drops its ripple.
    /// </summary>
    [Test]
    public void DisabledRemovesRipple()
    {
        var comp = Context.Render<MudFab>(parameters => parameters
            .Add(p => p.Disabled, true));

        comp.Find("button").ClassList.Should().NotContain("mud-ripple");
    }

    /// <summary>
    /// A Label extends the FAB and renders between the start and end icons, which take IconSize rather than the FAB's Size.
    /// </summary>
    [Test]
    public void LabelExtendsFabBetweenIcons()
    {
        var comp = Context.Render<MudFab>(parameters => parameters
            .Add(p => p.Label, "Compose")
            .Add(p => p.StartIcon, Icons.Material.Filled.Edit)
            .Add(p => p.EndIcon, Icons.Material.Filled.Send)
            .Add(p => p.IconSize, Size.Small));

        comp.Find("button").ClassList.Should().Contain("mud-fab-extended");
        var label = comp.Find(".mud-fab-label");
        label.TextContent.Should().Be("Compose");
        label.Children.Should().HaveCount(2).And.AllSatisfy(icon => icon.ClassList.Should().Contain("mud-icon-size-small"));
    }

    /// <summary>
    /// Without icons or a label, the FAB renders no icon.
    /// </summary>
    [Test]
    public void RendersNoIconWhenNoneIsSet()
    {
        var comp = Context.Render<MudFab>();

        comp.FindAll(".mud-icon-root").Should().BeEmpty();
    }

    /// <summary>
    /// Turning off DropShadow removes the FAB's elevation.
    /// </summary>
    [Test]
    public void DropShadowOffRemovesElevation()
    {
        var comp = Context.Render<MudFab>(parameters => parameters
            .Add(p => p.DropShadow, false));

        comp.Find("button").ClassList.Should().Contain("mud-fab-disable-elevation");
    }
}
