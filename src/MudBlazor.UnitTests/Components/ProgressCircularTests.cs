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
            container.ChildElementCount.Should().Be(2);

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
        [TestCase(false)]
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
        [TestCase(false)]
        public void TestClassesForIntermediate(bool indeterminate)
        {
            var comp = Context.RenderComponent<MudProgressCircular>(x => x.Add(y => y.Indeterminate, indeterminate));

            var container = comp.Find(".mud-progress-circular");

            if (indeterminate)
            {
                container.ClassList.Should().Contain("mud-progress-indeterminate");
                container.ClassList.Should().NotContain("mud-progress-static");
            }
            else
            {
                container.ClassList.Should().Contain("mud-progress-static");
                container.ClassList.Should().NotContain("mud-progress-indeterminate");
            }

            var circleContainer = comp.Find(".mud-progress-circular-circle");

            if (indeterminate)
            {
                circleContainer.ClassList.Should().Contain("mud-progress-indeterminate");
                circleContainer.ClassList.Should().NotContain("mud-progress-static");
            }
            else
            {
                circleContainer.ClassList.Should().Contain("mud-progress-static");
                circleContainer.ClassList.Should().NotContain("mud-progress-indeterminate");
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

        [Test]
        [TestCase(3, "22.5 22.5 43 43")]
        [TestCase(5, "21.5 21.5 45 45")]
        [TestCase(10, "19 19 50 50")]
        [TestCase(1, "23.5 23.5 41 41")]
        [TestCase(0, "24 24 40 40")]
        public void MudProgressCircular_ShouldHaveCorrectViewBox_BasedOnStrokeWidth(int strokeWidth, string expectedViewBox)
        {
            var comp = Context.RenderComponent<MudProgressCircular>(parameters => parameters
                .Add(p => p.StrokeWidth, strokeWidth)
            );

            // Find the SVG element
            var svgElement = comp.Find("svg");
            svgElement.Should().NotBeNull();

            // Check the viewBox attribute
            var viewBoxAttribute = svgElement.GetAttribute("viewBox");
            viewBoxAttribute.Should().Be(expectedViewBox);

            // Test with Indeterminate = true as well
            var compIndeterminate = Context.RenderComponent<MudProgressCircular>(parameters => parameters
                .Add(p => p.StrokeWidth, strokeWidth)
                .Add(p => p.Indeterminate, true)
            );

            var svgElementIndeterminate = compIndeterminate.Find("svg");
            svgElementIndeterminate.Should().NotBeNull();

            var viewBoxAttributeIndeterminate = svgElementIndeterminate.GetAttribute("viewBox");
            viewBoxAttributeIndeterminate.Should().Be(expectedViewBox);
        }
    }
}
