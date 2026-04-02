using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
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
    public async Task ReadOnlyShouldTriggerOnBlur()
    {
        var calls = 0;
        FocusEventArgs? args = null;
        var comp = Context.Render<MudInput<string>>(parameters => parameters
            .Add(p => p.ReadOnly, true)
            .Add(p => p.OnBlur, x =>
            {
                calls++;
                args = x;
            }));

        await comp.Find("input").BlurAsync();

        calls.Should().Be(1);
        args.Should().NotBeNull();
        args!.Type.Should().Contain(".additional");
    }
}
