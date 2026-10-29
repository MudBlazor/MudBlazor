#nullable enable
using AwesomeAssertions;
using Bunit;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class AvatarTests : BunitTest
{
    /// <summary>
    /// Avatar variants retain the requested color classes, including Text for issue #13982.
    /// </summary>
    [Test]
    public void Avatar_VariantAndColorClasses(
        [Values(Variant.Text, Variant.Filled, Variant.Outlined)] Variant variant,
        [Values(Color.Default, Color.Primary, Color.Error)] Color color)
    {
        var comp = Context.Render<MudAvatar>(parameters => parameters
            .Add(p => p.Variant, variant)
            .Add(p => p.Color, color)
            .AddChildContent("MK"));

        var avatar = comp.Find(".mud-avatar");
        avatar.ClassList.Should().Contain($"mud-avatar-{variant.ToStringFast(true)}");
        avatar.ClassList.Should().Contain($"mud-avatar-{variant.ToStringFast(true)}-{color.ToStringFast(true)}");
        avatar.TextContent.Should().Be("MK");
    }

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
    /// A blank ARIA name is no name, so the avatar must not claim role="img" for it.
    /// </summary>
    [Test]
    [TestCase("aria-label", null)]
    [TestCase("aria-label", "")]
    [TestCase("aria-label", "   ")]
    [TestCase("aria-labelledby", null)]
    [TestCase("aria-labelledby", "")]
    public void Avatar_WithBlankAccessibleName_ShouldNotClaimImageRole(string attribute, string? value)
    {
        var comp = Context.Render<MudAvatar>(parameters => parameters
            .AddUnmatched(attribute, value)
            .AddChildContent("AB"));

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
