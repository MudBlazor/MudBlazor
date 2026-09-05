using AwesomeAssertions;
using Bunit;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class AvatarTests : BunitTest
    {
        /// <summary>
        /// An avatar without an accessible name must not claim role="img", which would hide its initials.
        /// </summary>
        [Test]
        public void Avatar_WithoutAccessibleName_ShouldNotClaimImageRole()
        {
            var comp = Context.Render<MudAvatar>(parameters => parameters.AddChildContent("AB"));

            // role="img" without a name hides the initials from assistive technology.
            comp.Find("div.mud-avatar").HasAttribute("role").Should().BeFalse();
        }

        /// <summary>
        /// An avatar with an ARIA name exposes role="img".
        /// </summary>
        [Test]
        [TestCase("aria-label", "Jane Doe")]
        [TestCase("aria-labelledby", "avatar-name")]
        public void Avatar_WithAccessibleName_ShouldExposeImageRole(string attribute, string value)
        {
            var comp = Context.Render<MudAvatar>(parameters => parameters
                .AddUnmatched(attribute, value)
                .AddChildContent("AB"));

            var avatar = comp.Find("div.mud-avatar");
            avatar.GetAttribute("role").Should().Be("img");
            avatar.GetAttribute(attribute).Should().Be(value);
        }
    }
}
