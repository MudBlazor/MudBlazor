using FluentAssertions;
using MudBlazor.Extensions;


using NUnit.Framework;
// Disable obsolete warning of the extension method for this unit test.
#pragma warning disable CS0618


namespace MudBlazor.UnitTests.Extensions
{
    [TestFixture]
    public class EnumExtensionsTests
    {
        [Test]
        public void ToDescriptionString()
        {
            EnumExtensions.ToDescriptionString(Adornment.Start).Should().Be("start");
            EnumExtensions.ToDescriptionString(Align.Inherit).Should().Be("inherit");
            EnumExtensions.ToDescriptionString(Breakpoint.Sm).Should().Be("sm");
        }
    }
}
