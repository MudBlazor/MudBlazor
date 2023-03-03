using FluentAssertions;
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
        /// <remarks>Remove during The Big Break: Breaking Changes in v7</remarks>>
        [Test]
        public void ToDescriptionStringOld()
        {
            MudBlazor.Extensions.EnumExtensions.ToDescriptionString(Adornment.Start).Should().Be("start");
            MudBlazor.Extensions.EnumExtensions.ToDescriptionString(Align.Inherit).Should().Be("inherit");
            MudBlazor.Extensions.EnumExtensions.ToDescriptionString(Breakpoint.Sm).Should().Be("sm");
        }
#pragma warning restore CS0618
    }
}
