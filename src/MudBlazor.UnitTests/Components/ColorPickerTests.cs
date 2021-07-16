#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.Utilities;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components.Components
{
    [TestFixture]
    public class ColorPickerTests
    {
        private const double _defaultXForColorPanel = 208.5;
        private const double _defaultYForColorPanel = 28.43;
        private const string _hueSliderCssSelector = ".mud-slider.mud-picker-color-slider.hue input";
        private const string _alphaSliderCssSelector = ".mud-picker-color-slider.alpha input";
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddTestServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        [Test]
        public async Task Default()
        {
            throw new NotImplementedException();
        }

        private void CheckColorRelatedValues(IRenderedComponent<SimpleColorPickerTest> comp, double expectedX, double expectedY, MudColor expectedColor, ColorPickerMode mode)
        {
            comp.Instance.ColorValue.Should().Be(expectedColor);

            if (mode == ColorPickerMode.RGB || mode == ColorPickerMode.HSL)
            {
                var castedInputs = GetColorInputs(comp);

                if (mode == ColorPickerMode.RGB)
                {
                    castedInputs[0].Value.Should().Be(expectedColor.R.ToString());
                    castedInputs[1].Value.Should().Be(expectedColor.G.ToString());
                    castedInputs[2].Value.Should().Be(expectedColor.B.ToString());
                    castedInputs[3].Value.Should().Be(expectedColor.A.ToString());
                }
                else
                {
                    castedInputs[0].Value.Should().Be(expectedColor.H.ToString());
                    castedInputs[1].Value.Should().Be(expectedColor.S.ToString());
                    castedInputs[2].Value.Should().Be(expectedColor.L.ToString());
                    castedInputs[3].Value.Should().Match(x => double.Parse(x) == Math.Round((expectedColor.A / 255.0), 2));
                }
            }
            else if (mode == ColorPickerMode.HEX)
            {
                var castedInputs = GetColorInputs(comp, 1);
                castedInputs[0].Value.Should().Be(expectedColor.Value);
            }

            var selector = comp.Find(".mud-picker-color-selector");
            selector.Should().NotBeNull();

            var selectorStyleAttribute = selector.GetAttribute("style");
            selectorStyleAttribute.Should().Be($"transform: translate({expectedX.ToString(CultureInfo.InvariantCulture)}px, {expectedY.ToString(CultureInfo.InvariantCulture)}px);");

            var hueSlideValue = comp.FindAll(".mud-picker-color-slider.hue input");
            hueSlideValue.Should().ContainSingle();
            hueSlideValue[0].Should().BeAssignableTo<IHtmlInputElement>();

            (hueSlideValue[0] as IHtmlInputElement).Value.Should().Be(((int)expectedColor.H).ToString());

            var alphaSlider = comp.FindAll(_alphaSliderCssSelector);
            alphaSlider.Should().ContainSingle();
            alphaSlider[0].Should().BeAssignableTo<IHtmlInputElement>();

            (alphaSlider[0] as IHtmlInputElement).Value.Should().Be(((int)expectedColor.A).ToString());

            var alphaSliderStyleAttritbute = (alphaSlider[0].Parent as IHtmlElement).GetAttribute("style");

            alphaSliderStyleAttritbute.Should().Be($"background-image: linear-gradient(to right, transparent, {expectedColor.ToRGB()});");
        }

        private IHtmlInputElement[] GetColorInputs(IRenderedComponent<SimpleColorPickerTest> comp, int expectedCount = 4)
        {
            var inputs = comp.FindAll(".mud-picker-color-inputs input");

            inputs.Should().AllBeAssignableTo<IHtmlInputElement>();
            inputs.Should().HaveCount(expectedCount);

            var castedInputs = inputs.Cast<IHtmlInputElement>().ToArray();

            return castedInputs;
        }

        private IHtmlInputElement GetColorInput(IRenderedComponent<SimpleColorPickerTest> comp, int index, int expectedCount = 4) => GetColorInputs(comp, expectedCount)[index];

        [Test]
        [TestCase(40, 255.13, _defaultYForColorPanel)]
        public void SetR(byte r, double selectorXPosition, double selectorYPosition)
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>();
            Console.WriteLine(comp.Markup);

            IHtmlInputElement rInput = GetColorInput(comp, 0);

            var expectedColor = comp.Instance.ColorValue.SetR(r);
            rInput.Change(expectedColor.R.ToString());

            CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.RGB);
        }

        [Test]
        [TestCase(240, 195.04, 14.71)]
        public void SetG(byte g, double selectorXPosition, double selectorYPosition)
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>();
            Console.WriteLine(comp.Markup);

            IHtmlInputElement gInput = GetColorInput(comp, 1);

            var expectedColor = comp.Instance.ColorValue.SetG(g);

            gInput.Change(expectedColor.G.ToString());

            CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.RGB);
        }

        [Test]
        [TestCase(90, 55.11, 161.76)]
        public void SetB(byte b, double selectorXPosition, double selectorYPosition)
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>();
            Console.WriteLine(comp.Markup);

            IHtmlInputElement bInput = GetColorInput(comp, 2);

            var expectedColor = comp.Instance.ColorValue.SetB(b);

            bInput.Change(expectedColor.B.ToString());

            CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.RGB);
        }

        [Test]
        [TestCase(90, _defaultXForColorPanel, _defaultYForColorPanel)]
        public void SetA_InRGBMode(byte a, double selectorXPosition, double selectorYPosition)
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>();
            Console.WriteLine(comp.Markup);

            IHtmlInputElement aInput = GetColorInput(comp, 3);

            var expectedColor = comp.Instance.ColorValue.SetAlpha(a);

            aInput.Change(expectedColor.A.ToString());

            CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.RGB);
        }

        [Test]
        [TestCase(90, 207.12, _defaultYForColorPanel)]
        public void SetH(int h, double selectorXPosition, double selectorYPosition)
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.ColorPickerMode, ColorPickerMode.HSL));
            Console.WriteLine(comp.Markup);

            IHtmlInputElement hInput = GetColorInput(comp, 0);

            var expectedColor = comp.Instance.ColorValue.SetH(h);

            hInput.Change(expectedColor.H.ToString());

            CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.HSL);
        }

        [Test]
        [TestCase(0.4, 134.01, 61.76)]
        public void SetS(double s, double selectorXPosition, double selectorYPosition)
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.ColorPickerMode, ColorPickerMode.HSL));
            Console.WriteLine(comp.Markup);

            IHtmlInputElement sColor = GetColorInput(comp, 1);

            var expectedColor = comp.Instance.ColorValue.SetS(s);

            sColor.Change(expectedColor.S.ToString());

            CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.HSL);
        }

        [Test]
        [TestCase(0.67, 162.38, 23.53)]
        public void SetL(double l, double selectorXPosition, double selectorYPosition)
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.ColorPickerMode, ColorPickerMode.HSL));
            Console.WriteLine(comp.Markup);

            IHtmlInputElement lColor = GetColorInput(comp, 2);

            var expectedColor = comp.Instance.ColorValue.SetL(l);
            lColor.Change(expectedColor.L.ToString());

            CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.HSL);
        }

        [Test]
        [TestCase(0.5, _defaultXForColorPanel, _defaultYForColorPanel)]
        public void SetAlpha_AsHLS(double a, double selectorXPosition, double selectorYPosition)
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.ColorPickerMode, ColorPickerMode.HSL));
            Console.WriteLine(comp.Markup);

            IHtmlInputElement lColor = GetColorInput(comp, 3);

            var expectedColor = comp.Instance.ColorValue.SetAlpha(a);
            lColor.Change(a.ToString());

            CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.HSL);
        }

        [Test]
        [TestCase("#8cb829ff", 240.92, 69.61)]
        public void SetColorInput(string colorHexString, double selectorXPosition, double selectorYPosition)
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.ColorPickerMode, ColorPickerMode.HEX));
            Console.WriteLine(comp.Markup);

            var inputs = comp.FindAll(".mud-picker-color-inputs input");

            IHtmlInputElement lColor = GetColorInput(comp, 0, 1);

            var expectedColor = colorHexString;
            lColor.Change(colorHexString);

            CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.HEX);
        }

        [Test]
        public void SetAlphaSlider()
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>();
            Console.WriteLine(comp.Markup);

            for (int i = 256 - 1; i >= 0; i--)
            {
                var expectedColor = comp.Instance.ColorValue.SetAlpha((byte)i);

                var hueColerSlider = comp.FindAll(_alphaSliderCssSelector);
                hueColerSlider.Should().ContainSingle();
                hueColerSlider[0].Should().BeAssignableTo<IHtmlInputElement>();

                InputEventDispatchExtensions.Input(hueColerSlider[0], i.ToString());

                CheckColorRelatedValues(comp, _defaultXForColorPanel, _defaultYForColorPanel, expectedColor, ColorPickerMode.RGB);
            }
        }

        [Test]
        public void SetHueSlider()
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>();
            Console.WriteLine(comp.Markup);

            for (int i = 0; i < 360; i++)
            {
                var expectedColor = comp.Instance.ColorValue.SetH(i);

                var alphaColerSlider = comp.FindAll(_hueSliderCssSelector);
                alphaColerSlider.Should().ContainSingle();
                alphaColerSlider[0].Should().BeAssignableTo<IHtmlInputElement>();

                InputEventDispatchExtensions.Input(alphaColerSlider[0], i.ToString());

                CheckColorRelatedValues(comp, 207.12, 28.43, expectedColor, ColorPickerMode.RGB);
            }
        }

        [Test]
        public void Click_ColorPanel()
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>();
            Console.WriteLine(comp.Markup);

            var overlay = comp.Find(".mud-picker-color-overlay-black .mud-picker-color-overlay");

            double x = 99.2;
            double y = 200.98;

            overlay.Click(new Microsoft.AspNetCore.Components.Web.MouseEventArgs { OffsetX = x, OffsetY = y });

            MudColor expectedColor = "#232232ff";

            CheckColorRelatedValues(comp, x, y, expectedColor, ColorPickerMode.RGB);
        }

        [Test]
        public void Click_ModeBtton()
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>();
            Console.WriteLine(comp.Markup);

            MudColor color = comp.Instance.ColorValue;
            var modeButton = comp.Find(".mud-picker-control-switch button");

            //default to rgb
            CheckColorRelatedValues(comp, _defaultXForColorPanel, _defaultYForColorPanel, color, ColorPickerMode.RGB);

            // click to switch to HSL
            modeButton.Click();
            CheckColorRelatedValues(comp, _defaultXForColorPanel, _defaultYForColorPanel, color, ColorPickerMode.HSL);

            //click again to switch to hex
            modeButton.Click();
            CheckColorRelatedValues(comp, _defaultXForColorPanel, _defaultYForColorPanel, color, ColorPickerMode.HEX);

            //click last time to reset to RGB
            modeButton.Click();
            CheckColorRelatedValues(comp, _defaultXForColorPanel, _defaultYForColorPanel, color, ColorPickerMode.RGB);
        }




    }
}
