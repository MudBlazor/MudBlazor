
using System;
using System.Globalization;
using Bunit;
using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ProgressCircularTests : BunitTest
    {
        [Test]
        public void DefaultValues()
        {
            var circular = new MudProgressCircular();

            circular.Color.Should().Be(Color.Default);
            circular.Size.Should().Be(Size.Medium);
            circular.Indeterminate.Should().BeFalse();
            circular.Rounded.Should().BeFalse();
            circular.Min.Should().Be(0.0);
            circular.Max.Should().Be(100.0);
            circular.Value.Should().Be(0.0);
            circular.StrokeWidth.Should().Be(3);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DefaultStructure(bool indeterminate)
        {
            var minValue = -500;
            var maxValue = 500;
            var valueValue = -400;
            var strokeWidthValue = 50;

            var comp = Context.RenderComponent<MudProgressCircular>(x =>
                {
                    x.Add(y => y.Value, valueValue);
                    x.Add(y => y.Min, minValue);
                    x.Add(y => y.Max, maxValue);
                    x.Add(y => y.StrokeWidth, strokeWidthValue);
                    x.Add(y => y.Class, "my-custom-class");
                    x.Add(y => y.Indeterminate, indeterminate);
                });

            var container = comp.Find(".my-custom-class");
            container.GetAttribute("role").Should().Be("progressbar");
            container.GetAttribute("aria-valuenow").Should().Be(valueValue.ToString());
            container.GetAttribute("aria-valuemin").Should().Be(minValue.ToString());
            container.GetAttribute("aria-valuemax").Should().Be(maxValue.ToString());
            container.GetAttribute("aria-live").Should().Be(
                indeterminate ?
                 null : "polite");
            container.ChildElementCount.Should().Be(1);

            var circleContainer = container.Children[0];
            circleContainer.ClassList.Should().Contain("mud-progress-circular-svg");
            circleContainer.ChildElementCount.Should().Be(1);

            var circleElement = circleContainer.Children[0];
            circleElement.ClassList.Should().Contain("mud-progress-circular-circle");

            if (indeterminate)
            {
                circleElement.GetAttribute("stroke-width").Should().Be(strokeWidthValue.ToString());
                circleElement.GetAttribute("style").Should().BeNullOrEmpty();
            }
            else
            {
                circleElement.GetAttribute("stroke-width").Should().Be(strokeWidthValue.ToString());
                circleElement.GetAttribute("style").Should().Be("stroke-dasharray: 126; stroke-dashoffset: 113;");
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(true)]
        public void TestClassesForRounded(bool rounded)
        {
            var comp = Context.RenderComponent<MudProgressCircular>(x => x.Add(y => y.Rounded, rounded));

            var container = comp.Find(".mud-progress-circular-circle");

            if (rounded)
            {
                container.ClassList.Should().Contain("mud-progress-circular-circle-rounded");
            }
            else
            {
                container.ClassList.Should().NotContain("mud-progress-circular-circle-rounded");
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(true)]
        public void TestClassesForIntermediate(bool indeterminate)
        {
            var comp = Context.RenderComponent<MudProgressCircular>(x => x.Add(y => y.Indeterminate, indeterminate));

            var container = comp.Find(".mud-progress-circular");

            if (indeterminate)
            {
                container.ClassList.Should().Contain("mud-progress-indeterminate");
            }
            else
            {
                container.ClassList.Should().NotContain("mud-progress-static");
            }

            var circleContainer = comp.Find(".mud-progress-circular-circle");

            if (indeterminate)
            {
                circleContainer.ClassList.Should().Contain("mud-progress-indeterminate");
            }
            else
            {
                circleContainer.ClassList.Should().NotContain("mud-progress-static");
            }
        }

        [Test]
        [TestCase(Size.Large, "large")]
        [TestCase(Size.Medium, "medium")]
        [TestCase(Size.Small, "small")]
        public void TestClassesForSize(Size size, string expectedString)
        {
            var comp = Context.RenderComponent<MudProgressCircular>(x => x.Add(y => y.Size, size));

            var container = comp.Find(".mud-progress-circular");

            container.ClassList.Should().Contain($"mud-progress-{expectedString}");
        }

        [Test]
        [TestCase(Color.Success, "success")]
        [TestCase(Color.Surface, "surface")]
        [TestCase(Color.Error, "error")]
        public void TestClassesForColor(Color color, string expectedString)
        {
            var comp = Context.RenderComponent<MudProgressCircular>(x => x.Add(y => y.Color, color));

            var container = comp.Find(".mud-progress-circular");

            container.ClassList.Should().Contain($"mud-{expectedString}-text");
        }
    }
}
