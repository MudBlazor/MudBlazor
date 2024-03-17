using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.Components;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State;

[TestFixture]
public class ParameterStateUsageTests : BunitTest
{
    [Test]
    public void SharedHandlerIntegrationTest()
    {
        var comp = Context.RenderComponent<ParameterStateSharedHandlerTestComp>();

        comp.Find("span.abc").InnerHtml.Trimmed().Should().Be("1");
        comp.Find("span.xyz").InnerHtml.Trimmed().Should().Be("1");
        comp.Find("button.abc").Click();
        comp.Find("span.abc").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.xyz").InnerHtml.Trimmed().Should().Be("1");
        comp.Find("button.xyz").Click();
        comp.Find("span.abc").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.xyz").InnerHtml.Trimmed().Should().Be("2");
    }

}
