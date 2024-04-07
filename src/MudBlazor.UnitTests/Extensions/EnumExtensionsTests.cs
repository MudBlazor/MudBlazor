using System;
using System.Linq;
using FluentAssertions;
using MudBlazor.Extensions;
using MudBlazor.UnitTests.Dummy;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions
{
    [TestFixture]
    public class EnumExtensionsTests
    {
        [Test]
        [TestCase(typeof(Adornment), new[] { "None", "Start", "End" })]
        [TestCase(typeof(Adornment?), new[] { "None", "Start", "End" })]
        [TestCase(typeof(string), new string[0])]
        public void GetSafeEnumValues_Test(Type type, string[] expectedNames)
        {
            var values = MudBlazor.Extensions.EnumExtensions.GetSafeEnumValues(type);
            var stringValues = values.Select(x => x.ToString());
            stringValues.Should().BeEquivalentTo(expectedNames);
        }

        [Test]
        public void ToDescriptionStringNew()
        {
            Adornment.Start.ToDescriptionString().Should().Be("start");
            Align.Inherit.ToDescriptionString().Should().Be("inherit");
            Breakpoint.Sm.ToDescriptionString().Should().Be("sm");
        }
        
        [Test]
        [TestCase(typeof(Adornment), new[] { "None", "start", "end" })]
        public void GetEnumDisplayName_Test(Type type, string[] expectedNames)
        {
            foreach (var expectedName in expectedNames)
            {
                var enumValue = Enum.Parse(type, expectedName, true) as Enum;
                var displayName = enumValue?.GetEnumDisplayName();
                displayName.Should().Be(expectedName);
            }
        }
        
    }
}
