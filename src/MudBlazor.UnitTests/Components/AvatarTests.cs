using System;
using Bunit;
using FluentAssertions;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class AvatarTests : BunitTest
    {
        /// <summary>
        /// MudAvatar without Alt renders image without alt property
        /// </summary>
        [Test]
        public void MudAvatarShouldRenderImageWithoutAltIfAltIsEmptyString()
        {
            var imageUrl = Parameter(nameof(MudAvatar.Image),
                "https://images.freeimages.com/images/small-previews/cd5/lady-avatar-1632969.jpg");
            var alt = Parameter(nameof(MudAvatar.Alt), "");

            var comp = Context.RenderComponent<MudAvatar>(imageUrl, alt);
            // print the generated html

            var imageElement = comp.Find("img");
            imageElement.ClassName.Should().Contain("mud-avatar-img");
            imageElement.Attributes.GetNamedItem("src")?.Value.Should()
                .Be("https://images.freeimages.com/images/small-previews/cd5/lady-avatar-1632969.jpg");
            imageElement.Attributes.Should().NotContain(e => e.Name == "alt");
        }

        /// <summary>
        /// MudAvatar without Alt renders image without alt property
        /// </summary>
        [Test]
        public void MudAvatarShouldRenderImageWithoutAltIfAltIsNotSet()
        {
            var imageUrl = Parameter(nameof(MudAvatar.Image),
                "https://images.freeimages.com/images/small-previews/cd5/lady-avatar-1632969.jpg");

            var comp = Context.RenderComponent<MudAvatar>(imageUrl);
            // print the generated html

            var imageElement = comp.Find("img");
            imageElement.ClassName.Should().Contain("mud-avatar-img");
            imageElement.Attributes.GetNamedItem("src")?.Value.Should()
                .Be("https://images.freeimages.com/images/small-previews/cd5/lady-avatar-1632969.jpg");
            imageElement.Attributes.Should().NotContain(e => e.Name == "alt");
        }

        /// <summary>
        /// MudAvatar with Alt renders image with alt property
        /// </summary>
        [Test]
        public void MudAvatarShouldRenderImageWithAlt()
        {
            var imageUrl = Parameter(nameof(MudAvatar.Image),
                "https://images.freeimages.com/images/small-previews/cd5/lady-avatar-1632969.jpg");
            var alt = Parameter(nameof(MudAvatar.Alt), "Lady Avatar");

            var comp = Context.RenderComponent<MudAvatar>(imageUrl, alt);
            // print the generated html

            var imageElement = comp.Find("img");
            imageElement.ClassName.Should().Contain("mud-avatar-img");
            imageElement.Attributes.GetNamedItem("src")?.Value.Should()
                .Be("https://images.freeimages.com/images/small-previews/cd5/lady-avatar-1632969.jpg");
            imageElement.Attributes.GetNamedItem("alt")?.Value.Should()
                .Be("Lady Avatar");
        }



    }
}



