// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State;

#nullable enable
[TestFixture]
public class ParameterChangedContextTests
{
    [Test]
    [TestCase("#fcefe5", "#5fa9e2", "#fcefe5", null, "Text", "#5fa9e2")]
    [TestCase("#fcefe5", "#5fa9e2", "#fcefe5", "#5fa9e2", "Text", "#5fa9e2")]
    [TestCase("#fcefe5", null, "#fcefe5", "#5fa9e2", "Value", "#5fa9e2")]
    [TestCase("#fcefe5", null, "#fcefe5", null, "Text", null)]
    [TestCase("#fcefe5", "#5fa9e2", "#fcefe5", "#fcefe5", "Text", "#5fa9e2")]
    [TestCase("#fcefe5", null, "#fcefe5", "#fcefe5", "Value", "#fcefe5")]
    [TestCase("#fcefe5", "#fcefe5", "#fcefe5", "#5fa9e2", "Value", "#5fa9e2")]
    [TestCase("#fcefe5", "#fcefe5", "#fcefe5", "#fcefe5", "", null)]
    [TestCase(null, "#5fa9e2", "#fcefe5", "#5fa9e2", "Text", "#5fa9e2")]
    [TestCase(null, "#5fa9e2", null, "#5fa9e2", "Text", "#5fa9e2")]
    [TestCase(null, null, "#fcefe5", "#5fa9e2", "Value", "#5fa9e2")]
    [TestCase(null, null, null, "#5fa9e2", "Value", "#5fa9e2")]
    [TestCase(null, null, null, null, "", null)]
    public void ResolveEffectiveParameter_ShouldSelectCorrectParameter(string textBefore, string textAfter, string valueBefore, string valueAfter, string expectedParameter, string? expectedColor)
    {
        var result = Resolve(textBefore, textAfter, valueBefore, valueAfter);

        if (expectedParameter == "Text")
        {
            result.HasEffectiveParameter.Should().BeTrue();
            result.IsParameter1.Should().BeTrue();
            result.IsParameter2.Should().BeFalse();
            result.Parameter1Value.Should().Be(expectedColor);
            result.Parameter2Value.Should().BeNull();
        }
        else if (expectedParameter == "Value")
        {
            result.HasEffectiveParameter.Should().BeTrue();
            result.IsParameter2.Should().BeTrue();
            result.IsParameter1.Should().BeFalse();
            result.Parameter1Value.Should().BeNull();
            result.Parameter2Value.Should().Be(expectedColor);
        }
        else
        {
            result.HasEffectiveParameter.Should().BeFalse();
            result.IsParameter1.Should().BeFalse();
            result.IsParameter2.Should().BeFalse();
            result.Parameter1Value.Should().BeNull();
            result.Parameter2Value.Should().BeNull();
        }
    }

    private static EffectiveParameterResult<string?, string?> Resolve(string textBefore, string? textAfter, string valueBefore, string? valueAfter)
    {
        var parameterStates = new ParameterStateCollection(new Dictionary<string, ParameterStateValue>
        {
            ["Text"] = new("Text", textBefore, textAfter),
            ["Value"] = new("Value", valueBefore, valueAfter)
        });

        var parameterView = ParameterView.FromDictionary(new Dictionary<string, object?>
        {
            ["Text"] = textAfter,
            ["Value"] = valueAfter
        });

        var context = new ParameterChangedContext(parameterView, parameterStates);

        var psText = ParameterStateInternal<string?>.Attach(new ParameterMetadata("Text"), () => textAfter, () => default);
        var psValue = ParameterStateInternal<string?>.Attach(new ParameterMetadata("Value"), () => valueAfter, () => default);

        return context.ResolveEffectiveParameter(psText, psValue, "Text");
    }
}
