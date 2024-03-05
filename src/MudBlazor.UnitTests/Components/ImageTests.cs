// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
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

            var comp = Context.RenderComponent<MudImage>(p =>
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

            img.ClassList.Should().BeEquivalentTo(new[] { "my-custom-class", "mud-elevation-25", "object-bottom", "object-cover", "mud-image", "fluid" });
        }

        [Test]
        [TestCase(ObjectFit.Contain, "contain")]
        [TestCase(ObjectFit.Cover, "cover")]
        [TestCase(ObjectFit.Fill, "fill")]
        [TestCase(ObjectFit.None, "none")]
        [TestCase(ObjectFit.ScaleDown, "scale-down")]
        public void Image_ObjectFitToClassMapping(ObjectFit fit, string expectedClass)
        {

            var comp = Context.RenderComponent<MudImage>(p =>
            {
                p.Add(x => x.ObjectFit, fit);
            });

            var img = comp.Find("img");
            img.ClassList.Should().Contain(new[] { "mud-image", $"object-{expectedClass}" });
        }

        [Test]
        [TestCase(ObjectPosition.Bottom, "bottom")]
        [TestCase(ObjectPosition.Center, "center")]
        [TestCase(ObjectPosition.Left, "left")]
        [TestCase(ObjectPosition.LeftBottom, "left-bottom")]
        [TestCase(ObjectPosition.LeftTop, "left-top")]
        [TestCase(ObjectPosition.Right, "right")]
        [TestCase(ObjectPosition.RightBottom, "right-bottom")]
        [TestCase(ObjectPosition.RightTop, "right-top")]
        [TestCase(ObjectPosition.Top, "top")]
        public void Image_ObjectPositionToClassMapping(ObjectPosition position, string expectedClass)
        {

            var comp = Context.RenderComponent<MudImage>(p =>
            {
                p.Add(x => x.ObjectPosition, position);
            });

            var img = comp.Find("img");
            img.ClassList.Should().Contain(new[] { "mud-image", $"object-{expectedClass}" });
        }
    }
}

