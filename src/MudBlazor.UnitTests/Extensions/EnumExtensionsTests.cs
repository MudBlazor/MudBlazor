using FluentAssertions;
using MudBlazor.UnitTests.Dummy;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions
{
    [TestFixture]
    public class EnumExtensionsTests
    {
        [Test]
        public void ToDescriptionStringNew()
        {
            Adornment.Start.ToDescriptionString().Should().Be("start");
            Align.Inherit.ToDescriptionString().Should().Be("inherit");
            Breakpoint.Sm.ToDescriptionString().Should().Be("sm");
        }

#pragma warning disable CS0618
        /// <remarks>Remove this test(including DummyEnumEmpty) during The Big Break: Breaking Changes in v7</remarks>>
        [Test]
        public void ToDescriptionStringOld()
        {
            DummyEnumEmpty? dummyNullEnum = 0;
            MudBlazor.Extensions.EnumExtensions.ToDescriptionString(Adornment.Start).Should().Be("start");
            MudBlazor.Extensions.EnumExtensions.ToDescriptionString(Align.Inherit).Should().Be("inherit");
            MudBlazor.Extensions.EnumExtensions.ToDescriptionString(Breakpoint.Sm).Should().Be("sm");
            MudBlazor.Extensions.EnumExtensions.ToDescriptionString(dummyNullEnum).Should().Be("0");
        }
#pragma warning restore CS0618
    }
}
