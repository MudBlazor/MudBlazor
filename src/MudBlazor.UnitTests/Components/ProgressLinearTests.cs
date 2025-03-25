
using System;
using System.Globalization;
using Bunit;
using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ProgressLinearTests : BunitTest
    {
        [Test]
        public void DefaultValues()
        {
            var linear = new MudProgressLinear();

            linear.BufferValue.Should().Be(0);
            linear.Value.Should().Be(0);
            linear.Buffer.Should().BeFalse();
            linear.ChildContent.Should().BeNull();
            linear.Color.Should().Be(Color.Default);
            linear.Indeterminate.Should().BeFalse();
            linear.Max.Should().Be(100.0);
            linear.Min.Should().Be(0.0);
            linear.Rounded.Should().BeFalse();
            linear.Size.Should().Be(Size.Small);
            linear.Striped.Should().BeFalse();
            linear.Value.Should().Be(0.0);
            linear.Vertical.Should().BeFalse();
        }

        [Test]
        [TestCase(0, 100, 30, 20, 30.0, 20.0)]
        [TestCase(0, 10, 3, 2, 30.0, 20.0)]
        [TestCase(0, 10, 0.03, 0.02, 0.3, 0.2)]

        [TestCase(-100, 0, -70, -80, 30.0, 20.0)]
        [TestCase(0, 200, 30, 20, 15, 10)]
        [TestCase(200, 400, 230, 220, 15, 10)]
        public void CheckingPercentageAndBufferValue(double min, double max, double value, double buffervalue, double expectedValue, double expectedBufferValue)
        {
            var comp = Context.RenderComponent<MudProgressLinear>(x =>
            {
                x.Add(y => y.Min, min);
                x.Add(y => y.Max, max);
                x.Add(y => y.Value, value);
                x.Add(y => y.BufferValue, buffervalue);
            });

            comp.Instance.Value.Should().Be(value);
            comp.Instance.GetValuePercent().Should().Be(expectedValue);

            comp.Instance.BufferValue.Should().Be(buffervalue);
            comp.Instance.GetBufferPercent().Should().Be(expectedBufferValue);
        }

        [Test]
        [TestCase(0, 100, -30, -20, 0.0, 0.0)]
        [TestCase(0, 100, 30, -20, 30, 0.0)]
        [TestCase(0, 100, -30, 20, 0.0, 20)]

        [TestCase(0, 100, 130, 120, 100, 100)]
        [TestCase(0, 100, 30, 120, 30, 100)]
        [TestCase(0, 100, 130, 20, 100, 20)]

        [TestCase(0, 0, 30, 20, 0.0, 0.0)]

        public void EnsureMaxAndMinConsitency(double min, double max, double value, double buffervalue, double expectedValue, double expectedBufferValue)
        {
            var comp = Context.RenderComponent<MudProgressLinear>(x =>
            {
                x.Add(y => y.Min, min);
                x.Add(y => y.Max, max);
                x.Add(y => y.Value, value);
                x.Add(y => y.BufferValue, buffervalue);
            });

            comp.Instance.Value.Should().Be(value);
            comp.Instance.GetValuePercent().Should().Be(expectedValue);

            comp.Instance.BufferValue.Should().Be(buffervalue);
            comp.Instance.GetBufferPercent().Should().Be(expectedBufferValue);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DefaultStructure(bool isVertical)
        {
            var comp = Context.RenderComponent<MudProgressLinear>(x =>
                {
                    x.Add(y => y.Min, -500);
                    x.Add(y => y.Max, 500);
                    x.Add(y => y.Value, -400);
                    x.Add(y => y.Class, "my-custom-class");
                    x.Add(y => y.Vertical, isVertical);
                });

            var container = comp.Find(".my-custom-class");
            container.GetAttribute("role").Should().Be("progressbar");

            container.ChildElementCount.Should().Be(1);
            var barContainer = container.Children[0];

            barContainer.ClassList.Should().Contain("mud-progress-linear-bars");
            barContainer.ChildElementCount.Should().Be(1);

            var barElement = barContainer.Children[0];
            barElement.ClassList.Should().Contain("mud-progress-linear-bar");

            barElement.GetAttribute("style").Should().Be(
                isVertical ?
                $"transform: translateY(90%);" : $"transform: translateX(-90%);");
        }

        [Test]
        public void IndeterminateStructure()
        {
            var comp = Context.RenderComponent<MudProgressLinear>(x =>
            {
                x.Add(y => y.Min, -500);
                x.Add(y => y.Max, 500);
                x.Add(y => y.Value, -400);
                x.Add(y => y.Class, "my-custom-class");
                x.Add(y => y.Indeterminate, true);
            });

            var container = comp.Find(".my-custom-class");
            container.GetAttribute("role").Should().Be("progressbar");

            container.ChildElementCount.Should().Be(1);
            var barContainer = container.Children[0];

            barContainer.ClassList.Should().Contain("mud-progress-linear-bars");
            barContainer.ChildElementCount.Should().Be(2);

            var firstBarElement = barContainer.Children[0];
            firstBarElement.ClassList.Should().Contain("mud-progress-linear-bar");

            var secondBarElement = barContainer.Children[1];
            firstBarElement.ClassList.Should().Contain("mud-progress-linear-bar");
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void BufferStructure(bool isVertical)
        {
            var comp = Context.RenderComponent<MudProgressLinear>(x =>
            {
                x.Add(y => y.Min, -500);
                x.Add(y => y.Max, 500);
                x.Add(y => y.Value, -400);
                x.Add(y => y.BufferValue, -100);
                x.Add(y => y.Class, "my-custom-class");
                x.Add(y => y.Buffer, true);
                x.Add(y => y.Vertical, isVertical);
            });

            var container = comp.Find(".my-custom-class");
            container.GetAttribute("role").Should().Be("progressbar");


            container.ChildElementCount.Should().Be(1);
            var barContainer = container.Children[0];

            barContainer.ClassList.Should().Contain("mud-progress-linear-bars");
            barContainer.ChildElementCount.Should().Be(3);

            var firstBarElement = barContainer.Children[0];
            firstBarElement.ClassList.Should().Contain("mud-progress-linear-bar");

            var secondBarElement = barContainer.Children[1];
            secondBarElement.ClassList.Should().Contain("mud-progress-linear-bar");

            secondBarElement.GetAttribute("style").Should().Be(
                isVertical ?
                $"transform: translateY(90%);" : $"transform: translateX(-90%);");

            var thirdBarElement = barContainer.Children[2];
            thirdBarElement.ClassList.Should().Contain("mud-progress-linear-bar", "last");

            thirdBarElement.GetAttribute("style").Should().Be(
                isVertical ?
                $"transform: translateY(60%);" : $"transform: translateX(-60%);");
        }

        [Test]
        public void IndeterminateWithChildContent()
        {
            var comp = Context.RenderComponent<MudProgressLinear>(x =>
            {
                x.Add(y => y.Min, -500);
                x.Add(y => y.Max, 500);
                x.Add(y => y.Value, -400);
                x.Add(y => y.Class, "my-custom-class");
                x.Add(y => y.Indeterminate, true);
                x.Add(y => y.ChildContent, "<p>my content</p>");
            });

            var container = comp.Find(".my-custom-class");
            container.GetAttribute("role").Should().Be("progressbar");

            container.ChildElementCount.Should().Be(2);
            var barContainer = container.Children[0];

            barContainer.ClassList.Should().Contain("mud-progress-linear-bars");
            barContainer.ChildElementCount.Should().Be(2);

            var firstBarElement = barContainer.Children[0];
            firstBarElement.ClassList.Should().Contain("mud-progress-linear-bar");

            var secondBarElement = barContainer.Children[1];
            firstBarElement.ClassList.Should().Contain("mud-progress-linear-bar");

            var contentContainer = container.Children[1];

            contentContainer.ClassList.Should().Contain("mud-progress-linear-content");
            contentContainer.ChildElementCount.Should().Be(1);
            contentContainer.TextContent.Should().Be("my content");
        }

        [Test]
        [TestCase(true)]
        [TestCase(true)]
        public void TestClassesForRounded(bool rounded)
        {
            var comp = Context.RenderComponent<MudProgressLinear>(x => x.Add(y => y.Rounded, rounded));

            var container = comp.Find(".mud-progress-linear");

            if (rounded)
            {
                container.ClassList.Should().Contain("mud-progress-linear-rounded");
            }
            else
            {
                container.ClassList.Should().NotContain("mud-progress-linear-rounded");
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(true)]
        public void TestClassesForStriped(bool striped)
        {
            var comp = Context.RenderComponent<MudProgressLinear>(x => x.Add(y => y.Striped, striped));

            var container = comp.Find(".mud-progress-linear");

            if (striped)
            {
                container.ClassList.Should().Contain("mud-progress-linear-striped");
            }
            else
            {
                container.ClassList.Should().NotContain("mud-progress-linear-striped");
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(true)]
        public void TestClassesForIntermediate(bool indeterminate)
        {
            var comp = Context.RenderComponent<MudProgressLinear>(x => x.Add(y => y.Indeterminate, indeterminate));

            var container = comp.Find(".mud-progress-linear");

            if (indeterminate)
            {
                container.ClassList.Should().Contain("mud-progress-indeterminate");
            }
            else
            {
                container.ClassList.Should().NotContain("mud-progress-indeterminate");
            }
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void TestClassesForBuffer(bool buffer, bool indeterminate)
        {
            var comp = Context.RenderComponent<MudProgressLinear>(x =>
            {
                x.Add(y => y.Indeterminate, indeterminate);
                x.Add(y => y.Buffer, buffer);

            });

            var container = comp.Find(".mud-progress-linear");

            if (buffer && indeterminate == false)
            {
                container.ClassList.Should().Contain("mud-progress-linear-buffer");
            }
            else
            {
                container.ClassList.Should().NotContain("mud-progress-linear-buffer");
            }
        }

        [Test]
        [TestCase(Size.Large, "large")]
        [TestCase(Size.Medium, "medium")]
        [TestCase(Size.Small, "small")]
        public void TestClassesForSize(Size size, string expectedString)
        {
            var comp = Context.RenderComponent<MudProgressLinear>(x => x.Add(y => y.Size, size));

            var container = comp.Find(".mud-progress-linear");

            container.ClassList.Should().Contain($"mud-progress-linear-{expectedString}");
        }

        [Test]
        [TestCase(Color.Success, "success")]
        [TestCase(Color.Surface, "surface")]
        [TestCase(Color.Error, "error")]
        public void TestClassesForColor(Color color, string expectedString)
        {
            var comp = Context.RenderComponent<MudProgressLinear>(x => x.Add(y => y.Color, color));

            var container = comp.Find(".mud-progress-linear");

            container.ClassList.Should().Contain($"mud-progress-linear-color-{expectedString}");
        }

        [Test]
        [TestCase(true)]
        [TestCase(true)]
        public void TestClassesForVertical(bool vertical)
        {
            var comp = Context.RenderComponent<MudProgressLinear>(x => x.Add(y => y.Vertical, vertical));

            var container = comp.Find(".mud-progress-linear");

            if (vertical)
            {
                container.ClassList.Should().Contain("vertical");
                container.ClassList.Should().NotContain("horizontal");

            }
            else
            {
                container.ClassList.Should().Contain("horizontal");
                container.ClassList.Should().NotContain("vertical");
            }
        }

        [Test]
        [TestCase("en-us")]
        [TestCase("de-DE")]
        [TestCase("he-IL")]
        [TestCase("ar-ER")]
        public void AriaValuesInDifferentCultures(string cultureString)
        {
            var culture = new CultureInfo(cultureString, false);

            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            var comp = Context.RenderComponent<MudProgressLinear>(x =>
            {
                x.Add(y => y.Min, 10.2);
                x.Add(y => y.Max, 125.22);
                x.Add(y => y.Value, 75.3);
                x.Add(y => y.Class, "my-custom-class");
            });

            var container = comp.Find(".my-custom-class");
            container.GetAttribute("role").Should().Be("progressbar");

            container.GetAttribute("aria-valuenow").Should().Be("75.3");
            container.GetAttribute("aria-valuemin").Should().Be("10.2");
            container.GetAttribute("aria-valuemax").Should().Be("125.22");
        }
    }
}
