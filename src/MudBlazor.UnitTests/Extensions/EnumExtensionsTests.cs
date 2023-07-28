using System.Linq;
using System;
using FluentAssertions;
using MudBlazor.UnitTests.Dummy;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions
{
    [TestFixture]
    public class EnumExtensionsTests
    {
        [Test]
        [TestCase(typeof(Size), new[] { "Small", "Medium", "Large" })]
        [TestCase(typeof(Size?), new[] { "Small", "Medium", "Large" })]
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
            Size.Large.ToDescriptionString().Should().Be("large");
            Align.Inherit.ToDescriptionString().Should().Be("inherit");
            Breakpoint.Sm.ToDescriptionString().Should().Be("sm");
        }

#pragma warning disable CS0618
        /// <remarks>Remove this test(including DummyEnumEmpty) during The Big Break: Breaking Changes in v7</remarks>>
        [Test]
        public void ToDescriptionStringOld()
        {
            DummyEnumEmpty? dummyNullEnum = 0;
            MudBlazor.Extensions.EnumExtensions.ToDescriptionString(Size.Medium).Should().Be("medium");
            MudBlazor.Extensions.EnumExtensions.ToDescriptionString(Align.Inherit).Should().Be("inherit");
            MudBlazor.Extensions.EnumExtensions.ToDescriptionString(Breakpoint.Sm).Should().Be("sm");
            MudBlazor.Extensions.EnumExtensions.ToDescriptionString(dummyNullEnum).Should().Be("0");
        }
#pragma warning restore CS0618
    }
}
