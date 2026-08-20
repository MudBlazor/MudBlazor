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

    [TestCase(true)]
    [TestCase(false)]
    public void FullWidthShouldSetClass(bool fullWidth)
    {
        var comp = Context.Render<MudInput<string>>(parameters => parameters
            .Add(p => p.FullWidth, fullWidth));

        if (fullWidth)
        {
            comp.Find("div.mud-input").ClassList.Should().Contain("mud-input-full-width");
        }
        else
        {
            comp.Find("div.mud-input").ClassList.Should().NotContain("mud-input-full-width");
        }
    }

    [Test]
    public async Task MudInputIsClearingShouldRespectMouseState()
    {
        var comp = Context.Render<MudInput<string>>(parameters => parameters
            .Add(x => x.Clearable, true)
            .Add(x => x.Value, "Some value")
        );

        var button = comp.Find("div.mud-input .mud-input-clear-button");
        button.Should().NotBeNull();
        await button.MouseDownAsync();
        comp.Instance.IsClearing.Should().BeTrue();
        await button.ClickAsync();
        comp.Instance.IsClearing.Should().BeFalse();

        comp = Context.Render<MudInput<string>>(parameters => parameters
            .Add(x => x.Clearable, true)
            .Add(x => x.Value, "Some value")
        );

        button = comp.Find("div.mud-input .mud-input-clear-button");

        await button.MouseDownAsync();
        comp.Instance.IsClearing.Should().BeTrue();
        await button.MouseLeaveAsync();
        comp.Instance.IsClearing.Should().BeFalse();
    }

    /// <summary>
    /// A caller-supplied aria-required should reach both halves of the range, while required still follows the parameter.
    /// </summary>
    [Test]
    public void RangeInputLetsUserAttributesOverrideAriaRequired()
    {
        var comp = Context.Render<MudRangeInput<string>>(parameters => parameters
            .Add(p => p.UserAttributes!, new Dictionary<string, object> { { "aria-required", "true" } }));

        var inputs = comp.FindAll("input");
        inputs.Should().HaveCount(2);

        foreach (var input in inputs)
        {
            input.GetAttribute("aria-required").Should().Be("true");
            input.HasAttribute("required").Should().BeFalse();
        }
    }

    /// <summary>
    /// The per-input aria-label and id stay component-owned so the two halves keep distinct names and identifiers.
    /// </summary>
    [Test]
    public void RangeInputKeepsPerInputAriaLabelAndId()
    {
        var comp = Context.Render<MudRangeInput<string>>(parameters => parameters
            .Add(p => p.UserAttributes!, new Dictionary<string, object> { { "aria-required", "true" } }));

        var inputs = comp.FindAll("input");

        inputs[0].GetAttribute("aria-label").Should().Be("Start");
        inputs[1].GetAttribute("aria-label").Should().Be("End");
        inputs[0].GetAttribute("id").Should().EndWith("-start");
        inputs[1].GetAttribute("id").Should().EndWith("-end");
    }
}
