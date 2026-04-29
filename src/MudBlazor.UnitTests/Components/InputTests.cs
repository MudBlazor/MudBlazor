using System.Threading.Tasks;
using AwesomeAssertions;
using Bunit;

namespace MudBlazor.UnitTests.Components;
#nullable enable
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

    [Test]
    [Arguments(InputSizing.Auto, "mud-input-sizing-auto")]
    [Arguments(InputSizing.Fixed, "mud-input-sizing-fixed")]
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
}
