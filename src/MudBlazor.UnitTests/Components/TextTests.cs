using System.Linq;
using AwesomeAssertions;
using Bunit;

namespace MudBlazor.UnitTests.Components;
public class TextTests : BunitTest
{
    [Test]
    public void Defaults_ShouldExposeExpectedParameterValues()
    {
        var comp = Context.Render<MudText>();

        comp.Instance.Typo.Should().Be(Typo.body1);
        comp.Instance.Align.Should().Be(Align.Inherit);
        comp.Instance.Color.Should().Be(Color.Inherit);
        comp.Instance.GutterBottom.Should().BeFalse();
        comp.Instance.Inline.Should().BeFalse();
        comp.Instance.HtmlTag.Should().BeNull();
    }

    [Test]
    public void Defaults_ShouldRenderBody1ParagraphWithoutOptionalClasses()
    {
        var comp = Context.Render<MudText>();

        comp.MarkupMatches("""<p class="mud-typography mud-typography-body1"></p>""");
    }

    [Test]
    public void UserAttributes_ShouldBeSplattedOnTheRootElement()
    {
        var comp = Context.Render<MudText>(parameters => parameters
            .AddUnmatched("data-test", "test-value")
            .AddUnmatched("aria-label", "example"));

        var element = comp.Find(".mud-typography");

        element.GetAttribute("data-test").Should().Be("test-value");
        element.GetAttribute("aria-label").Should().Be("example");
    }

    [Test]
    [Arguments(Align.Inherit, false, null)]
    [Arguments(Align.Left, false, "mud-typography-align-left")]
    [Arguments(Align.Center, false, "mud-typography-align-center")]
    [Arguments(Align.Right, false, "mud-typography-align-right")]
    [Arguments(Align.Justify, false, "mud-typography-align-justify")]
    [Arguments(Align.Start, false, "mud-typography-align-left")]
    [Arguments(Align.End, false, "mud-typography-align-right")]
    [Arguments(Align.Start, true, "mud-typography-align-right")]
    [Arguments(Align.End, true, "mud-typography-align-left")]
    public void Align_ShouldRenderTheExpectedAlignmentClass(Align align, bool rightToLeft, string expectedClass)
    {
        var comp = Context.Render<MudText>(parameters => parameters
            .Add(p => p.Align, align)
            .Add(p => p.RightToLeft, rightToLeft));

        var element = comp.Find(".mud-typography");
        var alignmentClasses = element
            .ClassList
            .Where(x => x.StartsWith("mud-typography-align-"))
            .ToArray();

        if (expectedClass is null)
        {
            alignmentClasses.Should().BeEmpty();
        }
        else
        {
            alignmentClasses.Should().Contain(expectedClass);
            alignmentClasses.Should().HaveCount(1);
        }
    }

