using Bunit;
using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class TextTests : BunitTest
{
    [Test]
    public void CustomAttributes_AreRenderedCorrectly()
    {
        // Arrange
        var component = Context.RenderComponent<MudText>(builder => builder
            .AddUnmatched("data-test", "test-value")
        );

        // Assert
        component.Markup.Should().Contain("data-test=\"test-value\"");
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
    public void AlignAndRightToLeftClass(Align align, bool rightToLeft, string expectedClass)
    {
        // Arrange
        var component = Context.RenderComponent<MudText>(builder => builder
            .Add(p => p.Align, align)
            .Add(p => p.RightToLeft, rightToLeft)
        );

        // Assert
        if (string.IsNullOrEmpty(expectedClass))
        {
            component.Markup.Should().NotContain("mud-typography-align-");
        }
        else
        {
            component.Markup.Should().Contain(expectedClass);
        }
    }

    [TestCase(Color.Primary, "mud-primary-text")]
    [TestCase(Color.Secondary, "mud-secondary-text")]
    [TestCase(Color.Tertiary, "mud-tertiary-text")]
    public void ColorProperty_AddsColorClass(Color color, string expectedClass)
    {
        // Arrange
        var component = Context.RenderComponent<MudText>(builder => builder
            .Add(p => p.Color, color)
        );

        // Assert
        component.Markup.Should().Contain(expectedClass);
    }

    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public void GutterBottom_AddsGutterBottomClass(bool gutterBottom)
    {
        // Arrange
        var component = Context.RenderComponent<MudText>(builder => builder
            .Add(p => p.GutterBottom, gutterBottom)
        );

        // Assert
        if (gutterBottom)
        {
            component.Markup.Should().Contain("mud-typography-gutterbottom");
        }
        else
        {
            component.Markup.Should().NotContain("mud-typography-gutterbottom");
        }
    }

    [Test]
    public void ChildContent_IsRenderedInsideComponent()
    {
        // Arrange
        var component = Context.RenderComponent<MudText>(builder => builder
            .AddChildContent("Hello, World!")
        );

        // Assert
        component.Markup.Should().Contain("Hello, World!");
    }
    [Test]
    public void DefaultRender_ShouldRenderParagraph()
    {
        // Arrange
        var component = Context.RenderComponent<MudText>();

        // Assert
        component.Find("p").Should().NotBeNull();
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
    public void TypoProperty_ChangesTagName(Typo typo, string expectedTagName)
    {
        // Arrange
        var component = Context.RenderComponent<MudText>(builder => builder
            .Add(p => p.Typo, typo)
        );

        // Assert
        component.Find(expectedTagName).Should().NotBeNull();
    }

    [TestCase("p")]
    [TestCase("span")]
    [TestCase("div")]
    public void HtmlTagProperty_OverridesTypoProperty(string tagName)
    {
        // Arrange
        var component = Context.RenderComponent<MudText>(builder => builder
            .Add(p => p.Typo, Typo.h1)
            .Add(p => p.HtmlTag, tagName)
        );

        // Assert
        component.FindAll(tagName).Count.Should().Be(1);
    }
}
