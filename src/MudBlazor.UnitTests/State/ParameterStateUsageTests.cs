using System;
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.Components;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State;

#nullable enable
[TestFixture]
public class ParameterStateUsageTests : BunitTest
{
    [Test]
    public void SharedHandlerIntegrationTest()
    {
        var comp = Context.RenderComponent<ParameterStateSharedHandlerTestComp>();

        // note: the handler for abc and the one for xyz are each called once per click
        // the handlers for o and p are lambdas which are excluded from this optimization, so they
        // are each called per click resulting in an increment of 2 per click for op 
        comp.Find("span.abc").InnerHtml.Trimmed().Should().Be("1");
        comp.Find("span.op").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.xyz").InnerHtml.Trimmed().Should().Be("1");
        comp.Find("button.abc").Click();
        comp.Find("span.abc").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.op").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.xyz").InnerHtml.Trimmed().Should().Be("1");
        comp.Find("button.xyz").Click();
        comp.Find("span.abc").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.op").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.xyz").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("button.op").Click();
        comp.Find("span.abc").InnerHtml.Trimmed().Should().Be("2");
        comp.Find("span.op").InnerHtml.Trimmed().Should().Be("4");
        comp.Find("span.xyz").InnerHtml.Trimmed().Should().Be("2");
    }

    [Test]
    public void EventArgsIntegrationTest()
    {
        var comp = Context.RenderComponent<ParameterStateEventArgsTestComp>();
        comp.Find(".parameter-changes").Children.Length.Should().Be(0);
        comp.Find("button.increment-int-param").Click();
        comp.Find(".parameter-changes").Children.Length.Should().Be(1);
        comp.Find(".parameter-changes").FirstChild?.TextContent.Trimmed().Should().Be("IntParam: 0=>1");
        comp.Find("button.increment-int-param").Click();
        comp.Find(".parameter-changes").Children.Length.Should().Be(2);
        comp.Find(".parameter-changes").LastChild?.TextContent.Trimmed().Should().Be("IntParam: 1=>2");
    }

    [Test]
    public void ComparerIntegrationTest()
    {
        var comp = Context.RenderComponent<ParameterStateComparerTestComp>(parameters => parameters
            .Add(parameter => parameter.DoubleParam, 10000f));
        IElement ParamChanges() => comp.Find(".parameter-changes");
        comp.Find(".parameter-changes").Children.Length.Should().Be(1);
        ParamChanges().Children[0].TextContent.Trimmed().Should().Be("DoubleParam: 0=>10000");
        comp.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.DoubleParam, 10001f));
        comp.Find(".parameter-changes").Children.Length.Should().Be(2);
        ParamChanges().Children[1].TextContent.Trimmed().Should().Be("DoubleParam: 10000=>10001");
        comp.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.DoubleParam, 1000000f));
        comp.Find(".parameter-changes").Children.Length.Should().Be(3);
        ParamChanges().Children[2].TextContent.Trimmed().Should().Be("DoubleParam: 10001=>1000000");
        comp.SetParametersAndRender(parameters => parameters.Add(parameter => parameter.DoubleParam, 1000001f));
        comp.Find(".parameter-changes").Children.Length.Should().Be(3, "Within the epsilon tolerance. Therefore, change handler shouldn't fire.");
    }
}
