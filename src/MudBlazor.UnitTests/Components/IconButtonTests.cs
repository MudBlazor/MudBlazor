using AwesomeAssertions;
using Bunit;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

/// <summary>
/// Covers what <see cref="MudIconButton"/> renders on top of the shared behavior in <see cref="BaseButtonTests{TButton}"/>.
/// </summary>
[TestFixture]
public class IconButtonTests : BunitTest
{
    /// <summary>
    /// Icon renders as the only content, sized by the button's Size, and ChildContent is ignored.
    /// </summary>
    [Test]
    public void IconRendersInsteadOfChildContent()
    {
        var comp = Context.Render<MudIconButton>(parameters => parameters
            .Add(p => p.Icon, Icons.Material.Filled.Delete)
            .Add(p => p.Size, Size.Small)
            .AddChildContent("Delete"));

        comp.Find(".mud-icon-button-label svg").ClassList.Should().Contain("mud-icon-size-small");
        comp.Find("button").TextContent.Should().BeEmpty();
    }

    /// <summary>
    /// Without an Icon, ChildContent is rendered as text so the button can show a short label.
    /// </summary>
    [Test]
    public void ChildContentRendersWithoutIcon()
    {
        var comp = Context.Render<MudIconButton>(parameters => parameters
            .AddChildContent("AB"));

        comp.FindAll(".mud-icon-button-label").Should().BeEmpty();
        comp.Find("button .mud-typography").TextContent.Should().Be("AB");
    }

    /// <summary>
    /// The default text variant renders a round icon button colored through its text color, without the regular button classes.
    /// </summary>
    [Test]
    public void TextVariantRendersAsIconButton()
    {
        var comp = Context.Render<MudIconButton>(parameters => parameters
            .Add(p => p.Icon, Icons.Material.Filled.Delete)
            .Add(p => p.Color, Color.Primary));

        var classes = comp.Find("button").ClassList;
        classes.Should().Contain(["mud-icon-button", "mud-primary-text", "hover:mud-primary-hover", "mud-ripple", "mud-ripple-icon"]);
        classes.Should().NotContain(["mud-button", "mud-button-text"]);
    }

    /// <summary>
    /// A filled or outlined icon button takes the regular button classes for its variant and color instead of a text color.
    /// </summary>
    [TestCase(Variant.Filled, "mud-button-filled", "mud-button-filled-primary")]
    [TestCase(Variant.Outlined, "mud-button-outlined", "mud-button-outlined-primary")]
    public void NonTextVariantRendersAsButton(Variant variant, string variantClass, string colorClass)
    {
        var comp = Context.Render<MudIconButton>(parameters => parameters
            .Add(p => p.Icon, Icons.Material.Filled.Delete)
            .Add(p => p.Variant, variant)
            .Add(p => p.Color, Color.Primary));

        var classes = comp.Find("button").ClassList;
        classes.Should().Contain(["mud-icon-button", "mud-button", variantClass, colorClass]);
        classes.Should().NotContain(["mud-primary-text", "mud-ripple-icon"]);
    }

    /// <summary>
    /// A non-medium Size and a non-default Edge each add a class, while the defaults add none.
    /// </summary>
    [TestCase(Size.Medium, Edge.False, null, null)]
    [TestCase(Size.Small, Edge.Start, "mud-icon-button-size-small", "mud-icon-button-edge-start")]
    [TestCase(Size.Large, Edge.End, "mud-icon-button-size-large", "mud-icon-button-edge-end")]
    public void SizeAndEdgeSetClasses(Size size, Edge edge, string sizeClass, string edgeClass)
    {
        var comp = Context.Render<MudIconButton>(parameters => parameters
            .Add(p => p.Icon, Icons.Material.Filled.Delete)
            .Add(p => p.Size, size)
            .Add(p => p.Edge, edge));

        var classes = comp.Find("button").ClassList;
        classes.SingleOrDefault(c => c.StartsWith("mud-icon-button-size-")).Should().Be(sizeClass);
        classes.SingleOrDefault(c => c.StartsWith("mud-icon-button-edge-")).Should().Be(edgeClass);
    }
}
