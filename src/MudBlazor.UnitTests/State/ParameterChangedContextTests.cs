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
    [TestCase("#fcefe5", "#fcefe5", "#fcefe5", null, "Text", "#fcefe5")]
    [TestCase("#fcefe5", "#5fa9e2", "#fcefe5", "#fcefe5", "Text", "#5fa9e2")]
    [TestCase("#fcefe5", null, "#fcefe5", "#fcefe5", "Value", "#fcefe5")]
    [TestCase("#fcefe5", "#fcefe5", "#fcefe5", "#5fa9e2", "Value", "#5fa9e2")]
    [TestCase("#fcefe5", "#fcefe5", "#fcefe5", "#fcefe5", "", null)]
    [TestCase("#fcefe5", "#5fa9e2", null, null, "Text", "#5fa9e2")]
    [TestCase("#fcefe5", "#5fa9e2", "#fcefe5", null, "Text", "#5fa9e2")]
    [TestCase(null, "#5fa9e2", "#fcefe5", "#5fa9e2", "Text", "#5fa9e2")]
    [TestCase(null, "#5fa9e2", null, "#5fa9e2", "Text", "#5fa9e2")]
    [TestCase(null, null, "#fcefe5", "#5fa9e2", "Value", "#5fa9e2")]
    [TestCase(null, null, null, "#5fa9e2", "Value", "#5fa9e2")]
    [TestCase(null, null, null, null, "", null)]
    public void ResolveEffectiveParameter_ShouldSelectCorrectParameter_TextDominant(string textBefore, string textAfter, string valueBefore, string valueAfter, string expectedParameter, string? expectedColor)
    {
        var result = Resolve(textBefore, textAfter, valueBefore, valueAfter, "Text");

        switch (expectedParameter)
        {
            case "Text":
                result.EffectiveParameterName.Should().Be("Text");
                result.HasEffectiveParameter.Should().BeTrue();
                result.IsParameter1.Should().BeTrue();
                result.IsParameter2.Should().BeFalse();
                result.Parameter1Value.Should().Be(expectedColor);
                result.Parameter2Value.Should().BeNull();
                break;
            case "Value":
                result.EffectiveParameterName.Should().Be("Value");
                result.HasEffectiveParameter.Should().BeTrue();
                result.IsParameter2.Should().BeTrue();
                result.IsParameter1.Should().BeFalse();
                result.Parameter1Value.Should().BeNull();
                result.Parameter2Value.Should().Be(expectedColor);
                break;
            default:
                result.EffectiveParameterName.Should().Be(string.Empty);
                result.HasEffectiveParameter.Should().BeFalse();
                result.IsParameter1.Should().BeFalse();
                result.IsParameter2.Should().BeFalse();
                result.Parameter1Value.Should().BeNull();
                result.Parameter2Value.Should().BeNull();
                break;
        }
    }

    [Test]
    [TestCase("#fcefe5", "#5fa9e2", "#fcefe5", null, "Text", "#5fa9e2")]
    [TestCase("#fcefe5", "#5fa9e2", "#fcefe5", "#5fa9e2", "Value", "#5fa9e2")]
    [TestCase("#fcefe5", null, "#fcefe5", "#5fa9e2", "Value", "#5fa9e2")]
    [TestCase("#fcefe5", null, "#fcefe5", null, "Value", null)]
    [TestCase("#fcefe5", "#fcefe5", "#fcefe5", null, "Text", "#fcefe5")]
    [TestCase("#fcefe5", "#5fa9e2", "#fcefe5", "#fcefe5", "Text", "#5fa9e2")]
    [TestCase("#fcefe5", null, "#fcefe5", "#fcefe5", "Value", "#fcefe5")]
    [TestCase("#fcefe5", "#fcefe5", "#fcefe5", "#5fa9e2", "Value", "#5fa9e2")]
    [TestCase("#fcefe5", "#fcefe5", "#fcefe5", "#fcefe5", "", null)]
    [TestCase("#fcefe5", "#5fa9e2", null, null, "Text", "#5fa9e2")]
    [TestCase("#fcefe5", "#5fa9e2", "#fcefe5", null, "Text", "#5fa9e2")]
    [TestCase(null, "#5fa9e2", "#fcefe5", "#5fa9e2", "Value", "#5fa9e2")]
    [TestCase(null, "#5fa9e2", null, "#5fa9e2", "Value", "#5fa9e2")]
    [TestCase(null, null, "#fcefe5", "#5fa9e2", "Value", "#5fa9e2")]
    [TestCase(null, null, null, "#5fa9e2", "Value", "#5fa9e2")]
    [TestCase(null, null, null, null, "", null)]
    public void ResolveEffectiveParameter_ShouldSelectCorrectParameter_ValueDominant(string textBefore, string textAfter, string valueBefore, string valueAfter, string expectedParameter, string? expectedColor)
    {
        var result = Resolve(textBefore, textAfter, valueBefore, valueAfter, "Value");

        switch (expectedParameter)
        {
            case "Text":
                result.EffectiveParameterName.Should().Be("Text");
                result.HasEffectiveParameter.Should().BeTrue();
                result.IsParameter1.Should().BeTrue();
                result.IsParameter2.Should().BeFalse();
                result.Parameter1Value.Should().Be(expectedColor);
                result.Parameter2Value.Should().BeNull();
                break;
            case "Value":
                result.EffectiveParameterName.Should().Be("Value");
                result.HasEffectiveParameter.Should().BeTrue();
                result.IsParameter2.Should().BeTrue();
                result.IsParameter1.Should().BeFalse();
                result.Parameter1Value.Should().BeNull();
                result.Parameter2Value.Should().Be(expectedColor);
                break;
            default:
                result.EffectiveParameterName.Should().Be(string.Empty);
                result.HasEffectiveParameter.Should().BeFalse();
                result.IsParameter1.Should().BeFalse();
                result.IsParameter2.Should().BeFalse();
                result.Parameter1Value.Should().BeNull();
                result.Parameter2Value.Should().BeNull();
                break;
        }
    }

    [Test]
    public void ResolveEffectiveParameter_ShouldThrow_WhenDominantParameterUnknown()
    {
        // Arrange: both Text and Value change
        const string TextBefore = "#fcefe5";
        const string TextAfter = "#5fa9e2";
        const string ValueBefore = "#fcefe5";
        const string ValueAfter = "#5fa9e2";

        // Act & Assert
        FluentActions
            .Invoking(() => Resolve(TextBefore, TextAfter, ValueBefore, ValueAfter, "NonExistentDominant"))
            .Should().Throw<ArgumentException>()
            .WithMessage("Unknown dominant parameter 'NonExistentDominant'.");
    }

    private static EffectiveParameterResult<string?, string?> Resolve(string textBefore, string? textAfter, string valueBefore, string? valueAfter, string dominant)
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

        return context.ResolveEffectiveParameter(psText, psValue, dominant);
    }
}
