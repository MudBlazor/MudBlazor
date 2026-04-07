using AwesomeAssertions;
using Bunit;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

#nullable enable
[TestFixture]
public class InputTests : BunitTest
{
    [Test]
    public async Task ReadOnlyShouldNotHaveClearButton()
    {
        var comp = Context.Render<MudInput<string>>(p => p
            .Add(x => x.Text, "some value")
            .Add(x => x.Clearable, true)
            .Add(x => x.ReadOnly, false));

        comp.FindAll(".mud-input-clear-button").Count.Should().Be(1);

        await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ReadOnly, true)); //no clear button when readonly
        comp.FindAll(".mud-input-clear-button").Count.Should().Be(0);
    }

    [TestCase(InputSizing.Auto, "mud-input-sizing-auto")]
    [TestCase(InputSizing.Fixed, "mud-input-sizing-fixed")]
    public void InputSizingHasClass(InputSizing sizing, string expectedClass)
    {
        var comp = Context.Render<MudInput<string>>(parameters => parameters
            .Add(p => p.Sizing, sizing));

        comp.Find("div.mud-input").ClassList.Should().Contain(expectedClass);
    }

    [Test]
    public void RangeInputDefaultAriaLabels()
    {
        var comp = Context.Render<MudRangeInput<string>>();
        var inputs = comp.FindAll("input");

        inputs[0].Attributes.GetNamedItem("aria-label")?.Value.Should().Be("Start");
        inputs[1].Attributes.GetNamedItem("aria-label")?.Value.Should().Be("End");
    }

    [Test]
    public void RangeInputCustomAriaLabels()
    {
        const string startAriaLabel = "From";
        const string endAriaLabel = "To";
        var comp = Context.Render<MudRangeInput<string>>(parameters => parameters
            .Add(x => x.StartInputAriaLabel, startAriaLabel)
            .Add(x => x.EndInputAriaLabel, endAriaLabel));
        var inputs = comp.FindAll("input");

        inputs[0].Attributes.GetNamedItem("aria-label")?.Value.Should().Be(startAriaLabel);
        inputs[1].Attributes.GetNamedItem("aria-label")?.Value.Should().Be(endAriaLabel);
    }

    [Test]
    public void RangeInput_Error_UsesAriaErrorAttributesOnBothInputs()
    {
        const string errorId = "range-error-id";
        var comp = Context.Render<MudRangeInput<string>>(parameters => parameters
            .Add(x => x.Error, true)
            .Add(x => x.ErrorId, errorId)
            .Add(x => x.ErrorText, "Range error"));
        var inputs = comp.FindAll("input");

        inputs.Count.Should().Be(2);
        inputs[0].GetAttribute("aria-invalid").Should().Be("true");
        inputs[1].GetAttribute("aria-invalid").Should().Be("true");
        inputs[0].GetAttribute("aria-describedby").Should().Be(errorId);
        inputs[1].GetAttribute("aria-describedby").Should().Be(errorId);
        inputs[0].GetAttribute("aria-errormessage").Should().Be(errorId);
        inputs[1].GetAttribute("aria-errormessage").Should().Be(errorId);
    }
}
