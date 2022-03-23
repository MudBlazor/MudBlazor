using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class SliderTests : BunitTest
    {
        [Test]
        public void DefaultValues()
        {
            var slider = new MudSlider<double>();

            slider.Min.Should().Be(0.0);
            slider.Max.Should().Be(100.0);
            slider.Step.Should().Be(1.0);
            slider.Value.Should().Be(0.0);

            slider.TickMarks.Should().BeFalse();
            slider.Vertical.Should().BeFalse();
            slider.Disabled.Should().BeFalse();
            slider.Immediate.Should().BeTrue();
            slider.TickMarkLabels.Should().BeNull();

            slider.Converter.Should().NotBeNull();
            slider.Color.Should().Be(Color.Primary);

            slider.Variant.Should().Be(Variant.Text);
            slider.Variant.Should().Be(Variant.Text);

            slider.Size.Should().Be(Size.Small);
        }

        [Test]
        [TestCase(Size.Small, "small")]
        [TestCase(Size.Medium, "medium")]
        [TestCase(Size.Large, "large")]
        public void CheckSizeCssClass(Size size, string expectedSizeClass)
        {
            var comp = Context.RenderComponent<MudSlider<int>>(x => x.Add(p => p.Size, size));

            var slider = comp.Find(".mud-slider");
            slider.ClassList.Should().ContainInOrder(new[] { "mud-slider", $"mud-slider-{expectedSizeClass}" });
        }


        [Test]
        public void CheckVerticalClass()
        {
            var verticalSliderComponent = Context.RenderComponent<MudSlider<int>>(x => x.Add(p => p.Vertical, true));

            var verticalSlider = verticalSliderComponent.Find(".mud-slider");
            verticalSlider.ClassList.Should().ContainInOrder(new[] { "mud-slider", "mud-slider-small", "mud-slider-vertical" });

            var horizontalSliderComponent = Context.RenderComponent<MudSlider<int>>(x => x.Add(p => p.Vertical, true));

            var horizontalSlider = horizontalSliderComponent.Find(".mud-slider");
            horizontalSlider.ClassList.Should().ContainInOrder(new[] { "mud-slider", "mud-slider-small" });
        }

        [Test]
        [TestCase(Size.Small, "small")]
        [TestCase(Size.Medium, "medium")]
        [TestCase(Size.Large, "large")]
        public void CheckInputSizeCssClass(Size size, string expectedSizeClass)
        {
            var comp = Context.RenderComponent<MudSlider<int>>(x => x.Add(p => p.Size, size));

            var slider = comp.Find(".mud-slider");
            slider.ClassList.Should().ContainInOrder(new[] { "mud-slider", $"mud-slider-{expectedSizeClass}", "mud-slider-primary" });
        }

        [Test]
        [TestCase(Color.Default, "default")]
        [TestCase(Color.Primary, "primary")]
        [TestCase(Color.Secondary, "secondary")]
        [TestCase(Color.Tertiary, "tertiary")]
        [TestCase(Color.Info, "info")]
        [TestCase(Color.Success, "success")]
        [TestCase(Color.Warning, "warning")]
        [TestCase(Color.Error, "error")]
        [TestCase(Color.Dark, "dark")]
        [TestCase(Color.Transparent, "transparent")]
        [TestCase(Color.Inherit, "inherit")]
        [TestCase(Color.Surface, "surface")]
        public void CheckColorCssClass(Color color, string expectedColorClass)
        {
            var comp = Context.RenderComponent<MudSlider<int>>(x => x.Add(p => p.Color, color));

            var slider = comp.Find(".mud-slider");
            slider.ClassList.Should().ContainInOrder(new[] { "mud-slider", "mud-slider-small", $"mud-slider-{expectedColorClass}" });
        }

        [Test]
        public void GenerellStructure()
        {
            var comp = Context.RenderComponent<MudSlider<double>>(x =>
            {
                x.Add(p => p.Min, 100.0);
                x.Add(p => p.Max, 200.0);
                x.Add(p => p.Step, 10.0);
                x.Add(p => p.Value, 120.0);

            });

            comp.Nodes.Should().ContainSingle();

            var rootElement = comp.Find(".mud-slider");
            (comp.Nodes.First() as IHtmlElement).InnerHtml.Should().Be(rootElement.InnerHtml);

            rootElement.Children.Should().ContainSingle();

            var container = rootElement.Children.First();
            container.ClassList.Should().Contain("mud-slider-container");

            container.Children.Should().HaveCount(1);

            var input = container.Children.ElementAt(0);

            input.ClassList.Should().Contain("mud-slider-input");
            (input as IHtmlInputElement).Value.Should().Be("120");

            var expectedAttributes = new Dictionary<string, string>()
            {
                { "aria-valuenow","120" },
                { "aria-valuemin","100" },
                { "aria-valuemax","200" },
                { "role","slider" },
                { "min","100" },
                { "max","200" },
                { "step","10" },
            };

            foreach (var item in expectedAttributes)
            {
                input.GetAttribute(item.Key).Should().Be(item.Value);
            }
        }

        [Test]
        public void Structure_WithChildContent()
        {
            var comp = Context.RenderComponent<SliderWithContentTest>(x => x.Add(p => p.Text, "my text"));

            comp.Nodes.Should().ContainSingle();

            var rootElement = comp.Find(".mud-slider");

            (comp.Nodes.First() as IHtmlElement).InnerHtml.Should().Be(rootElement.InnerHtml);

            rootElement.Children.Should().HaveCount(2);

            var childContent = rootElement.Children.ElementAt(0);
            childContent.TextContent.Should().Be("my text");
            var container = rootElement.Children.ElementAt(1);
            container.ClassList.Should().Contain("mud-slider-container");

            container.Children.Should().HaveCount(1);

            container.Children.ElementAt(0).ClassList.Should().Contain("mud-slider-input");
        }

        [Test]
        public void Structure_WithFilled()
        {
            var comp = Context.RenderComponent<MudSlider<double>>(x =>
            {
                x.Add(p => p.Value, 20.0);
                x.Add(p => p.Variant, Variant.Filled);
            });

            comp.Nodes.Should().ContainSingle();

            var rootElement = comp.Find(".mud-slider");
            (comp.Nodes.First() as IHtmlElement).InnerHtml.Should().Be(rootElement.InnerHtml);

            rootElement.Children.Should().ContainSingle();

            var container = rootElement.Children.First();
            container.ClassList.Should().Contain("mud-slider-container");

            container.Children.Should().HaveCount(2);

            var containerInner = container.Children.ElementAt(0);
            containerInner.ClassList.Should().Contain("mud-slider-inner-container");

            var filling = containerInner.Children.ElementAt(0);
            filling.ClassList.Should().Contain("mud-slider-filled");

            container.Children.ElementAt(1).ClassList.Should().Contain("mud-slider-input");
        }

        [Test]
        public void TickMarksEnabled_ButNoLabels()
        {
            var comp = Context.RenderComponent<MudSlider<double>>(x =>
            {
                x.Add(p => p.Value, 20.0);
                x.Add(p => p.Step, 25.0);
                x.Add(p => p.TickMarks, true);
            });

            comp.Nodes.Should().ContainSingle();

            var rootElement = comp.Find(".mud-slider");
            (comp.Nodes.First() as IHtmlElement).InnerHtml.Should().Be(rootElement.InnerHtml);

            rootElement.Children.Should().ContainSingle();

            var container = rootElement.Children.First();
            container.ClassList.Should().Contain("mud-slider-container");

            container.Children.Should().HaveCount(2);

            var filling = container.Children.ElementAt(0);

            filling.ClassList.Should().Contain("mud-slider-inner-container");
            filling.Children.Should().ContainSingle();

            var tickMarks = filling.Children.First();
            tickMarks.ClassList.Should().Contain("mud-slider-tickmarks");
            tickMarks.Children.Should().HaveCount(5);

            foreach (var item in tickMarks.Children)
            {
                item.ClassList.Should().Contain(new[] { "d-flex", "flex-column", "relative" });
                item.Children.Should().ContainSingle();

                item.Children.First().ClassList.Should().Contain("mud-slider-track-tick");
            }

            container.Children.ElementAt(1).ClassList.Should().Contain("mud-slider-input");
        }

        [Test]
        public void TickMarksEnabled_ButLabels()
        {
            var labels = new[] { "red", "green", "yello", "blue", "black" };

            var comp = Context.RenderComponent<MudSlider<double>>(x =>
            {
                x.Add(p => p.Value, 20.0);
                x.Add(p => p.Step, 25.0);
                x.Add(p => p.TickMarks, true);
                x.Add(p => p.TickMarkLabels, labels);
            });

            comp.Nodes.Should().ContainSingle();

            var rootElement = comp.Find(".mud-slider");
            (comp.Nodes.First() as IHtmlElement).InnerHtml.Should().Be(rootElement.InnerHtml);

            rootElement.Children.Should().ContainSingle();

            var container = rootElement.Children.First();
            container.ClassList.Should().Contain("mud-slider-container");

            container.Children.Should().HaveCount(2);

            var filling = container.Children.ElementAt(0);

            filling.ClassList.Should().Contain("mud-slider-inner-container");
            filling.Children.Should().ContainSingle();

            var tickMarks = filling.Children.First();
            tickMarks.ClassList.Should().Contain("mud-slider-tickmarks");
            tickMarks.Children.Should().HaveCount(5);

            Int32 itemCounter = 0;
            foreach (var item in tickMarks.Children)
            {
                item.ClassList.Should().Contain(new[] { "d-flex", "flex-column", "relative" });
                item.Children.Should().HaveCount(2);

                item.Children.First().ClassList.Should().Contain("mud-slider-track-tick");

                var content = item.Children.ElementAt(1);
                content.ClassList.Should().Contain("mud-slider-track-tick-label");
                content.TextContent.Should().Be(labels[itemCounter++]);
            }

            container.Children.ElementAt(1).ClassList.Should().Contain("mud-slider-input");
        }

        [Test]
        [TestCase(0.0, 100.0, 25.0, 5)]
        [TestCase(0.0, 100.0, 10.0, 11)]
        [TestCase(0.0, 100.0, 1.0, 101)]

        [TestCase(100.0, 200.0, 25.0, 5)]
        [TestCase(-200.0, -100.0, 25.0, 5)]
        public void TickMarksEnabled_CheckAmount(double min, double max, double step, int expectedAmount)
        {
            var comp = Context.RenderComponent<MudSlider<double>>(x =>
            {
                x.Add(p => p.Min, min);
                x.Add(p => p.Max, max);
                x.Add(p => p.Step, step);
                x.Add(p => p.TickMarks, true);
            });

            comp.Nodes.Should().ContainSingle();

            var tickMarks = comp.Find(".mud-slider-tickmarks");
            tickMarks.Children.Should().HaveCount(expectedAmount);
        }

        [Test]
        [TestCase(0.0, 100.0, 0, "0")]
        [TestCase(0.0, 100.0, 20, "20")]
        [TestCase(0.0, 100.0, 22.5, "22.5")]
        [TestCase(0.0, 100.0, 50, "50")]
        [TestCase(0.0, 100.0, 100, "100")]

        [TestCase(0.0, 1.0, 0.0, "0")]
        [TestCase(0.0, 1.0, 0.2, "20")]
        [TestCase(0.0, 1.0, 0.5, "50")]
        [TestCase(0.0, 1.0, 1, "100")]

        [TestCase(1.0, 2.0, 1.0, "0")]
        [TestCase(1.0, 2.0, 1.2, "20")]
        [TestCase(1.0, 2.0, 1.5, "50")]
        [TestCase(1.0, 2.0, 2, "100")]

        [TestCase(-100.0, 100.0, -100.0, "0")]
        [TestCase(-100.0, 100.0, -50, "25")]
        [TestCase(-100.0, 100.0, 0, "50")]
        [TestCase(-100.0, 100.0, 50, "75")]
        [TestCase(-100.0, 100.0, 100, "100")]

        [TestCase(0.0, 100.0, 110, "100")]
        [TestCase(0.0, 100.0, -10, "0")]
        [TestCase(-200.0, -100.0, -90, "100")]
        [TestCase(-200.0, -100.0, -210, "0")]
        public void Percentage(double min, double max, double value, string expectedPercentage)
        {
            var cultures = new[] {
                new CultureInfo("en-us", false),
                new CultureInfo("de-DE", false),
                new CultureInfo("he-IL", false),
                new CultureInfo("ar-ER", false),
            };

            foreach (var culture in cultures)
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;

                var comp = Context.RenderComponent<MudSlider<double>>(x =>
                {
                    x.Add(p => p.Max, max);
                    x.Add(p => p.Min, min);
                    x.Add(p => p.Value, value);
                    x.Add(p => p.Variant, Variant.Filled);
                    x.Add(p => p.ValueLabel, true);
                });

                var thumb = comp.Find(".mud-slider-value-label");
                thumb.GetAttribute("style").Should().Be($"left:{expectedPercentage}%;");

                var filling = comp.Find(".mud-slider-filled");
                filling.GetAttribute("style").Should().Be($"width:{expectedPercentage}%;");
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public async Task CheckInput(bool immediate)
        {
            var comp = Context.RenderComponent<MudSlider<double>>(x =>
            {
                x.Add(p => p.Max, 200);
                x.Add(p => p.Min, 100);
                x.Add(p => p.Value, 150);
                x.Add(p => p.Immediate, immediate);
                x.Add(p => p.Variant, Variant.Filled);
            });

            var input = comp.Find(".mud-slider-input");
            var filling = comp.Find(".mud-slider-filled");
            var eventArgs = new ChangeEventArgs { Value = "180" };

            if (immediate == false)
            {
                Assert.ThrowsAsync<MissingEventHandlerException>(() => input.InputAsync(eventArgs));
                await input.ChangeAsync(eventArgs);
            }
            else
            {
                Assert.ThrowsAsync<MissingEventHandlerException>(() => input.ChangeAsync(eventArgs));
                await input.InputAsync(eventArgs);
            }

            filling.GetAttribute("style").Should().Be($"width:80%;");
        }
    }
}



