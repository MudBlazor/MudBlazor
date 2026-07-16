using System.Linq;
using AwesomeAssertions;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor.Interop;
using MudBlazor.UnitTests.Mocks;
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
    [NonParallelizable] // ScriptDiagnostics.MissingScriptLogged is process-wide; keep other fixtures from racing the reset.
    public void MissingMudBlazorScript_LogsGuidanceOnceAndDoesNotCrash()
    {
        // https://github.com/MudBlazor/MudBlazor/issues/13477
        // When the MudBlazor script isn't referenced, window.mudElementRef is undefined and the
        // first-render blur-attach interop throws. That must not tear down the circuit; instead
        // we log actionable guidance once, no matter how many inputs are on the page.
        Context.JSInterop
            .SetupVoid("mudElementRef.addOnBlurEvent", _ => true)
            .SetException(new JSException("Could not find 'mudElementRef.addOnBlurEvent' ('mudElementRef' was undefined)."));

        var provider = new MockLoggerProvider();
        var logger = (MockLogger)provider.CreateLogger(GetType().FullName!);
        Context.Services.AddLogging(x => x.ClearProviders().AddProvider(provider));

        ScriptDiagnostics.MissingScriptLogged = false;

        var render = () =>
        {
            Context.Render<MudInput<string>>();
            Context.Render<MudInput<int>>();
        };

        render.Should().NotThrow();

        var errors = logger.GetEntries().Where(e => e.Level == LogLevel.Error).ToList();
        errors.Should().ContainSingle();
        errors[0].Message.Should().Be(ScriptDiagnostics.MissingScriptMessage);
    }
}
