using AwesomeAssertions;
using Bunit;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class DividerTests : BunitTest
{
    /// <summary>
    /// Without content, a single <c>hr</c> is rendered and no wrapper exists.
    /// </summary>
    [Test]
    public void Divider_WithoutContent_RendersSingleHr()
    {
        var comp = Context.Render<MudDivider>();

        comp.FindAll("hr").Should().HaveCount(1);
        comp.FindAll(".mud-divider-with-content").Should().BeEmpty();
    }

    /// <summary>
    /// With content, the text is rendered between two lines.
    /// </summary>
    [Test]
    public void Divider_WithContent_RendersContentBetweenTwoLines()
    {
        var comp = Context.Render<MudDivider>(parameters => parameters
            .AddChildContent("Hello"));

        comp.FindAll("hr").Should().HaveCount(2);
        comp.Find(".mud-divider-with-content-text").TextContent.Should().Be("Hello");
    }

    /// <summary>
    /// With content, the wrapper is a single separator whose accessible name is the content.
    /// </summary>
    [Test]
    public void Divider_WithContent_ExposesSingleLabeledSeparator()
    {
        var comp = Context.Render<MudDivider>(parameters => parameters
            .AddChildContent("Hello"));

        var wrapper = comp.Find("[role=separator]");
        var labelId = wrapper.GetAttribute("aria-labelledby");

        labelId.Should().NotBeNullOrEmpty();
        comp.Find($"#{labelId}").TextContent.Should().Be("Hello");
        comp.FindAll("[role=separator]").Should().HaveCount(1);
    }

    /// <summary>
    /// With content, both decorative lines are hidden from assistive technology.
    /// </summary>
    [Test]
    public void Divider_WithContent_HidesLinesFromAssistiveTechnology()
    {
        var comp = Context.Render<MudDivider>(parameters => parameters
            .AddChildContent("Hello"));

        comp.FindAll("hr").Should().AllSatisfy(hr =>
            hr.GetAttribute("aria-hidden").Should().Be("true"));
    }

    /// <summary>
    /// A vertical divider with content exposes a vertical orientation.
    /// </summary>
    [Test]
    public void Divider_WithContentAndVertical_SetsVerticalOrientation()
    {
        var comp = Context.Render<MudDivider>(parameters => parameters
            .Add(p => p.Vertical, true)
            .AddChildContent("Hello"));

        comp.Find("[role=separator]").GetAttribute("aria-orientation").Should().Be("vertical");
    }

    /// <summary>
    /// With content, Class, Style and user attributes land on the wrapper and not on the lines.
    /// </summary>
    [Test]
    public void Divider_WithContent_AppliesClassStyleAndAttributesToWrapper()
    {
        var comp = Context.Render<MudDivider>(parameters => parameters
            .Add(p => p.Class, "my-class")
            .Add(p => p.Style, "color: red;")
            .AddUnmatched("data-test", "x")
            .AddChildContent("Hello"));

        var wrapper = comp.Find("[role=separator]");
        wrapper.ClassList.Should().Contain("my-class");
        wrapper.GetAttribute("style").Should().Contain("color: red");
        wrapper.GetAttribute("data-test").Should().Be("x");

        comp.FindAll("hr").Should().AllSatisfy(hr =>
        {
            hr.ClassList.Should().NotContain("my-class");
            hr.HasAttribute("style").Should().BeFalse();
            hr.HasAttribute("data-test").Should().BeFalse();
        });
    }

    /// <summary>
    /// Without content, Class, Style and user attributes stay on the single hr.
    /// </summary>
    [Test]
    public void Divider_WithoutContent_AppliesClassStyleAndAttributesToHr()
    {
        var comp = Context.Render<MudDivider>(parameters => parameters
            .Add(p => p.Class, "my-class")
            .Add(p => p.Style, "color: red;")
            .AddUnmatched("data-test", "x"));

        var hr = comp.Find("hr");
        hr.ClassList.Should().Contain("my-class");
        hr.GetAttribute("style").Should().Contain("color: red");
        hr.GetAttribute("data-test").Should().Be("x");
    }

    /// <summary>
    /// With content, a caller's role and aria-labelledby replace the generated ones.
    /// </summary>
    [Test]
    public void Divider_WithContent_CallerAriaAttributesWin()
    {
        var comp = Context.Render<MudDivider>(parameters => parameters
            .AddUnmatched("role", "presentation")
            .AddUnmatched("aria-labelledby", "external-label")
            .AddChildContent("Hello"));

        var wrapper = comp.Find(".mud-divider-with-content");
        wrapper.GetAttribute("role").Should().Be("presentation");
        wrapper.GetAttribute("aria-labelledby").Should().Be("external-label");
    }

    /// <summary>
    /// With content, DividerType, Absolute and FlexItem go on the wrapper and Light goes on the lines.
    /// </summary>
    [Test]
    public void Divider_WithContent_RoutesVariantClasses()
    {
        var comp = Context.Render<MudDivider>(parameters => parameters
            .Add(p => p.DividerType, DividerType.Middle)
            .Add(p => p.Absolute, true)
            .Add(p => p.FlexItem, true)
            .Add(p => p.Light, true)
            .AddChildContent("Hello"));

        var wrapper = comp.Find("[role=separator]");
        wrapper.ClassList.Should().Contain("mud-divider-middle");
        wrapper.ClassList.Should().Contain("mud-divider-absolute");
        wrapper.ClassList.Should().Contain("mud-divider-flexitem");

        comp.FindAll("hr").Should().AllSatisfy(hr =>
        {
            hr.ClassList.Should().Contain("mud-divider-light");
            hr.ClassList.Should().NotContain("mud-divider-middle");
            hr.ClassList.Should().NotContain("mud-divider-absolute");
            hr.ClassList.Should().NotContain("mud-divider-flexitem");
        });
    }
}
