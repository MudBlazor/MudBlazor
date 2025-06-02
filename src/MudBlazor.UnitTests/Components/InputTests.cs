using Bunit;
using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

#nullable enable
[TestFixture]
public class InputTests : BunitTest
{
    [Test]
    public void ReadOnlyShouldNotHaveClearButton()
    {
        var comp = Context.RenderComponent<MudInput<string>>(p => p
            .Add(x => x.Text, "some value")
            .Add(x => x.Clearable, true)
            .Add(x => x.ReadOnly, false));

        comp.FindAll(".mud-input-clear-button").Count.Should().Be(1);

        comp.SetParametersAndRender(p => p.Add(x => x.ReadOnly, true)); //no clear button when readonly
        comp.FindAll(".mud-input-clear-button").Count.Should().Be(0);
    }

    [Test]

    public void AdornmentTextWithOnAdornmentClickShouldBeButton()
    {
        var comp = Context.RenderComponent<MudInput<string>>(p => p
            .Add(x => x.Text, "some value")
            .Add(x => x.AdornmentText, "Click Here")
            .Add(x => x.Adornment, Adornment.End)
            .Add(x => x.OnAdornmentClick, () => { }));
        comp.Find("button.mud-button-text").Should().NotBeNull("Should render an button");
    }
#nullable disable
}
