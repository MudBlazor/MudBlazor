using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.UnitTests.TestComponents.Input;
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
    public async Task WithImmediate_BindValueModifiedOnChange_SoTextUpdated()
    {
        // Arrange

        var comp = Context.RenderComponent<InputToUpperTest>(parameters => parameters
            .Add(i => i.Immediate, true)
        );
        var stu = comp.Instance;
        var input = comp.FindComponent<MudInput<string>>().Instance;

        // Act

        await comp.Find("input").InputAsync(new ChangeEventArgs { Value = "AbCdEfGhI" });

        // Assert

        stu.Value.Should().Be("ABCDEFGHI");
        input.Value.Should().Be("ABCDEFGHI");
        input.Text.Should().Be("ABCDEFGHI");
    }

    [Test]
    public async Task WithoutImmediate_BindValueModifiedOnChange_SoTextUpdated()
    {
        // Arrange

        var comp = Context.RenderComponent<InputToUpperTest>(parameters => parameters
            .Add(i => i.Immediate, false)
        );
        var stu = comp.Instance;
        var input = comp.FindComponent<MudInput<string>>().Instance;

        // Act

        comp.Find("input").KeyDown("a"); // Trick to force the focus on input
        await comp.Find("input").ChangeAsync(new ChangeEventArgs { Value = "AbCdEfGhI" });

        // Assert

        stu.Value.Should().Be("ABCDEFGHI");
        input.Value.Should().Be("ABCDEFGHI");
        input.Text.Should().Be("ABCDEFGHI");
    }
}
