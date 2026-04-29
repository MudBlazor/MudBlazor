using AwesomeAssertions;
using MudBlazor.Extensions;

namespace MudBlazor.UnitTests.Extensions
{
    public class EnumExtensionsTests
    {
        [Test]
        [Arguments(null, new string[0])]
        [Arguments(typeof(Adornment), new[] { "None", "Start", "End" })]
        [Arguments(typeof(Adornment?), new[] { "None", "Start", "End" })]
        [Arguments(typeof(string), new string[0])]
        public void GetSafeEnumValues(Type type, string[] expectedNames)
        {
            var values = EnumExtensions.GetSafeEnumValues(type);
            var stringValues = values.Select(x => x.ToString());
            stringValues.Should().BeEquivalentTo(expectedNames);
        }

        [Test]
        public void ToStringFast_ShouldReturnLoweredStrings()
        {
            Adornment.Start.ToStringFast(true).Should().Be("start");
            Align.Inherit.ToStringFast(true).Should().Be("inherit");
            Breakpoint.Sm.ToStringFast(true).Should().Be("sm");
        }

        [Test]
        [Arguments(Adornment.Start, Edge.Start)]
        [Arguments(Adornment.End, Edge.End)]
        [Arguments(Adornment.None, Edge.False)]
        [Arguments((Adornment)999, Edge.False)] // Invalid adornment value
        public void Adornment_ToEdge_Should_ReturnExpectedValue(Adornment adornment, Edge expectedEdge)
        {
            // Act
            var result = adornment.ToEdge();

            // Assert
            result.Should().Be(expectedEdge);
        }
    }
}