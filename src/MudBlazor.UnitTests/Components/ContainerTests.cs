using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ContainerTests : BunitTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GuttersProperty_AddsClass(bool gutters)
        {
            // Arrange
            var component = Context.RenderComponent<MudContainer>(builder => builder
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
