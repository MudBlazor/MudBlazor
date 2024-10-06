using Bunit;
using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class TextTests : BunitTest
{
    [Test]
    public void DefaultPropertyValues()
    {
        var comp = Context.RenderComponent<MudText>();

        comp.Instance.Typo.Should().Be(Typo.body1);
        comp.Instance.Align.Should().Be(Align.Inherit);
        comp.Instance.Color.Should().Be(Color.Inherit);
        comp.Instance.GutterBottom.Should().BeFalse();
        comp.Instance.Inline.Should().BeFalse();
        comp.Instance.HtmlTag.Should().BeNull();
    }

    [Test]
    public void CustomAttributes_ShouldRender()
    {
        // Arrange
        var comp = Context.RenderComponent<MudText>(builder => builder
            .AddUnmatched("data-test", "test-value")
        );

        // Assert
        comp.Markup.Should().Contain("data-test=\"test-value\"");
    }

    [TestCase(Align.Inherit, false, "")]
    [TestCase(Align.Left, false, "mud-typography-align-left")]
    [TestCase(Align.Center, false, "mud-typography-align-center")]
    [TestCase(Align.Right, false, "mud-typography-align-right")]
    [TestCase(Align.Justify, false, "mud-typography-align-justify")]
    [TestCase(Align.Start, false, "mud-typography-align-left")]
    [TestCase(Align.End, false, "mud-typography-align-right")]
    [TestCase(Align.Start, true, "mud-typography-align-right")]
    [TestCase(Align.End, true, "mud-typography-align-left")]
    public void Align_And_RightToLeft_AppliesCorrectClass(Align align, bool rightToLeft, string expectedClass)
    {
        // Arrange
        var comp = Context.RenderComponent<MudText>(builder => builder
            .Add(p => p.Align, align)
            .Add(p => p.RightToLeft, rightToLeft)
        );

        // Assert
        if (string.IsNullOrEmpty(expectedClass))
        {
            comp.Markup.Should().NotContain("mud-typography-align-");
        }
        else
        {
            comp.Markup.Should().Contain(expectedClass);
        }
    }

    [TestCase(Color.Primary, "mud-primary-text")]
    [TestCase(Color.Secondary, "mud-secondary-text")]
    [TestCase(Color.Tertiary, "mud-tertiary-text")]
    public void ColorProperty_AppliesCorrectClass(Color color, string expectedClass)
    {
        // Arrange
        var comp = Context.RenderComponent<MudText>(builder => builder
            .Add(p => p.Color, color)
        );

        // Assert
        comp.Markup.Should().Contain(expectedClass);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void GutterBottom_AppliesCorrectClass(bool gutterBottom)
    {
        // Arrange
        var comp = Context.RenderComponent<MudText>(builder => builder
            .Add(p => p.GutterBottom, gutterBottom)
        );

        // Assert
        if (gutterBottom)
        {
            comp.Markup.Should().Contain("mud-typography-gutterbottom");
        }
        else
        {
            comp.Markup.Should().NotContain("mud-typography-gutterbottom");
        }
    }

    [Test]
    public void ChildContent_IsRenderedInsideComponent()
    {
        // Arrange
        var comp = Context.RenderComponent<MudText>(builder => builder
            .AddChildContent("Hello, World!")
        );

        // Assert
        comp.Markup.Should().Contain("Hello, World!");
    }

    [TestCase(Typo.h1, "h1")]
    [TestCase(Typo.h2, "h2")]
    [TestCase(Typo.h3, "h3")]
    [TestCase(Typo.h4, "h4")]
    [TestCase(Typo.h5, "h5")]
    [TestCase(Typo.h6, "h6")]
    [TestCase(Typo.subtitle1, "p")]
    [TestCase(Typo.subtitle2, "p")]
    [TestCase(Typo.body1, "p")]
    [TestCase(Typo.body2, "p")]
    [TestCase(Typo.input, "span")]
    [TestCase(Typo.button, "span")]
    [TestCase(Typo.caption, "span")]
    [TestCase(Typo.overline, "span")]
    public void Typo_ChangesTag(Typo typo, string expectedTag)
    {
        // Arrange
        var comp = Context.RenderComponent<MudText>(builder => builder
            .Add(p => p.Typo, typo)
        );

        // Assert
        comp.Find(expectedTag).Should().NotBeNull();
    }

    [TestCase(Typo.h1, null, "h1")] // Null defaults to h1
    [TestCase(Typo.h1, "span", "span")] // span instead of h1
    [TestCase(Typo.body1, null, "p")] // Null defaults to p
    [TestCase(Typo.body1, "span", "span")] // span instead of p
    [TestCase(Typo.caption, null, "span")] // Null defaults to span
    [TestCase(Typo.caption, "", "span")] // Empty string defaults to span
    [TestCase(Typo.caption, "p", "p")] // p instead of span
    public void ActualTag_With_TypoProperty_Or_HtmlTagProperty(Typo typo, string htmlTag, string expectedTag)
    {
        // Arrange
        var comp = Context.RenderComponent<MudText>(builder => builder
            .Add(p => p.Typo, typo)
            .Add(p => p.HtmlTag, htmlTag)
        );

        // Assert
        comp.FindAll(expectedTag).Count.Should().Be(1);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void InlineProperty_AppliesCorrectClass(bool inline)
    {
        // Arrange
        var comp = Context.RenderComponent<MudText>(builder => builder
            .Add(p => p.Inline, inline)
        );

        // Assert
        if (inline)
        {
            // The inline display class should be added.
            comp.Markup.Should().Contain("d-inline");
        }
        else
        {
            // No display class should be set by default.
            comp.Markup.Should().NotMatch("d-*");
        }
    }
}
