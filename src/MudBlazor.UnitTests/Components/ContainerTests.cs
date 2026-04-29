using AwesomeAssertions;

namespace MudBlazor.UnitTests.Components
{
    public class ContainerTests : BunitTest
    {
        [Test]
        [Arguments(true)]
        [Arguments(false)]
        public void GuttersProperty_AddsClass(bool gutters)
        {
            // Arrange
            var component = Context.Render<MudContainer>(builder => builder
                .Add(p => p.Gutters, gutters)
            );

            // Assert
            if (gutters)
            {
                component.Markup.Should().Contain("mud-container--gutters");
            }
            else
            {
                component.Markup.Should().NotContain("mud-container--gutters");
            }
        }
    }
}