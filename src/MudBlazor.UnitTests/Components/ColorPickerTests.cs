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
using Microsoft.AspNetCore.Components.Web;
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
        private static MudColor _defaultColor = "#594ae2";
        private const string _hueSliderCssSelector = ".mud-slider.mud-picker-color-slider.hue input";
        private const string _alphaSliderCssSelector = ".mud-picker-color-slider.alpha input";
        private const string _colorDotCssSelector = ".mud-picker-color-fill";
        private const string _toolbarCssSelector = ".mud-toolbar";
        private const string _mudColorPickerCssSelector = ".mud-picker-color-picker";
        private const string _slidersControlCssSelector = ".mud-picker-color-sliders";
        private const string _colorInputCssSelector = ".mud-picker-color-inputs";
        private const string _colorInputModeSwitchCssSelector = ".mud-picker-control-switch";
        private const string _alphaInputCssSelector = ".input-field-alpha";
        private Bunit.TestContext ctx;

        private MockEventListener _eventListener;

        [SetUp]
        public void Setup()
        {
            _eventListener = new MockEventListener();

            ctx = new Bunit.TestContext();
            ctx.AddTestServices();
            ctx.Services.AddSingleton<IEventListener>(_eventListener);
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

            alphaSliderStyleAttritbute.Should().Be($"background-image: linear-gradient(to right, transparent, {expectedColor.ToString(MudColorOutputFormats.RGB)});");
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
        public void MouseMove()
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>();
            Console.WriteLine(comp.Markup);

            MouseEventArgs args = new MouseEventArgs
            {
                OffsetX = 117.0,
                OffsetY = 123.0,
                Buttons = 1,
            };

            _eventListener.FireEvent(args);

            var expectedColor = new MudColor(85, 80, 129, 255);

            CheckColorRelatedValues(comp, args.OffsetX, args.OffsetY, expectedColor, ColorPickerMode.RGB);

            MouseEventArgs argsWihtoutLeftButtonPushed = new MouseEventArgs
            {
                OffsetX = 117.0,
                OffsetY = 123.0,
                Buttons = 0,
            };

            _eventListener.FireEvent(argsWihtoutLeftButtonPushed);
            CheckColorRelatedValues(comp, args.OffsetX, args.OffsetY, expectedColor, ColorPickerMode.RGB);
        }

        [Test]
        [TestCase(PickerVariant.Dialog)]
        [TestCase(PickerVariant.Inline)]
        public async Task MouseMove_InDialogMode(PickerVariant variant)
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>(p => p.Add(y =>
            y.Variant, variant));

            Console.WriteLine(comp.Markup);

            MouseEventArgs args = new MouseEventArgs
            {
                OffsetX = 117.0,
                OffsetY = 123.0,
                Buttons = 1,
            };

            _eventListener.Callbacks.Values.Should().BeEmpty();

            await comp.Instance.OpenPicker();
            _eventListener.Callbacks.Values.Should().ContainSingle();

            _eventListener.FireEvent(args);

            var expectedColor = new MudColor(85, 80, 129, 255);

            CheckColorRelatedValues(comp, args.OffsetX, args.OffsetY, expectedColor, ColorPickerMode.RGB);

            MouseEventArgs argsWihtoutLeftButtonPushed = new MouseEventArgs
            {
                OffsetX = 117.0,
                OffsetY = 123.0,
                Buttons = 0,
            };

            _eventListener.FireEvent(argsWihtoutLeftButtonPushed);
            CheckColorRelatedValues(comp, args.OffsetX, args.OffsetY, expectedColor, ColorPickerMode.RGB);

            await comp.Instance.ClosePicker();

            _eventListener.Callbacks.Should().BeEmpty();
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

        [Test]
        public void ColorPalette_Interaction()
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>();
            Console.WriteLine(comp.Markup);

            var colorDot = comp.Find(_colorDotCssSelector);
            // no collection
            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-picker-color-collection"));
            colorDot.Click();

            // collection found
            var colorsToSelectPanel = comp.Find(".mud-picker-color-collection");
            colorsToSelectPanel.Children.Should().HaveCount(5);

            var expectedColors = new MudColor[] { "#ff4081ff", "#2196f3ff", "#00c853ff", "#ff9800ff", "#f44336ff" };

            for (int i = 0; i < 5; i++)
            {
                var styleAtribute = colorsToSelectPanel.Children[i].GetAttribute("style");
                styleAtribute.Should().Be($"background: {expectedColors[i].ToString(MudColorOutputFormats.RGBA)};");
            }

            colorDot.Click();

            // again no collection visible
            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-picker-color-collection"));
        }

        [Test]
        public void ColorPalette_CustomColors()
        {
            var expectedColors = new MudColor[] { "#23af3daa", "#56a23dff", "#56a85dff" };

            var comp = ctx.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.Palette, expectedColors));
            Console.WriteLine(comp.Markup);

            var colorDot = comp.Find(_colorDotCssSelector);
            colorDot.Click();

            // collection found
            var colorsToSelectPanel = comp.Find(".mud-picker-color-collection");
            colorsToSelectPanel.Children.Should().HaveCount(expectedColors.Length);

            for (int i = 0; i < expectedColors.Length; i++)
            {
                var styleAtribute = colorsToSelectPanel.Children[i].GetAttribute("style");
                styleAtribute.Should().Be($"background: {expectedColors[i].ToString(MudColorOutputFormats.RGBA)};");
            }
        }

        [Test]
        public void ColorPalette_SelectColor()
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>();
            Console.WriteLine(comp.Markup);

            var colorDot = comp.Find(_colorDotCssSelector);
            // no collection
            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-picker-color-collection"));
            colorDot.Click();

            var expectedColors = new MudColor[] { "#ff4081ff", "#2196f3ff", "#00c853ff", "#ff9800ff", "#f44336ff" };

            for (int i = 0; i < 5; i++)
            {
                var colorsToSelectPanel = comp.Find(".mud-picker-color-collection");

                colorsToSelectPanel.Children[i].Click();
                colorDot = comp.Find(_colorDotCssSelector);

                var styleAtribute = colorDot.GetAttribute("style");
                styleAtribute.Should().Be($"background: {expectedColors[i].ToString(MudColorOutputFormats.RGBA)};");
                comp.Instance.ColorValue.Should().Be(expectedColors[i]);

                colorDot.Click();
            }
        }

        [Test]
        public void Toogle_Toolbar()
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.DisableToolbar, false));
            Console.WriteLine(comp.Markup);

            _ = comp.Find(_toolbarCssSelector);

            comp.SetParametersAndRender(p => p.Add(x => x.DisableToolbar, true));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_toolbarCssSelector));

            comp.SetParametersAndRender(p => p.Add(x => x.DisableToolbar, false));

            _ = comp.Find(_toolbarCssSelector);
        }

        [Test]
        public void Toogle_ColorField()
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.DisableColorField, false));
            Console.WriteLine(comp.Markup);

            _ = comp.Find(_mudColorPickerCssSelector);

            comp.SetParametersAndRender(p => p.Add(x => x.DisableColorField, true));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_mudColorPickerCssSelector));

            comp.SetParametersAndRender(p => p.Add(x => x.DisableColorField, false));

            _ = comp.Find(_mudColorPickerCssSelector);
        }

        [Test]
        public void Toogle_Preview()
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.DisablePreview, false));
            Console.WriteLine(comp.Markup);

            _ = comp.Find(_colorDotCssSelector);

            comp.SetParametersAndRender(p => p.Add(x => x.DisablePreview, true));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_colorDotCssSelector));

            comp.SetParametersAndRender(p => p.Add(x => x.DisablePreview, false));

            _ = comp.Find(_colorDotCssSelector);
        }

        [Test]
        public void Toogle_Sliders()
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.DisableSliders, false));
            Console.WriteLine(comp.Markup);

            _ = comp.Find(_slidersControlCssSelector);

            comp.SetParametersAndRender(p => p.Add(x => x.DisableSliders, true));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_slidersControlCssSelector));

            comp.SetParametersAndRender(p => p.Add(x => x.DisableSliders, false));

            _ = comp.Find(_slidersControlCssSelector);
        }

        [Test]
        [TestCase(ColorPickerMode.HEX)]
        [TestCase(ColorPickerMode.HSL)]
        [TestCase(ColorPickerMode.RGB)]
        public void Toogle_Input(ColorPickerMode mode)
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ColorPickerMode, mode);
                p.Add(x => x.DisableInput, false);

            });
            Console.WriteLine(comp.Markup);

            _ = comp.Find(_colorInputCssSelector);

            comp.SetParametersAndRender(p => p.Add(x => x.DisableInput, true));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_colorInputCssSelector));

            comp.SetParametersAndRender(p => p.Add(x => x.DisableInput, false));

            _ = comp.Find(_colorInputCssSelector);
        }


        [Test]
        public void Toogle_ModeSwitch()
        {
            var comp = ctx.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.DisableModeSwitch, false));
            Console.WriteLine(comp.Markup);

            _ = comp.Find(_colorInputModeSwitchCssSelector);

            comp.SetParametersAndRender(p => p.Add(x => x.DisableModeSwitch, true));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_colorInputModeSwitchCssSelector));

            comp.SetParametersAndRender(p => p.Add(x => x.DisableModeSwitch, false));

            _ = comp.Find(_colorInputModeSwitchCssSelector);
        }

        [Test]
        [TestCase(ColorPickerMode.HSL)]
        [TestCase(ColorPickerMode.RGB)]
        public void Toogle_Alpha(ColorPickerMode mode)
        {
            var color = new MudColor(12, 220, 124, 120);
            var expectedColor = new MudColor(12, 220, 124, 120);

            var comp = ctx.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ColorPickerMode, mode);
                p.Add(x => x.DisableAlpha, false);
                p.Add(x => x.ColorValue, color);
            });
            Console.WriteLine(comp.Markup);

            _ = comp.Find(_alphaInputCssSelector);

            comp.SetParametersAndRender(p => p.Add(x => x.DisableAlpha, true));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_alphaInputCssSelector));
            comp.Instance.ColorValue.Should().Be(expectedColor);

            comp.SetParametersAndRender(p => p.Add(x => x.DisableAlpha, false));

            _ = comp.Find(_alphaInputCssSelector);
            comp.Instance.ColorValue.Should().Be(expectedColor);
        }

        [Test]
        public void Toogle_Alpha_HexInputMode()
        {
            var color = new MudColor(12, 220, 124, 120);
            var expectedColor = new MudColor(12, 220, 124, 120);

            var comp = ctx.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ColorPickerMode, ColorPickerMode.HEX);
                p.Add(x => x.DisableAlpha, false);
                p.Add(x => x.ColorValue, color);
            });
            Console.WriteLine(comp.Markup);

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_alphaInputCssSelector));

            comp.SetParametersAndRender(p => p.Add(x => x.DisableAlpha, true));

            comp.Instance.ColorValue.Should().Be(expectedColor);

            var inputs = comp.FindAll(".mud-picker-color-inputfield input");
            inputs.Should().ContainSingle();
            inputs.Should().AllBeAssignableTo<IHtmlInputElement>();
            (inputs[0] as IHtmlInputElement).Value.Should().Be("#0cdc7c");

            comp.Instance.TextValue.Should().Be("#0cdc7c");

            comp.SetParametersAndRender(p => p.Add(x => x.DisableAlpha, false));
            comp.Instance.ColorValue.Should().Be(expectedColor);
            inputs = comp.FindAll(".mud-picker-color-inputfield input");
            inputs.Should().ContainSingle();
            inputs.Should().AllBeAssignableTo<IHtmlInputElement>();
            (inputs[0] as IHtmlInputElement).Value.Should().Be("#0cdc7c78");

            comp.Instance.TextValue.Should().Be("#0cdc7c");

        }
    }
}
