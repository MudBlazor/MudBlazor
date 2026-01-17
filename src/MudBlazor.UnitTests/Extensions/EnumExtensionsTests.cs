using AwesomeAssertions;
using MudBlazor.Extensions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions
{
    [TestFixture]
    public class EnumExtensionsTests
    {
        [Test]
        [TestCase(null, new string[0])]
        [TestCase(typeof(Adornment), new[] { "None", "Start", "End" })]
        [TestCase(typeof(Adornment?), new[] { "None", "Start", "End" })]
        [TestCase(typeof(string), new string[0])]
        public void GetSafeEnumValues_Test(Type type, string[] expectedNames)
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

        [TestCase(Adornment.Start, Edge.Start)]
        [TestCase(Adornment.End, Edge.End)]
        [TestCase(Adornment.None, Edge.False)]
        [TestCase((Adornment)999, Edge.False)] // Invalid adornment value
        public void Adornment_ToEdge_Should_ReturnExpectedValue(Adornment adornment, Edge expectedEdge)
        {
            // Act
            var result = adornment.ToEdge();

            // Assert
            result.Should().Be(expectedEdge);
        }
    }
}
