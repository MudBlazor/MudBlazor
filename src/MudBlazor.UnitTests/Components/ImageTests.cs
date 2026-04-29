// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using AwesomeAssertions;
using Bunit;

namespace MudBlazor.UnitTests.Components
{
    public class ImageTests : BunitTest
    {
        [Test]
        public void Image_DefaultValues()
        {
            var image = new MudImage();

            image.Fluid.Should().BeFalse();
            image.Src.Should().BeNullOrEmpty();
            image.Alt.Should().BeNullOrEmpty();
            image.Height.Should().BeNull();
            image.Width.Should().BeNull();
            image.Elevation.Should().Be(0);
            image.ObjectFit.Should().Be(ObjectFit.Fill);
            image.ObjectPosition.Should().Be(ObjectPosition.Center);
        }

        [Test]
        public void Image_GeneralStructure()
        {

            var comp = Context.Render<MudImage>(p =>
            {
                p.Add(x => x.Fluid, true);
                p.Add(x => x.Src, "https://myimgsource.com/image.png");
                p.Add(x => x.Alt, "my description");
                p.Add(x => x.Height, 20);
                p.Add(x => x.Width, 120);
                p.Add(x => x.Elevation, 25);
                p.Add(x => x.ObjectFit, ObjectFit.Cover);
                p.Add(x => x.ObjectPosition, ObjectPosition.Bottom);
                p.Add(x => x.Class, "my-custom-class");
                p.Add(x => x.Style, "background:gray");
            });

            var img = comp.Find("img");
            img.GetAttribute("src").Should().Be("https://myimgsource.com/image.png");
            img.GetAttribute("alt").Should().Be("my description");
            img.GetAttribute("height").Should().Be("20");
            img.GetAttribute("width").Should().Be("120");
            img.GetAttribute("style").Should().Be("background:gray");

            img.ClassList.Should().BeEquivalentTo("my-custom-class", "mud-elevation-25", "object-bottom", "object-cover", "mud-image", "fluid");
        }

        [Test]
        [Arguments(ObjectFit.Contain, "contain")]
        [Arguments(ObjectFit.Cover, "cover")]
        [Arguments(ObjectFit.Fill, "fill")]
        [Arguments(ObjectFit.None, "none")]
        [Arguments(ObjectFit.ScaleDown, "scale-down")]
        public void Image_ObjectFitToClassMapping(ObjectFit fit, string expectedClass)
        {

            var comp = Context.Render<MudImage>(p =>
            {
                p.Add(x => x.ObjectFit, fit);
            });

            var img = comp.Find("img");
            img.ClassList.Should().Contain(new[] { "mud-image", $"object-{expectedClass}" });
        }

        [Test]
        [Arguments(ObjectPosition.Bottom, "bottom")]
        [Arguments(ObjectPosition.Center, "center")]
        [Arguments(ObjectPosition.Left, "left")]
        [Arguments(ObjectPosition.LeftBottom, "left-bottom")]
        [Arguments(ObjectPosition.LeftTop, "left-top")]
        [Arguments(ObjectPosition.Right, "right")]
        [Arguments(ObjectPosition.RightBottom, "right-bottom")]
        [Arguments(ObjectPosition.RightTop, "right-top")]
        [Arguments(ObjectPosition.Top, "top")]
        public void Image_ObjectPositionToClassMapping(ObjectPosition position, string expectedClass)
        {

            var comp = Context.Render<MudImage>(p =>
            {
                p.Add(x => x.ObjectPosition, position);
            });

            var img = comp.Find("img");
            img.ClassList.Should().Contain(["mud-image", $"object-{expectedClass}"]);
        }

        [Test]
        public async Task SwitchesToFallbackSrcOnError()
        {
            var initialSrc = "primary-image.jpg";
            var fallbackSrc = "fallback-image.jpg";

            var comp = Context.Render<MudImage>(parameters => parameters
                .Add(p => p.Src, initialSrc)
                .Add(p => p.FallbackSrc, fallbackSrc)
            );

            // Trigger the `onerror` event
            await comp.Find("img").ErrorAsync();

            var img = comp.Find("img");

            img.GetAttribute("src").Should().Be(fallbackSrc);
        }

        [Test]
        public async Task FallbackMissingOnError()
        {
            var initialSrc = "primary-image.jpg";

            var comp = Context.Render<MudImage>(parameters => parameters
                .Add(p => p.Src, initialSrc)
            );

            // Trigger the `onerror` event
            await comp.Find("img").ErrorAsync();

            var img = comp.Find("img");

            img.GetAttribute("src").Should().Be(initialSrc);
        }
    }
}
