using FluentAssertions;
using MudBlazor.Extensions;


using NUnit.Framework;


namespace MudBlazor.UnitTests.Extensions
{
    [TestFixture]
    public class EnumExtensionsTests
    {
        [Test]
        public void ToDescriptionString()
        {
            Adornment.Start.ToDescriptionString().Should().Be("start");
            Align.Inherit.ToDescriptionString().Should().Be("inherit");
            Breakpoint.Sm.ToDescriptionString().Should().Be("sm");
        }
    }
}
