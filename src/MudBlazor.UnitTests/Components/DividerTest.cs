using AwesomeAssertions;
using Bunit;
using NUnit.Framework;

namespace MudBlazor.UnitTests;

public class DividerTest : BunitTest
{
    [Test]
    public void Divider_WithoutContent_RendersSingleHr()
    {
        var comp = Context.Render<MudDivider>();

        comp.FindAll("hr").Should().HaveCount(1);
        comp.FindAll(".mud-divider-with-content").Should().BeEmpty();
    }

    [Test]
    public void Divider_WithContent_RendersContentBetweenTwoLines()
    {
        var comp = Context.Render<MudDivider>(parameters => parameters
            .AddChildContent("Hello"));

        comp.FindAll("hr").Should().HaveCount(2);
        comp.Find(".mud-divider-with-content-text").TextContent.Should().Be("Hello");
    }
}