    [Test]
    [Arguments(Color.Inherit, null)]
    [Arguments(Color.Default, null)]
    [Arguments(Color.Primary, "mud-primary-text")]
    [Arguments(Color.Secondary, "mud-secondary-text")]
    [Arguments(Color.Tertiary, "mud-tertiary-text")]
    [Arguments(Color.Error, "mud-error-text")]
    public void Color_ShouldRenderTheExpectedTextColorClass(Color color, string expectedClass)
    {
        var comp = Context.Render<MudText>(parameters => parameters
            .Add(p => p.Color, color));

        var element = comp.Find(".mud-typography");
        var colorClasses = element
            .ClassList
            .Where(x => x.EndsWith("-text"))
            .ToArray();

        if (expectedClass is null)
        {
            colorClasses.Should().BeEmpty();
        }
        else
        {
            colorClasses.Should().Contain(expectedClass);
            colorClasses.Should().HaveCount(1);
        }
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public void GutterBottom_ShouldRenderTheMarginClassOnlyWhenEnabled(bool gutterBottom)
    {
        var comp = Context.Render<MudText>(parameters => parameters
            .Add(p => p.GutterBottom, gutterBottom));

        var element = comp.Find(".mud-typography");

        if (gutterBottom)
        {
            element.ClassList.Should().Contain("mud-typography-gutterbottom");
        }
        else
        {
            element.ClassList.Should().NotContain("mud-typography-gutterbottom");
        }
    }

    [Test]
    public void ChildContent_ShouldRenderInsideTheTypographyElement()
    {
        var comp = Context.Render<MudText>(parameters => parameters
            .AddChildContent("Hello, World!"));

        comp.MarkupMatches("""<p class="mud-typography mud-typography-body1">Hello, World!</p>""");
    }

    [Test]
    [Arguments(Typo.inherit, "span", "mud-typography-inherit")]
    [Arguments(Typo.h1, "h1", "mud-typography-h1")]
    [Arguments(Typo.h2, "h2", "mud-typography-h2")]
    [Arguments(Typo.h3, "h3", "mud-typography-h3")]
    [Arguments(Typo.h4, "h4", "mud-typography-h4")]
    [Arguments(Typo.h5, "h5", "mud-typography-h5")]
    [Arguments(Typo.h6, "h6", "mud-typography-h6")]
    [Arguments(Typo.subtitle1, "p", "mud-typography-subtitle1")]
    [Arguments(Typo.subtitle2, "p", "mud-typography-subtitle2")]
    [Arguments(Typo.body1, "p", "mud-typography-body1")]
    [Arguments(Typo.body2, "p", "mud-typography-body2")]
    [Arguments(Typo.button, "span", "mud-typography-button")]
    [Arguments(Typo.caption, "span", "mud-typography-caption")]
    [Arguments(Typo.overline, "span", "mud-typography-overline")]
    public void Typo_ShouldRenderTheExpectedTagAndTypographyClass(Typo typo, string expectedTag, string expectedClass)
    {
        var comp = Context.Render<MudText>(parameters => parameters
            .Add(p => p.Typo, typo)
            .AddChildContent("content"));

        var element = comp.Find(".mud-typography");
        var typographyClasses = element
            .ClassList
            .Where(x => x.StartsWith("mud-typography-"))
            .ToArray();

        element.TagName.Should().Be(expectedTag.ToUpperInvariant());
        typographyClasses.Should().Contain(expectedClass);
        typographyClasses.Should().HaveCount(1);
        element.TextContent.Should().Be("content");
    }

    [Test]
    [Arguments(Typo.h1, null, "h1", "mud-typography-h1")]
    [Arguments(Typo.body1, "", "p", "mud-typography-body1")]
    [Arguments(Typo.caption, "p", "p", "mud-typography-caption")]
    [Arguments(Typo.h4, "span", "span", "mud-typography-h4")]
    public void HtmlTag_ShouldOverrideOrFallbackToTheTypoSelectedTag(Typo typo, string htmlTag, string expectedTag, string expectedClass)
    {
        var comp = Context.Render<MudText>(parameters => parameters
            .Add(p => p.Typo, typo)
            .Add(p => p.HtmlTag, htmlTag)
            .AddChildContent("content"));

        var element = comp.Find(".mud-typography");
        var typographyClasses = element
            .ClassList
            .Where(x => x.StartsWith("mud-typography-"))
            .ToArray();

        element.TagName.Should().Be(expectedTag.ToUpperInvariant());
        typographyClasses.Should().Contain(expectedClass);
        typographyClasses.Should().HaveCount(1);
        element.TextContent.Should().Be("content");
    }

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public void Inline_ShouldRenderTheDisplayClassOnlyWhenEnabled(bool inline)
    {
        var comp = Context.Render<MudText>(parameters => parameters
            .Add(p => p.Inline, inline));

        var element = comp.Find(".mud-typography");

        if (inline)
        {
            element.ClassList.Should().Contain("d-inline");
        }
        else
        {
            element.ClassList.Should().NotContain("d-inline");
        }
    }
}