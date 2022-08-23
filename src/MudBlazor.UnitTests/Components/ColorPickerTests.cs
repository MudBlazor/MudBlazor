
#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.TestComponents.ColorPicker;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ColorPickerTests : BunitTest
    {
        private const double _defaultXForColorPanel = 209.84;
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
        private const string CssSelector = ".mud-picker-color-overlay-black .mud-picker-color-overlay";
        private const string _mudToolbarButtonsCssSelector = ".mud-toolbar button";

        private static MudColor[] _mudGridDefaultColors = new MudColor[]
                        {
                "#FFFFFF","#ebebeb","#d6d6d6","#c2c2c2","#adadad","#999999","#858586","#707070","#5c5c5c","#474747","#333333","#000000",
                "#133648","#071d53","#0f0638","#2a093b","#370c1b","#541107","#532009","#53350d","#523e0f","#65611b","#505518","#2b3d16",
                "#1e4c63","#0f2e76","#180b4e","#3f1256","#4e1629","#781e0e","#722f10","#734c16","#73591a","#8c8629","#707625","#3f5623",
                "#2e6c8c","#1841a3","#280c72","#591e77","#6f223d","#a62c17","#a0451a","#a06b23","#9f7d28","#c3bc3c","#9da436","#587934",
                "#3c8ab0","#2155ce","#331c8e","#702898","#8d2e4f","#d03a20","#ca5a24","#c8862e","#c99f35","#f3ec4e","#c6d047","#729b44",
                "#479fd3","#2660f5","#4725ab","#8c33b5","#aa395d","#eb512e","#ed732e","#f3ae3d","#f5c944","#fefb67","#ddeb5c","#86b953",
                "#59c4f7","#4e85f6","#5733e2","#af43eb","#d44a7a","#ed6c59","#ef8c56","#f3b757","#f6cd5b","#fef881","#e6ee7a","#a3d16e",
                "#78d3f8","#7fa6f8","#7e52f5","#c45ff6","#de789d","#f09286","#f2a984","#f6c983","#f9da85","#fef9a1","#ebf29b","#badc94",
                "#a5e1fa","#adc5fa","#ab8df7","#d696f8","#e8a7bf","#f4b8b1","#f6c7af","#f9daae","#fae5af","#fefbc0","#f3f7be","#d2e7ba",
                "#d2effd","#d6e1fc","#d6c9fa","#e9cbfb","#f3d4df","#f9dcd9","#fae3d8","#fcecd7","#fdf2d8","#fefce0","#f7fade","#e3edd6"
                        };

        private static MudColor[] _mudGridPaletteDefaultClors = new MudColor[]
                {
                   "#424242", "#2196f3", "#00c853", "#ff9800", "#f44336",
                  "#f6f9fb", "#9df1fa", "#bdffcf", "#fff0a3", "#ffd254",
                  "#e6e9eb", "#27dbf5", "#7ef7a0", "#ffe273", "#ffb31f",
                  "#c9cccf", "#13b8e8", "#14dc71", "#fdd22f", "#ff9102",
                  "#858791", "#0989c2", "#1bbd66", "#ebb323", "#fe6800",
                  "#585b62", "#17698e", "#17a258", "#d9980d", "#dc3f11",
                  "#353940", "#113b53", "#127942", "#bf7d11", "#aa0000"
                };

        private MockEventListener _eventListener;

        public override void Setup()
        {
            _eventListener = new MockEventListener();
            base.Setup();
            Context.Services.AddSingleton<IEventListenerFactory>(new MockEventListenerFactory(_eventListener));
        }

        private void CheckColorRelatedValues(IRenderedComponent<SimpleColorPickerTest> comp, double expectedX, double expectedY, MudColor expectedColor, ColorPickerMode mode, bool checkInstanceValue = true, bool isRtl = false)
        {
            if (checkInstanceValue == true)
            {
                comp.Instance.ColorValue.Should().Be(expectedColor);
            }

            if (mode is ColorPickerMode.RGB or ColorPickerMode.HSL)
            {
                var castedInputs = GetColorInputs(comp);

                if (mode == ColorPickerMode.RGB)
                {
                    castedInputs[0].Value.Should().Be(expectedColor.R.ToString());
                    castedInputs[1].Value.Should().Be(expectedColor.G.ToString());
                    castedInputs[2].Value.Should().Be(expectedColor.B.ToString());
                    castedInputs[3].Value.Should().Be(expectedColor.APercentage.ToString(CultureInfo.CurrentUICulture));
                }
                else
                {
                    castedInputs[0].Value.Should().Be(expectedColor.H.ToString(CultureInfo.CurrentUICulture));
                    castedInputs[1].Value.Should().Be(expectedColor.S.ToString(CultureInfo.CurrentUICulture));
                    castedInputs[2].Value.Should().Be(expectedColor.L.ToString(CultureInfo.CurrentUICulture));
                    castedInputs[3].Value.Should().Match(x => double.Parse(x, CultureInfo.CurrentUICulture) == Math.Round((expectedColor.A / 255.0), 2));
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

            ((IHtmlInputElement)hueSlideValue[0]).Value.Should().Be(((int)expectedColor.H).ToString());

            var alphaSlider = comp.FindAll(_alphaSliderCssSelector);
            alphaSlider.Should().ContainSingle();
            alphaSlider[0].Should().BeAssignableTo<IHtmlInputElement>();

            ((IHtmlInputElement)alphaSlider[0]).Value.Should().Be(((int)expectedColor.A).ToString());

            var alphaSliderStyleAttritbute = ((IHtmlElement)alphaSlider[0].Parent.Parent).GetAttribute("style");

            if (isRtl == false)
            {
                alphaSliderStyleAttritbute.Should().Be($"background-image: linear-gradient(to right, transparent, {expectedColor.ToString(MudColorOutputFormats.RGB)});");
            }
            else
            {
                alphaSliderStyleAttritbute.Should().Be($"background-image: linear-gradient(to left, transparent, {expectedColor.ToString(MudColorOutputFormats.RGB)});");
            }
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
        public void ColorPickerOpenButtonAriaLabel()
        {
            var comp = Context.RenderComponent<MudColorPicker>();
            var openButton = comp.Find(".mud-input-adornment button");
            openButton.Attributes.GetNamedItem("aria-label")?.Value.Should().Be("Open Color Picker");
        }
        
        [Test]
        public async Task Default()
        {
            var comp = Context.RenderComponent<MudColorPicker>();

            comp.Instance.DisableAlpha.Should().BeFalse();
            comp.Instance.DisableColorField.Should().BeFalse();
            comp.Instance.DisableModeSwitch.Should().BeFalse();
            comp.Instance.DisableInputs.Should().BeFalse();
            comp.Instance.DisableSliders.Should().BeFalse();
            comp.Instance.DisablePreview.Should().BeFalse();
            comp.Instance.ColorPickerMode.Should().Be(ColorPickerMode.RGB);
            comp.Instance.ColorPickerView.Should().Be(ColorPickerView.Spectrum);
            comp.Instance.UpdateBindingIfOnlyHSLChanged.Should().BeFalse();
            comp.Instance.Value.Should().Be(_defaultColor);
            comp.Instance.Palette.Should().BeEquivalentTo(_mudGridPaletteDefaultClors);
            comp.Instance.DisableDragEffect.Should().BeFalse();
        }

        [Test]
        [TestCase(40, 256.78, _defaultYForColorPanel)]
        public void SetR(byte r, double selectorXPosition, double selectorYPosition)
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>();
            //Console.WriteLine(comp.Markup);

            var rInput = GetColorInput(comp, 0);

            var expectedColor = comp.Instance.ColorValue.SetR(r);
            rInput.Change(expectedColor.R.ToString());

            CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.RGB);
        }

        [Test]
        [TestCase(240, 196.3, 14.71)]
        public void SetG(byte g, double selectorXPosition, double selectorYPosition)
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>();
            //Console.WriteLine(comp.Markup);

            var gInput = GetColorInput(comp, 1);

            var expectedColor = comp.Instance.ColorValue.SetG(g);

            gInput.Change(expectedColor.G.ToString());

            CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.RGB);
        }

        [Test]
        [TestCase(90, 55.47, 161.76)]
        public void SetB(byte b, double selectorXPosition, double selectorYPosition)
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>();
            //Console.WriteLine(comp.Markup);

            var bInput = GetColorInput(comp, 2);

            var expectedColor = comp.Instance.ColorValue.SetB(b);

            bInput.Change(expectedColor.B.ToString());

            CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.RGB);
        }

        [Test]
        [TestCase(0.9, _defaultXForColorPanel, _defaultYForColorPanel)]
        public void SetA_InRGBMode(double a, double selectorXPosition, double selectorYPosition)
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>();
            //Console.WriteLine(comp.Markup);

            var aInput = GetColorInput(comp, 3);

            var expectedColor = comp.Instance.ColorValue.SetAlpha(a);

            aInput.Change(a.ToString(CultureInfo.CurrentUICulture));

            CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.RGB);
        }

        [Test]
        [TestCase(90, 208.46, _defaultYForColorPanel)]
        public void SetH(int h, double selectorXPosition, double selectorYPosition)
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.ColorPickerMode, ColorPickerMode.HSL));
            //Console.WriteLine(comp.Markup);

            var hInput = GetColorInput(comp, 0);

            var expectedColor = comp.Instance.ColorValue.SetH(h);

            hInput.Change(expectedColor.H.ToString());

            CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.HSL);
        }

        [Test]
        [TestCase(0.4, 134.88, 61.76)]
        public void SetS(double s, double selectorXPosition, double selectorYPosition)
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.ColorPickerMode, ColorPickerMode.HSL));
            //Console.WriteLine(comp.Markup);

            var sColor = GetColorInput(comp, 1);

            var expectedColor = comp.Instance.ColorValue.SetS(s);

            sColor.Change(expectedColor.S.ToString(CultureInfo.CurrentUICulture));

            CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.HSL);
        }

        [Test]
        [TestCase(0.67, 163.43, 23.53)]
        public void SetL(double l, double selectorXPosition, double selectorYPosition)
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.ColorPickerMode, ColorPickerMode.HSL));
            //Console.WriteLine(comp.Markup);

            var lColor = GetColorInput(comp, 2);

            var expectedColor = comp.Instance.ColorValue.SetL(l);
            lColor.Change(expectedColor.L.ToString(CultureInfo.CurrentUICulture));

            CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.HSL);
        }

        [Test]
        [TestCase(0.5, _defaultXForColorPanel, _defaultYForColorPanel)]
        public void SetAlpha_AsHLS(double a, double selectorXPosition, double selectorYPosition)
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.ColorPickerMode, ColorPickerMode.HSL));
            //Console.WriteLine(comp.Markup);

            var lColor = GetColorInput(comp, 3);

            var expectedColor = comp.Instance.ColorValue.SetAlpha(a);
            lColor.Change(a.ToString(CultureInfo.CurrentUICulture));

            CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.HSL);
        }

        [Test]
        [TestCase("#8cb829ff", 242.48, 69.61)]
        public void SetColorInput(string colorHexString, double selectorXPosition, double selectorYPosition)
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.ColorPickerMode, ColorPickerMode.HEX));
            //Console.WriteLine(comp.Markup);

            var inputs = comp.FindAll(".mud-picker-color-inputs input");

            var lColor = GetColorInput(comp, 0, 1);

            var expectedColor = colorHexString;
            lColor.Change(colorHexString);

            CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.HEX);
        }

        [Test]
        [TestCase("#8qb829ff")]
        public void SetColorInput_InvailidNoChange(string colorHexString)
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.ColorPickerMode, ColorPickerMode.HEX));
            //Console.WriteLine(comp.Markup);

            var inputs = comp.FindAll(".mud-picker-color-inputs input");

            var hexInput = GetColorInput(comp, 0, 1);

            var expectedColor = _defaultColor;
            hexInput.Change(colorHexString);

            CheckColorRelatedValues(comp, _defaultXForColorPanel, _defaultYForColorPanel, expectedColor, ColorPickerMode.HEX);
        }

        [Test]
        public void SetAlphaSlider()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>();
            //Console.WriteLine(comp.Markup);

            for (var i = 256 - 1; i >= 0; i--)
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
            var comp = Context.RenderComponent<SimpleColorPickerTest>();
            //Console.WriteLine(comp.Markup);

            var args = new MouseEventArgs
            {
                OffsetX = 117.0,
                OffsetY = 140.0,
                Buttons = 1,
            };

            _eventListener.FireEvent(args);

            var expectedColor = new MudColor(74, 70, 112, 255);

            CheckColorRelatedValues(comp, args.OffsetX, args.OffsetY, expectedColor, ColorPickerMode.RGB);

            var argsWithoutLeftButtonPushed = new MouseEventArgs
            {
                OffsetX = 117.0,
                OffsetY = 140.0,
                Buttons = 0,
            };

            _eventListener.FireEvent(argsWithoutLeftButtonPushed);
            CheckColorRelatedValues(comp, args.OffsetX, args.OffsetY, expectedColor, ColorPickerMode.RGB);
        }

        [Test]
        [TestCase(PickerVariant.Dialog)]
        [TestCase(PickerVariant.Inline)]
        public async Task MouseMove_InDialogMode(PickerVariant variant)
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p => p.Add(y =>
            y.Variant, variant));

            //Console.WriteLine(comp.Markup);

            var args = new MouseEventArgs
            {
                OffsetX = 117.0,
                OffsetY = 140.0,
                Buttons = 1,
            };

            _eventListener.Callbacks.Values.Should().BeEmpty();

            await comp.Instance.OpenPicker();
            _eventListener.Callbacks.Values.Should().ContainSingle();

            _eventListener.FireEvent(args);

            var expectedColor = new MudColor(74, 70, 112, 255);

            CheckColorRelatedValues(comp, args.OffsetX, args.OffsetY, expectedColor, ColorPickerMode.RGB);

            var argsWithoutLeftButtonPushed = new MouseEventArgs
            {
                OffsetX = 117.0,
                OffsetY = 140.0,
                Buttons = 0,
            };

            _eventListener.FireEvent(argsWithoutLeftButtonPushed);
            CheckColorRelatedValues(comp, args.OffsetX, args.OffsetY, expectedColor, ColorPickerMode.RGB);

            await comp.Instance.ClosePicker();

            _eventListener.Callbacks.Should().BeEmpty();
        }

        [Test]
        public void SetHueSlider()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>();
            //Console.WriteLine(comp.Markup);

            for (var i = 0; i <= 360; i++)
            {
                var expectedColor = comp.Instance.ColorValue.SetH(i);

                var hueColorSlider = comp.FindAll(_hueSliderCssSelector);
                hueColorSlider.Should().ContainSingle();
                hueColorSlider[0].Should().BeAssignableTo<IHtmlInputElement>();

                InputEventDispatchExtensions.Input(hueColorSlider[0], i.ToString());

                CheckColorRelatedValues(comp, 208.46, _defaultYForColorPanel, expectedColor, ColorPickerMode.RGB);
            }
        }

        [Test]
        public void Click_ColorPanel()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>();
            //Console.WriteLine(comp.Markup);

            var overlay = comp.Find(CssSelector);

            var x = 99.2;
            var y = 200.98;

            overlay.Click(new MouseEventArgs { OffsetX = x, OffsetY = y });

            MudColor color = "#232232ff";
            MudColor expectedColor = new MudColor(color.R, color.G, color.B, _defaultColor);

            CheckColorRelatedValues(comp, x, y, expectedColor, ColorPickerMode.RGB);
        }

        [Test]
        public void Click_ModeBtton()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>();
            //Console.WriteLine(comp.Markup);

            var color = comp.Instance.ColorValue;
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
            var comp = Context.RenderComponent<SimpleColorPickerTest>();
            //Console.WriteLine(comp.Markup);

            var colorDot = comp.Find(_colorDotCssSelector);
            // no collection
            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-picker-color-collection"));
            colorDot.Click();

            // collection found
            var colorsToSelectPanel = comp.Find(".mud-picker-color-collection");
            colorsToSelectPanel.Children.Should().HaveCount(5);

            var expectedColors = new MudColor[] { "#ff4081ff", "#2196f3ff", "#00c853ff", "#ff9800ff", "#f44336ff" };

            for (var i = 0; i < 5; i++)
            {
                var styleAttribute = colorsToSelectPanel.Children[i].GetAttribute("style");
                styleAttribute.Should().Be($"background: {expectedColors[i].ToString(MudColorOutputFormats.RGBA)};");
            }

            colorDot.Click();

            // again no collection visible
            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-picker-color-collection"));
        }

        [Test]
        public void ColorPalette_CustomColors()
        {
            var expectedColors = new MudColor[] { "#23af3daa", "#56a23dff", "#56a85dff" };

            var comp = Context.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.Palette, expectedColors));
            //Console.WriteLine(comp.Markup);

            var colorDot = comp.Find(_colorDotCssSelector);
            colorDot.Click();

            // collection found
            var colorsToSelectPanel = comp.Find(".mud-picker-color-collection");
            colorsToSelectPanel.Children.Should().HaveCount(expectedColors.Length);

            for (var i = 0; i < expectedColors.Length; i++)
            {
                var styleAttribute = colorsToSelectPanel.Children[i].GetAttribute("style");
                styleAttribute.Should().Be($"background: {expectedColors[i].ToString(MudColorOutputFormats.RGBA)};");
            }
        }

        [Test]
        public void ColorPalette_SelectColor()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>();
            //Console.WriteLine(comp.Markup);

            var colorDot = comp.Find(_colorDotCssSelector);
            // no collection
            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-picker-color-collection"));
            colorDot.Click();

            var expectedColors = new MudColor[] { "#ff4081ff", "#2196f3ff", "#00c853ff", "#ff9800ff", "#f44336ff" };

            for (var i = 0; i < 5; i++)
            {
                var colorsToSelectPanel = comp.Find(".mud-picker-color-collection");

                colorsToSelectPanel.Children[i].Click();
                colorDot = comp.Find(_colorDotCssSelector);

                var styleAttribute = colorDot.GetAttribute("style");
                styleAttribute.Should().Be($"background: {expectedColors[i].ToString(MudColorOutputFormats.RGBA)};");
                comp.Instance.ColorValue.Should().Be(expectedColors[i]);

                colorDot.Click();
            }
        }

        [Test]
        public void Toogle_Toolbar()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.DisableToolbar, false));
            //Console.WriteLine(comp.Markup);

            _ = comp.Find(_toolbarCssSelector);

            comp.SetParametersAndRender(p => p.Add(x => x.DisableToolbar, true));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_toolbarCssSelector));

            comp.SetParametersAndRender(p => p.Add(x => x.DisableToolbar, false));

            _ = comp.Find(_toolbarCssSelector);
        }

        [Test]
        public void Toggle_ColorField()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.DisableColorField, false));
            //Console.WriteLine(comp.Markup);

            _ = comp.Find(_mudColorPickerCssSelector);

            comp.SetParametersAndRender(p => p.Add(x => x.DisableColorField, true));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_mudColorPickerCssSelector));

            comp.SetParametersAndRender(p => p.Add(x => x.DisableColorField, false));

            _ = comp.Find(_mudColorPickerCssSelector);
        }

        [Test]
        public void Toggle_Preview()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.DisablePreview, false));
            //Console.WriteLine(comp.Markup);

            _ = comp.Find(_colorDotCssSelector);

            comp.SetParametersAndRender(p => p.Add(x => x.DisablePreview, true));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_colorDotCssSelector));

            comp.SetParametersAndRender(p => p.Add(x => x.DisablePreview, false));

            _ = comp.Find(_colorDotCssSelector);
        }

        [Test]
        public void Toggle_Sliders()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.DisableSliders, false));
            //Console.WriteLine(comp.Markup);

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
        public void Toggle_Input(ColorPickerMode mode)
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ColorPickerMode, mode);
                p.Add(x => x.DisableInput, false);

            });
            //Console.WriteLine(comp.Markup);

            _ = comp.Find(_colorInputCssSelector);

            comp.SetParametersAndRender(p => p.Add(x => x.DisableInput, true));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_colorInputCssSelector));

            comp.SetParametersAndRender(p => p.Add(x => x.DisableInput, false));

            _ = comp.Find(_colorInputCssSelector);
        }


        [Test]
        public void Toggle_ModeSwitch()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p => p.Add(x => x.DisableModeSwitch, false));
            //Console.WriteLine(comp.Markup);

            _ = comp.Find(_colorInputModeSwitchCssSelector);

            comp.SetParametersAndRender(p => p.Add(x => x.DisableModeSwitch, true));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_colorInputModeSwitchCssSelector));

            comp.SetParametersAndRender(p => p.Add(x => x.DisableModeSwitch, false));

            _ = comp.Find(_colorInputModeSwitchCssSelector);
        }

        [Test]
        [TestCase(ColorPickerMode.HSL)]
        [TestCase(ColorPickerMode.RGB)]
        public void Toggle_Alpha(ColorPickerMode mode)
        {
            var color = new MudColor(12, 220, 124, 120);
            var expectedColor = new MudColor(12, 220, 124, 120);

            var comp = Context.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ColorPickerMode, mode);
                p.Add(x => x.DisableAlpha, false);
                p.Add(x => x.ColorValue, color);
            });
            //Console.WriteLine(comp.Markup);

            _ = comp.Find(_alphaInputCssSelector);

            comp.SetParametersAndRender(p => p.Add(x => x.DisableAlpha, true));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_alphaInputCssSelector));
            comp.Instance.ColorValue.Should().Be(expectedColor);
            comp.Instance.TextValue.Should().Be(expectedColor.ToString(MudColorOutputFormats.Hex));

            comp.SetParametersAndRender(p => p.Add(x => x.DisableAlpha, false));

            _ = comp.Find(_alphaInputCssSelector);
            comp.Instance.ColorValue.Should().Be(expectedColor);
            comp.Instance.TextValue.Should().Be(expectedColor.ToString(MudColorOutputFormats.HexA));

        }

        [Test]
        public void Toggle_Alpha_HexInputMode()
        {
            var color = new MudColor(12, 220, 124, 120);
            var expectedColor = new MudColor(12, 220, 124, 120);

            var comp = Context.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ColorPickerMode, ColorPickerMode.HEX);
                p.Add(x => x.DisableAlpha, false);
                p.Add(x => x.ColorValue, color);
            });
            //Console.WriteLine(comp.Markup);

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_alphaInputCssSelector));

            comp.SetParametersAndRender(p => p.Add(x => x.DisableAlpha, true));

            comp.Instance.ColorValue.Should().Be(expectedColor);

            var inputs = comp.FindAll(".mud-picker-color-inputfield input");
            inputs.Should().ContainSingle();
            inputs.Should().AllBeAssignableTo<IHtmlInputElement>();
            ((IHtmlInputElement)inputs[0]).Value.Should().Be("#0cdc7c");

            comp.Instance.TextValue.Should().Be("#0cdc7c");

            comp.SetParametersAndRender(p => p.Add(x => x.DisableAlpha, false));
            comp.Instance.ColorValue.Should().Be(expectedColor);
            inputs = comp.FindAll(".mud-picker-color-inputfield input");
            inputs.Should().ContainSingle();
            inputs.Should().AllBeAssignableTo<IHtmlInputElement>();
            ((IHtmlInputElement)inputs[0]).Value.Should().Be("#0cdc7c78");

            comp.Instance.TextValue.Should().Be("#0cdc7c78");
        }

        [Test]
        public void ToggleViewMode()
        {
            var comp = Context.RenderComponent<MudColorPicker>(p =>
            {
                p.Add(x => x.DisableToolbar, false);
                p.Add(x => x.PickerVariant, PickerVariant.Static);
            });

            //Console.WriteLine(comp.Markup);

            var buttons = comp.FindAll(_mudToolbarButtonsCssSelector);

            Dictionary<int, (ColorPickerView, string)> buttonMapper = new()
            {
                { 2, (ColorPickerView.Palette, ".mud-picker-color-view-collection") },
                { 1, (ColorPickerView.Grid, ".mud-picker-color-grid") },
                { 0, (ColorPickerView.Spectrum, ".mud-picker-color-overlay") },
            };

            foreach (var item in buttonMapper)
            {
                buttons[item.Key].Click();

                _ = comp.Find(item.Value.Item2);
            }
        }

        [Test]
        [TestCase(PickerVariant.Static, false)]
        [TestCase(PickerVariant.Inline, true)]
        [TestCase(PickerVariant.Dialog, true)]
        public async Task CloseButtonInToolbarVisible(PickerVariant variant, bool expectedVisibility)
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.Variant, variant);
                p.Add(x => x.DisableToolbar, false);
            });

            //Console.WriteLine(comp.Markup);

            if (variant is PickerVariant.Dialog or PickerVariant.Inline)
            {
                await comp.Instance.OpenPicker();
            }

            if (expectedVisibility == false)
            {
                Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-close-picker-button"));
            }
            else
            {
                _ = comp.Find(".mud-close-picker-button");
            }
        }

        [Test]
        [TestCase(ColorPickerView.Spectrum, 0)]
        [TestCase(ColorPickerView.Grid, 1)]
        [TestCase(ColorPickerView.Palette, 2)]
        public void ColorPickerView_Selection(ColorPickerView view, int index)
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, view);
                p.Add(x => x.DisableToolbar, false);
            });

            //Console.WriteLine(comp.Markup);

            var toolbarButtons = comp.FindAll(".mud-toolbar .mud-icon-button button");
            for (var i = 0; i < toolbarButtons.Count; i++)
            {
                if (i == index)
                {
                    toolbarButtons[i].ClassList.Should().Contain(".mud-icon-button-color-primary");
                }
                else
                {
                    toolbarButtons[i].ClassList.Should().NotContain(".mud-icon-button-color-primary");
                }
            }
        }

        [Test]
        public void PaletteView()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, ColorPickerView.Palette);
                p.Add(x => x.Palette, _mudGridPaletteDefaultClors);
            });

            //Console.WriteLine(comp.Markup);

            var expectedColors = _mudGridPaletteDefaultClors;

            var collectionView = comp.Find(".mud-picker-color-view-collection");

            collectionView.Children.Should().HaveCount(expectedColors.Length);

            for (var i = 0; i < collectionView.Children.Length; i++)
            {
                var expectedColor = expectedColors[i];
                var colorElement = collectionView.Children[i];
                colorElement.ClassList.Should().Contain("mud-picker-color-dot");

                var style = colorElement.GetAttribute("style");
                style.Should().Be($"background: {expectedColor.ToString(MudColorOutputFormats.RGBA)};");
            }
        }

        [Test]
        public void PaletteView_ChooseColor()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, ColorPickerView.Palette);
                p.Add(x => x.Palette, _mudGridPaletteDefaultClors);
            });

            //Console.WriteLine(comp.Markup);

            var expectedColors = _mudGridPaletteDefaultClors;
            var collectionView = comp.Find(".mud-picker-color-view-collection");

            for (var i = 0; i < collectionView.Children.Length; i++)
            {
                var expectedColor = expectedColors[i];
                var colorElement = collectionView.Children[i];
                colorElement.Click();

                comp.Instance.ColorValue.Should().Be(expectedColor);
                comp.Find(".mud-picker-color-view-collection").Children[i].ClassList.Should().BeEquivalentTo("mud-picker-color-dot", "selected");

            }
        }

        [Test]
        public void GridView()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, ColorPickerView.Grid);
            });

            //Console.WriteLine(comp.Markup);

            var expectedColors = _mudGridDefaultColors;

            var collectionView = comp.Find(".mud-picker-color-grid");

            collectionView.Children.Should().HaveCount(expectedColors.Length);

            for (var i = 0; i < collectionView.Children.Length; i++)
            {
                var expectedColor = expectedColors[i];
                var colorElement = collectionView.Children[i];
                colorElement.ClassList.Should().Contain("mud-picker-color-dot");

                var style = colorElement.GetAttribute("style");
                style.Should().Be($"background: {expectedColor.ToString(MudColorOutputFormats.RGBA)};");
            }
        }

        [Test]
        public void GridView_ChooseColor()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, ColorPickerView.Grid);
            });

            //Console.WriteLine(comp.Markup);

            var expectedColors = _mudGridDefaultColors;

            var collectionView = comp.Find(".mud-picker-color-grid");

            for (var i = 0; i < collectionView.Children.Length; i++)
            {
                var expectedColor = expectedColors[i];
                var colorElement = collectionView.Children[i];
                colorElement.ClassList.Should().Contain("mud-picker-color-dot");

                colorElement.Click();
                comp.Instance.ColorValue.Should().Be(expectedColor);

                comp.Find(".mud-picker-color-grid").Children[i].ClassList.Should().BeEquivalentTo("mud-picker-color-dot", "selected");
            }
        }

        [Test]
        public void GridCompactView()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, ColorPickerView.GridCompact);
            });

            //Console.WriteLine(comp.Markup);

            var expectedColors = _mudGridDefaultColors;

            var content = comp.Find(".mud-picker-content.mud-picker-color-content");
            content.Children.Should().ContainSingle();

            var gridView = comp.Find(".mud-picker-color-grid");

            gridView.Children.Should().HaveCount(expectedColors.Length);

            for (var i = 0; i < gridView.Children.Length; i++)
            {
                var expectedColor = expectedColors[i];
                var colorElement = gridView.Children[i];
                colorElement.ClassList.Should().Contain("mud-picker-color-dot");

                var style = colorElement.GetAttribute("style");
                style.Should().Be($"background: {expectedColor.ToString(MudColorOutputFormats.RGBA)};");
            }
        }

        [Test]
        public void GridCompactView_ChooseColor()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, ColorPickerView.GridCompact);
            });

            //Console.WriteLine(comp.Markup);

            var expectedColors = _mudGridDefaultColors;

            var gridView = comp.Find(".mud-picker-color-grid");

            gridView.Children.Should().HaveCount(expectedColors.Length);

            for (var i = 0; i < gridView.Children.Length; i++)
            {
                var expectedColor = expectedColors[i];
                var colorElement = gridView.Children[i];
                colorElement.ClassList.Should().Contain("mud-picker-color-dot");

                colorElement.Click();
                comp.Instance.ColorValue.Should().Be(expectedColor);

                comp.Find(".mud-picker-color-grid").Children[i].ClassList.Should().BeEquivalentTo("mud-picker-color-dot", "selected");
            }
        }

        [Test]
        [TestCase(PickerVariant.Inline)]
        [TestCase(PickerVariant.Dialog)]
        public async Task GridCompact_CloseOnSelect(PickerVariant variant)
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, ColorPickerView.GridCompact);
                p.Add(x => x.Variant, variant);
            });

            //Console.WriteLine(comp.Markup);

            await comp.Instance.OpenPicker();

            var expectedColors = _mudGridDefaultColors;

            var gridView = comp.Find(".mud-picker-color-grid");
            gridView.Children[0].Click();

            comp.Instance.ColorValue.Should().Be(expectedColors[0]);

            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-picker-container"));
        }

        [Test]
        [TestCase(PickerVariant.Inline)]
        [TestCase(PickerVariant.Dialog)]
        public async Task Palette_CloseOnSelect(PickerVariant variant)
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, ColorPickerView.Palette);
                p.Add(x => x.Variant, variant);
                p.Add(x => x.Palette, _mudGridPaletteDefaultClors);
            });

            await comp.Instance.OpenPicker();

            //Console.WriteLine(comp.Markup);

            var expectedColors = _mudGridPaletteDefaultClors;

            var gridView = comp.Find(".mud-picker-color-view-collection");
            gridView.Children[0].Click();

            comp.Instance.ColorValue.Should().Be(expectedColors[0]);

            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-picker-container"));
        }

        [Test]
        [TestCase(PickerVariant.Inline, ColorPickerView.Grid)]
        [TestCase(PickerVariant.Dialog, ColorPickerView.Grid)]
        [TestCase(PickerVariant.Inline, ColorPickerView.Spectrum)]
        [TestCase(PickerVariant.Dialog, ColorPickerView.Spectrum)]
        public async Task NoControls_CloseOnSelect(PickerVariant variant, ColorPickerView view)
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, view);
                p.Add(x => x.Variant, variant);
                p.Add(x => x.DisablePreview, true);
                p.Add(x => x.DisableSliders, true);
                p.Add(x => x.DisableInput, true);
            });

            await comp.Instance.OpenPicker();

            //Console.WriteLine(comp.Markup);

            var expectedColors = view == ColorPickerView.Grid ? _mudGridDefaultColors : _mudGridPaletteDefaultClors;

            IElement item;
            if (view == ColorPickerView.Grid)
            {
                item = comp.Find(".mud-picker-color-grid").Children[0];
            }
            else
            {
                item = comp.Find("[class=\"mud-picker-color-overlay\"][id]");
            }

            item.Click();


            comp.Instance.ColorValue.Should().NotBe(_defaultColor);

            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-picker-container"));
        }

        [Test]
        [TestCase(ColorPickerView.Spectrum, false, "#78797aff")]
        [TestCase(ColorPickerView.Grid, false, "#78797aff")]
        [TestCase(ColorPickerView.Palette, false, "#78797a")]
        [TestCase(ColorPickerView.GridCompact, false, "#78797a")]
        [TestCase(ColorPickerView.Spectrum, true, "#78797a")]
        [TestCase(ColorPickerView.Grid, true, "#78797a")]
        [TestCase(ColorPickerView.Palette, true, "#78797a")]
        [TestCase(ColorPickerView.GridCompact, true, "#78797a")]
        public void TextOutput_Alpha(ColorPickerView view, bool disableAlpha, string expectedOutput)
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, view);
                p.Add(x => x.DisableAlpha, disableAlpha);
                p.Add(x => x.ColorValue, new MudColor(120, 121, 122, 1.0));
            });

            comp.Instance.TextValue.Should().Be(expectedOutput);
        }

        [Test]
        public void SetNullColor_NothingChanged()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ColorPickerMode, ColorPickerMode.HSL);
            });

            //Console.WriteLine(comp.Markup);

            comp.SetParametersAndRender(p => p.Add(x => x.ColorValue, null));

            var lColor = GetColorInput(comp, 2);
            var expectedColor = _defaultColor;

            CheckColorRelatedValues(comp, _defaultXForColorPanel, _defaultYForColorPanel, expectedColor, ColorPickerMode.HSL, false);
            comp.FindComponent<MudColorPicker>().Instance.Value.Should().Be(_defaultColor);
        }

        [Test]
        public void SetHLS_NotChangeRBG_ButCallbackFired()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ColorPickerMode, ColorPickerMode.HSL);
                p.Add(x => x.AlwaysUpdateBinding, true);
                p.Add(x => x.ColorValue, new MudColor(245, 0.35, 0.95, 1.0));
            });

            //Console.WriteLine(comp.Markup);

            var sColor = GetColorInput(comp, 1);

            var colorValue = comp.Instance.ColorValue.ToString(MudColorOutputFormats.HexA);
            CheckColorRelatedValues(comp, 11.37, 7.84, comp.Instance.ColorValue, ColorPickerMode.HSL);

            var expectedColor = comp.Instance.ColorValue.SetS(comp.Instance.ColorValue.S - 0.01);
            sColor.Change(expectedColor.S.ToString(CultureInfo.CurrentUICulture));

            CheckColorRelatedValues(comp, 11.37, 7.84, expectedColor, ColorPickerMode.HSL);
            var colorValueAfterChange = comp.Instance.ColorValue.ToString(MudColorOutputFormats.HexA);

            colorValue.Should().Be(colorValueAfterChange);
            comp.Instance.ValueChangeCallbackCounter.Should().Be(1);
        }

        [Test]
        public void RTL_AlphaSliderInverseStyle()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.AddCascadingValue(true);
            });

            //Console.WriteLine(comp.Markup);

            CheckColorRelatedValues(comp, _defaultXForColorPanel, _defaultYForColorPanel, comp.Instance.ColorValue, ColorPickerMode.RGB, true, true);
        }

        [Test]
        public void AddAndRemoveEventListenerWhenChangingColorPickerView()
        {
            var comp = Context.RenderComponent<MudColorPicker>(p =>
            {
                p.Add(x => x.DisableToolbar, false);
                p.Add(x => x.PickerVariant, PickerVariant.Static);
                p.Add(x => x.ColorPickerView, ColorPickerView.Spectrum);
            });

            var buttons = comp.FindAll(_mudToolbarButtonsCssSelector);

            _eventListener.ElementIdMapper.Keys.Should().ContainSingle();
            var value = _eventListener.ElementIdMapper.Values.First();

            buttons[2].Click();
            _eventListener.ElementIdMapper.Keys.Should().BeEmpty();

            buttons[1].Click();
            _eventListener.ElementIdMapper.Keys.Should().BeEmpty();

            buttons[0].Click();

            _eventListener.ElementIdMapper.Keys.Should().ContainSingle();
            _eventListener.ElementIdMapper.Values.First().Should().Be(value);
        }

        [Test]
        public void EventListenerNotAttachedWhenEnableDragEffectIsDisabled()
        {
            var comp = Context.RenderComponent<MudColorPicker>(p =>
            {
                p.Add(x => x.DisableToolbar, false);
                p.Add(x => x.PickerVariant, PickerVariant.Static);
                p.Add(x => x.ColorPickerView, ColorPickerView.Spectrum);
                p.Add(x => x.DisableDragEffect, true);
            });

            var buttons = comp.FindAll(_mudToolbarButtonsCssSelector);

            _eventListener.ElementIdMapper.Keys.Should().BeEmpty();

            for (var i = 3 - 1; i >= 0; i--)
            {
                buttons[i].Click();
                _eventListener.ElementIdMapper.Keys.Should().BeEmpty();
            }
        }

        [Test]
        public void StableHue_WhenColorSpectrumClicked()
        {
            var comp = Context.RenderComponent<MudColorPicker>(p =>
            {
                p.Add(x => x.PickerVariant, PickerVariant.Static);
                p.Add(x => x.ColorPickerView, ColorPickerView.Spectrum);
            });

            //Console.WriteLine(comp.Markup);

            var overlay = comp.Find(CssSelector);

            double expectedHue = _defaultColor.H;

            for (int x = 0; x < 312; x += 5)
            {
                for (int y = 0; y < 250; y += 5)
                {
                    overlay.Click(new MouseEventArgs { OffsetX = x, OffsetY = y });

                    comp.Instance.Value.H.Should().Be(expectedHue);
                }
            }
        }

        [Test]
        public void Click_Selector_ColorPanel()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>();
            //Console.WriteLine(comp.Markup);

            var overlay = comp.Find(CssSelector);

            var x = 99.2;
            var y = 200.98;

            overlay.Click(new MouseEventArgs { OffsetX = x, OffsetY = y });

            MudColor color = "#232232ff";
            MudColor expectedColor = new MudColor(color.R, color.G, color.B, _defaultColor);

            CheckColorRelatedValues(comp, x, y, expectedColor, ColorPickerMode.RGB);

            var selector = comp.Find(".mud-picker-color-selector");

            //a click in the center of the selector shouldn't change something
            selector.Click(new MouseEventArgs { OffsetX = 13, OffsetY = 13 });

            CheckColorRelatedValues(comp, x, y, expectedColor, ColorPickerMode.RGB);

            selector.Click(new MouseEventArgs { OffsetX = 5, OffsetY = 20 });

            MudColor secondExpectedColor = new MudColor(31, 30, 42, _defaultColor);
            CheckColorRelatedValues(comp, x - 8, y + 7, secondExpectedColor, ColorPickerMode.RGB);

        }

        [Test]
        public async Task CheckPickerPopover()
        {
            var comp = Context.RenderComponent<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.Variant, PickerVariant.Inline);
            });
            
            await comp.Instance.OpenPicker();

            //Console.WriteLine(comp.Markup);

            var providerNode =  comp.Find(".mud-popover-provider");
            providerNode.Children.Should().ContainSingle();

            var popoverNode = providerNode.Children[0];

            popoverNode.ClassList.Should().BeEquivalentTo(new[]
            {
                "mud-popover",
                "mud-popover-fixed",
                "mud-popover-open",
                "mud-popover-top-left",
                "mud-popover-anchor-top-left",
                "mud-popover-overflow-flip-onopen",
            });
        }

        //https://github.com/MudBlazor/MudBlazor/issues/4899
        [Test]
        public void DistingishBetweenInternalAndExternalView()
        {
            var comp = Context.RenderComponent<PickerWithFixedView>();

            //open the color picker
            var inputField = comp.Find(".mud-input-slot");
            inputField.Click();

            //asset that the picker is open in grid mode
            var grid = comp.Find(".mud-picker-color-grid");

            //find spectrum button and click
            var spectrumButton = comp.FindAll(_mudToolbarButtonsCssSelector)[1];
            spectrumButton.Click();

            //find the overlay and click any position
            var overlay = comp.Find(CssSelector);
            overlay.Click(new MouseEventArgs { OffsetX = 10.5, OffsetY = 10.5 });

            //ensure that the spectrum mode is still open and not the color grid
            _ = comp.Find(".mud-picker-color-overlay");
            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-picker-color-grid"));

            //change the view per paramter
            comp.SetParametersAndRender(x => x.Add(y => y.ColorPickerView, ColorPickerView.GridCompact));

            //now the grid view should be visible
            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-picker-color-overlay"));
            _ = comp.Find(".mud-picker-color-grid");
        }
    }
}
