using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents.ButtonGroup;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ButtonGroupTests : BunitTest
    {
        [Test]
        public void WithFullWidthAndNoneButtonIsStreched_ThenAllButtonsStreched()
        {
            // Arrange

            var comp = Context.RenderComponent<ButtonGroupWithThreeButtons>(
                parameters => parameters
                    .Add(c => c.ButtonGroupFullWidth, true)
                    .Add(c => c.Button1FullWidth, false)
                    .Add(c => c.Button2FullWidth, false)
                    .Add(c => c.Button3FullWidth, false)
            );

            // Assert

            comp.FindAll(".mud-button-group-root.mud-width-full").Count.Should().Be(1);
            comp.FindAll(".mud-button-root.mud-width-full").Count.Should().Be(3);
        }

        [Test]
        public void WithFullWidthAndOneButtonIsStreched_ThenOtherButtonsNotStreched()
        {
            // Arrange

            var comp = Context.RenderComponent<ButtonGroupWithThreeButtons>(
                parameters => parameters
                    .Add(c => c.ButtonGroupFullWidth, true)
                    .Add(c => c.Button1FullWidth, true)
                    .Add(c => c.Button2FullWidth, false)
                    .Add(c => c.Button3FullWidth, false)
            );

            // Assert

            comp.FindAll(".mud-button-group-root.mud-width-full").Count.Should().Be(1);
            var buttonComps = comp.FindAll(".mud-button-root");
            buttonComps[0].ClassList.Should().Contain("mud-width-full");
            buttonComps[1].ClassList.Should().NotContain("mud-width-full");
            buttonComps[2].ClassList.Should().NotContain("mud-width-full");
        }

        [Test]
        public void WithFullWidth_WhenButtonWithFullWidthIsRemoved_ThenOtherButtonsAreStreched()
        {
            // Arrange

            var comp = Context.RenderComponent<ButtonGroupWithThreeButtons>(
                parameters => parameters
                    .Add(c => c.ButtonGroupFullWidth, true)
                    .Add(c => c.Button1FullWidth, true)
                    .Add(c => c.Button2FullWidth, false)
                    .Add(c => c.Button3FullWidth, false)
            );

            // Act

            comp.SetParam(c => c.Button1Displayed, false);

            // Assert

            comp.FindAll(".mud-button-group-root.mud-width-full").Count.Should().Be(1);
            var buttonComps = comp.FindAll(".mud-button-root");
            buttonComps.Count.Should().Be(2);
            buttonComps[0].ClassList.Should().Contain("mud-width-full");
            buttonComps[1].ClassList.Should().Contain("mud-width-full");
        }
    }
}
