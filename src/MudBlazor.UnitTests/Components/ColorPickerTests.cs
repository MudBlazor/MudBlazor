#pragma warning disable IDE1006 // leading underscore

using System.Globalization;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Extensions;
using MudBlazor.Resources;
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

        private static readonly MudColor _defaultColor = "#594ae2";
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

        private static readonly MudColor[] _mudGridDefaultColors = new MudColor[]
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

        private static readonly MudColor[] _mudGridPaletteDefaultColors = new MudColor[]
        {
            "#424242", "#2196f3", "#00c853", "#ff9800", "#f44336",
            "#f6f9fb", "#9df1fa", "#bdffcf", "#fff0a3", "#ffd254",
            "#e6e9eb", "#27dbf5", "#7ef7a0", "#ffe273", "#ffb31f",
            "#c9cccf", "#13b8e8", "#14dc71", "#fdd22f", "#ff9102",
            "#858791", "#0989c2", "#1bbd66", "#ebb323", "#fe6800",
            "#585b62", "#17698e", "#17a258", "#d9980d", "#dc3f11",
            "#353940", "#113b53", "#127942", "#bf7d11", "#aa0000"
        };

        private async Task CheckColorRelatedValues(IRenderedComponent<SimpleColorPickerTest> comp, double expectedX, double expectedY, MudColor expectedColor, ColorPickerMode mode, bool checkInstanceValue = true, bool isRtl = false)
        {
            if (checkInstanceValue)
            {
                await comp.WaitForAssertionAsync(() => comp.Instance.ColorValue.Should().Be(expectedColor));
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
                    castedInputs[3].Value.Should().Match(x => double.Parse(x, CultureInfo.CurrentUICulture) == Math.Round(expectedColor.A / 255.0, 2));
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

            var alphaSliderStyleAttribute = ((IHtmlElement)alphaSlider[0].Parent.Parent).GetAttribute("style");

            if (!isRtl)
            {
                alphaSliderStyleAttribute.Should().Be($"background-image: linear-gradient(to right, transparent, {expectedColor.ToString(MudColorOutputFormats.RGB)});");
            }
            else
            {
                alphaSliderStyleAttribute.Should().Be($"background-image: linear-gradient(to left, transparent, {expectedColor.ToString(MudColorOutputFormats.RGB)});");
            }
        }

        private static IHtmlInputElement[] GetColorInputs(IRenderedComponent<SimpleColorPickerTest> comp, int expectedCount = 4)
        {
            var inputs = comp.FindAll(".mud-picker-color-inputs input");

            inputs.Should().AllBeAssignableTo<IHtmlInputElement>();
            inputs.Should().HaveCount(expectedCount);

            var castedInputs = inputs.Cast<IHtmlInputElement>().ToArray();

            return castedInputs;
        }

        private static IHtmlInputElement GetColorInput(IRenderedComponent<SimpleColorPickerTest> comp, int index, int expectedCount = 4) => GetColorInputs(comp, expectedCount)[index];

        [Test]
        public void ColorPickerOpenButtonDefaultAriaLabel()
        {
            var comp = Context.Render<MudColorPicker>();
            var openButton = comp.Find(".mud-input-adornment button");
            openButton.Attributes.GetNamedItem("aria-label")?.Value.Should().Be("Open");
        }

        [Test]
        public void Default()
        {
            var comp = Context.Render<MudColorPicker>();

            comp.Instance.ShowAlpha.Should().BeTrue();
            comp.Instance.ShowColorField.Should().BeTrue();
            comp.Instance.ShowModeSwitch.Should().BeTrue();
            comp.Instance.ShowInputs.Should().BeTrue();
            comp.Instance.ShowSliders.Should().BeTrue();
            comp.Instance.ShowPreview.Should().BeTrue();
            comp.Instance.ColorPickerMode.Should().Be(ColorPickerMode.RGB);
            comp.Instance.ColorPickerView.Should().Be(ColorPickerView.Spectrum);
            comp.Instance.UpdateBindingIfOnlyHSLChanged.Should().BeFalse();
            comp.Instance.ReadValue.Should().Be(_defaultColor);
            comp.Instance.Palette.Should().BeEquivalentTo(_mudGridPaletteDefaultColors);
            comp.Instance.DragEffect.Should().BeTrue();
        }

        [Test]
        [TestCase(40, 256.78, _defaultYForColorPanel)]
        public async Task SetR(byte r, double selectorXPosition, double selectorYPosition)
        {
            var comp = Context.Render<SimpleColorPickerTest>();

            var rInput = GetColorInput(comp, 0);

            var expectedColor = comp.Instance.ColorValue.SetR(r);
            await rInput.ChangeAsync(expectedColor.R.ToString());

            await CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.RGB);
        }

        [Test]
        [TestCase(240, 196.3, 14.71)]
        public async Task SetG(byte g, double selectorXPosition, double selectorYPosition)
        {
            var comp = Context.Render<SimpleColorPickerTest>();

            var gInput = GetColorInput(comp, 1);

            var expectedColor = comp.Instance.ColorValue.SetG(g);

            await gInput.ChangeAsync(expectedColor.G.ToString());

            await CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.RGB);
        }

        [Test]
        [TestCase(90, 55.47, 161.76)]
        public async Task SetB(byte b, double selectorXPosition, double selectorYPosition)
        {
            var comp = Context.Render<SimpleColorPickerTest>();

            var bInput = GetColorInput(comp, 2);

            var expectedColor = comp.Instance.ColorValue.SetB(b);

            await bInput.ChangeAsync(expectedColor.B.ToString());

            await CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.RGB);
        }

        [Test]
        [TestCase(0.9, _defaultXForColorPanel, _defaultYForColorPanel)]
        public async Task SetA_InRGBMode(double a, double selectorXPosition, double selectorYPosition)
        {
            var comp = Context.Render<SimpleColorPickerTest>();

            var aInput = GetColorInput(comp, 3);

            var expectedColor = comp.Instance.ColorValue.SetAlpha(a);

            await aInput.ChangeAsync(a.ToString(CultureInfo.CurrentUICulture));

            await CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.RGB);
        }

        [Test]
        [TestCase(90, 208.46, _defaultYForColorPanel)]
        public async Task SetH(int h, double selectorXPosition, double selectorYPosition)
        {
            var comp = Context.Render<SimpleColorPickerTest>(p => p.Add(x => x.ColorPickerMode, ColorPickerMode.HSL));

            var hInput = GetColorInput(comp, 0);

            var expectedColor = comp.Instance.ColorValue.SetH(h);

            await hInput.ChangeAsync(expectedColor.H.ToString());

            await CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.HSL);
        }

        [Test]
        [TestCase(0.4, 134.88, 61.76)]
        public async Task SetS(double s, double selectorXPosition, double selectorYPosition)
        {
            var comp = Context.Render<SimpleColorPickerTest>(p => p.Add(x => x.ColorPickerMode, ColorPickerMode.HSL));

            var sColor = GetColorInput(comp, 1);

            var expectedColor = comp.Instance.ColorValue.SetS(s);

            await sColor.ChangeAsync(expectedColor.S.ToString(CultureInfo.CurrentUICulture));

            await CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.HSL);
        }

        [Test]
        [TestCase(0.67, 163.43, 23.53)]
        public async Task SetL(double l, double selectorXPosition, double selectorYPosition)
        {
            var comp = Context.Render<SimpleColorPickerTest>(p => p.Add(x => x.ColorPickerMode, ColorPickerMode.HSL));

            var lColor = GetColorInput(comp, 2);

            var expectedColor = comp.Instance.ColorValue.SetL(l);
            await lColor.ChangeAsync(expectedColor.L.ToString(CultureInfo.CurrentUICulture));

            await CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.HSL);
        }

        [Test]
        [TestCase(0.5, _defaultXForColorPanel, _defaultYForColorPanel)]
        public async Task SetAlpha_AsHLS(double a, double selectorXPosition, double selectorYPosition)
        {
            var comp = Context.Render<SimpleColorPickerTest>(p => p.Add(x => x.ColorPickerMode, ColorPickerMode.HSL));

            var lColor = GetColorInput(comp, 3);

            var expectedColor = comp.Instance.ColorValue.SetAlpha(a);
            await lColor.ChangeAsync(a.ToString(CultureInfo.CurrentUICulture));

            await CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.HSL);
        }

        [Test]
        [TestCase("#8cb829ff", 242.48, 69.61)]
        public async Task SetColorInput(string colorHexString, double selectorXPosition, double selectorYPosition)
        {
            var comp = Context.Render<SimpleColorPickerTest>(p => p.Add(x => x.ColorPickerMode, ColorPickerMode.HEX));

            var inputs = comp.FindAll(".mud-picker-color-inputs input");

            var lColor = GetColorInput(comp, 0, 1);

            var expectedColor = colorHexString;
            await lColor.ChangeAsync(colorHexString);

            await CheckColorRelatedValues(comp, selectorXPosition, selectorYPosition, expectedColor, ColorPickerMode.HEX);
        }

        [Test]
        [TestCase("#8qb829ff")]
        public async Task SetColorInput_InvalidNoChange(string colorHexString)
        {
            var comp = Context.Render<SimpleColorPickerTest>(p => p.Add(x => x.ColorPickerMode, ColorPickerMode.HEX));

            var inputs = comp.FindAll(".mud-picker-color-inputs input");

            var hexInput = GetColorInput(comp, 0, 1);

            var expectedColor = _defaultColor;
            await hexInput.ChangeAsync(colorHexString);

            await CheckColorRelatedValues(comp, _defaultXForColorPanel, _defaultYForColorPanel, expectedColor, ColorPickerMode.HEX);
        }

        [Test]
        public async Task SetAlphaSlider()
        {
            var comp = Context.Render<SimpleColorPickerTest>();

            for (var i = 256 - 1; i >= 0; i--)
            {
                var expectedColor = comp.Instance.ColorValue.SetAlpha((byte)i);

                var hueColorSlider = comp.FindAll(_alphaSliderCssSelector);
                hueColorSlider.Should().ContainSingle();
                hueColorSlider[0].Should().BeAssignableTo<IHtmlInputElement>();

                await hueColorSlider[0].InputAsync(i.ToString());

                await CheckColorRelatedValues(comp, _defaultXForColorPanel, _defaultYForColorPanel, expectedColor, ColorPickerMode.RGB);
            }
        }

        [Test]
        public async Task PointerMove()
        {
            var comp = Context.Render<SimpleColorPickerTest>();

            const double x = 117.0;
            const double y = 140.0;

            var overlay = comp.Find(CssSelector);

            await overlay.PointerDownAsync(new PointerEventArgs { OffsetX = x, OffsetY = y, Buttons = 1 });

            var expectedColor = new MudColor(74, 70, 112, 255);

            await CheckColorRelatedValues(comp, x, y, expectedColor, ColorPickerMode.RGB);

            await overlay.PointerDownAsync(new PointerEventArgs { OffsetX = x, OffsetY = y, Buttons = 0 });

            await CheckColorRelatedValues(comp, x, y, expectedColor, ColorPickerMode.RGB);
        }

        [Test]
        public async Task SetHueSlider()
        {
            var comp = Context.Render<SimpleColorPickerTest>();

            for (var i = 0; i <= 360; i++)
            {
                var expectedColor = comp.Instance.ColorValue.SetH(i);

                var hueColorSlider = comp.FindAll(_hueSliderCssSelector);
                hueColorSlider.Should().ContainSingle();
                hueColorSlider[0].Should().BeAssignableTo<IHtmlInputElement>();

                await hueColorSlider[0].InputAsync(i.ToString());

                await CheckColorRelatedValues(comp, 208.46, _defaultYForColorPanel, expectedColor, ColorPickerMode.RGB);
            }
        }

        [Test]
        public async Task Click_ColorPanel()
        {
            var comp = Context.Render<SimpleColorPickerTest>();

            var overlay = comp.Find(CssSelector);

            const double x = 99.2;
            const double y = 200.98;

            await overlay.PointerDownAsync(new PointerEventArgs { OffsetX = x, OffsetY = y });

            MudColor color = "#232232ff";
            var expectedColor = new MudColor(color.R, color.G, color.B, _defaultColor);

            await CheckColorRelatedValues(comp, x, y, expectedColor, ColorPickerMode.RGB);
        }

        [Test]
        public async Task Click_ModeButton()
        {
            var comp = Context.Render<SimpleColorPickerTest>();

            var color = comp.Instance.ColorValue;
            var modeButton = comp.Find(".mud-picker-control-switch button");

            //default to rgb
            await CheckColorRelatedValues(comp, _defaultXForColorPanel, _defaultYForColorPanel, color, ColorPickerMode.RGB);

            // click to switch to HSL
            await modeButton.ClickAsync();
            await CheckColorRelatedValues(comp, _defaultXForColorPanel, _defaultYForColorPanel, color, ColorPickerMode.HSL);

            //click again to switch to hex
            await modeButton.ClickAsync();
            await CheckColorRelatedValues(comp, _defaultXForColorPanel, _defaultYForColorPanel, color, ColorPickerMode.HEX);

            //click last time to reset to RGB
            await modeButton.ClickAsync();
            await CheckColorRelatedValues(comp, _defaultXForColorPanel, _defaultYForColorPanel, color, ColorPickerMode.RGB);
        }

        [Test]
        public async Task ColorPalette_Interaction()
        {
            var comp = Context.Render<SimpleColorPickerTest>();

            var colorDot = comp.Find(_colorDotCssSelector);
            // no collection
            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-picker-color-collection"));
            await colorDot.ClickAsync();

            // collection found
            var colorsToSelectPanel = comp.Find(".mud-picker-color-collection");
            colorsToSelectPanel.Children.Should().HaveCount(5);

            var expectedColors = new MudColor[] { "#ff4081ff", "#2196f3ff", "#00c853ff", "#ff9800ff", "#f44336ff" };

            for (var i = 0; i < 5; i++)
            {
                var styleAttribute = colorsToSelectPanel.Children[i].GetAttribute("style");
                styleAttribute.Should().Be($"background: {expectedColors[i].ToString(MudColorOutputFormats.RGBA)};");
            }

            await colorDot.ClickAsync();

            // again no collection visible
            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-picker-color-collection"));
        }

        [Test]
        public async Task ColorPalette_CustomColors()
        {
            var expectedColors = new MudColor[] { "#23af3daa", "#56a23dff", "#56a85dff" };

            var comp = Context.Render<SimpleColorPickerTest>(p => p.Add(x => x.Palette, expectedColors));

            var colorDot = comp.Find(_colorDotCssSelector);
            await colorDot.ClickAsync();

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
        public async Task ColorPalette_SelectColor()
        {
            var comp = Context.Render<SimpleColorPickerTest>();

            var colorDot = comp.Find(_colorDotCssSelector);
            // no collection
            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-picker-color-collection"));
            await colorDot.ClickAsync();

            var expectedColors = new MudColor[] { "#ff4081ff", "#2196f3ff", "#00c853ff", "#ff9800ff", "#f44336ff" };

            for (var i = 0; i < 5; i++)
            {
                var colorsToSelectPanel = comp.Find(".mud-picker-color-collection");

                await colorsToSelectPanel.Children[i].ClickAsync();
                colorDot = comp.Find(_colorDotCssSelector);

                var styleAttribute = colorDot.GetAttribute("style");
                styleAttribute.Should().Be($"background: {expectedColors[i].ToString(MudColorOutputFormats.RGBA)};");
                comp.Instance.ColorValue.Should().Be(expectedColors[i]);

                await colorDot.ClickAsync();
            }
        }

        [Test]
        public async Task Toggle_Toolbar()
        {
            var comp = Context.Render<SimpleColorPickerTest>(p => p.Add(x => x.ShowToolbar, true));

            _ = comp.Find(_toolbarCssSelector);

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ShowToolbar, false));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_toolbarCssSelector));

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ShowToolbar, true));

            _ = comp.Find(_toolbarCssSelector);
        }

        [Test]
        public async Task Toggle_ColorField()
        {
            var comp = Context.Render<SimpleColorPickerTest>(p => p.Add(x => x.ShowColorField, true));

            _ = comp.Find(_mudColorPickerCssSelector);

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ShowColorField, false));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_mudColorPickerCssSelector));

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ShowColorField, true));

            _ = comp.Find(_mudColorPickerCssSelector);
        }

        [Test]
        public async Task Toggle_Preview()
        {
            var comp = Context.Render<SimpleColorPickerTest>(p => p.Add(x => x.ShowPreview, true));

            _ = comp.Find(_colorDotCssSelector);

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ShowPreview, false));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_colorDotCssSelector));

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ShowPreview, true));

            _ = comp.Find(_colorDotCssSelector);
        }

        [Test]
        public async Task Toggle_Sliders()
        {
            var comp = Context.Render<SimpleColorPickerTest>(p => p.Add(x => x.ShowSliders, true));

            _ = comp.Find(_slidersControlCssSelector);

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ShowSliders, false));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_slidersControlCssSelector));

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ShowSliders, true));

            _ = comp.Find(_slidersControlCssSelector);
        }

        [Test]
        [TestCase(ColorPickerMode.HEX)]
        [TestCase(ColorPickerMode.HSL)]
        [TestCase(ColorPickerMode.RGB)]
        public async Task Toggle_Input(ColorPickerMode mode)
        {
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ColorPickerMode, mode);
                p.Add(x => x.DisableInput, false);

            });

            _ = comp.Find(_colorInputCssSelector);

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.DisableInput, true));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_colorInputCssSelector));

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.DisableInput, false));

            _ = comp.Find(_colorInputCssSelector);
        }

        [Test]
        public async Task Toggle_ModeSwitch()
        {
            var comp = Context.Render<SimpleColorPickerTest>(p => p.Add(x => x.ShowModeSwitch, true));

            _ = comp.Find(_colorInputModeSwitchCssSelector);

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ShowModeSwitch, false));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_colorInputModeSwitchCssSelector));

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ShowModeSwitch, true));

            _ = comp.Find(_colorInputModeSwitchCssSelector);
        }

        [Test]
        public void Tooltips_Disabled_ShouldSuppressTooltipContent()
        {
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ShowToolbar, true);
                p.Add(x => x.ShowTooltips, false);
            });

            var tooltipRoots = comp.FindAll(".mud-tooltip-root");
            tooltipRoots.Should().HaveCount(4);

            foreach (var root in tooltipRoots)
            {
                root.Children.Length.Should().Be(1);
            }

            var tooltips = comp.FindComponents<MudTooltip>();
            tooltips.Should().HaveCount(4);
            tooltips.Should().OnlyContain(t => string.IsNullOrEmpty(t.Instance.Text));
            comp.FindAll(".mud-tooltip").Should().BeEmpty();
        }

        [Test]
        public void Tooltips_Enabled_ShouldDisplayLocalizedContent()
        {
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ShowToolbar, true);
                p.Add(x => x.ShowTooltips, true);
            });

            var localizer = Context.Services.GetRequiredService<InternalMudLocalizer>();
            var expectedTexts = new[]
            {
                localizer[LanguageResource.MudColorPicker_SpectrumView].Value,
                localizer[LanguageResource.MudColorPicker_GridView].Value,
                localizer[LanguageResource.MudColorPicker_PaletteView].Value,
                localizer[LanguageResource.MudColorPicker_ModeSwitch].Value,
            };

            var tooltips = comp.FindComponents<MudTooltip>();
            tooltips.Should().HaveCount(4);
            tooltips.Select(t => t.Instance.Text).Should().Equal(expectedTexts);
        }

        [Test]
        [TestCase(ColorPickerMode.HSL)]
        [TestCase(ColorPickerMode.RGB)]
        public async Task Toggle_Alpha(ColorPickerMode mode)
        {
            var color = new MudColor(12, 220, 124, 120);
            var expectedColor = new MudColor(12, 220, 124, 120);

            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ColorPickerMode, mode);
                p.Add(x => x.ShowAlpha, true);
                p.Add(x => x.ColorValue, color);
            });

            _ = comp.Find(_alphaInputCssSelector);

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ShowAlpha, false));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_alphaInputCssSelector));
            comp.Instance.ColorValue.Should().Be(expectedColor);
            comp.Instance.TextValue.Should().Be(expectedColor.ToString(MudColorOutputFormats.Hex));

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ShowAlpha, true));

            _ = comp.Find(_alphaInputCssSelector);
            comp.Instance.ColorValue.Should().Be(expectedColor);
            comp.Instance.TextValue.Should().Be(expectedColor.ToString(MudColorOutputFormats.HexA));
        }

        [Test]
        public async Task Toggle_Alpha_HexInputMode()
        {
            var color = new MudColor(12, 220, 124, 120);
            var expectedColor = new MudColor(12, 220, 124, 120);

            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ColorPickerMode, ColorPickerMode.HEX);
                p.Add(x => x.ShowAlpha, true);
                p.Add(x => x.ColorValue, color);
            });

            Assert.Throws<ElementNotFoundException>(() => comp.Find(_alphaInputCssSelector));

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ShowAlpha, false));

            comp.Instance.ColorValue.Should().Be(expectedColor);

            var inputs = comp.FindAll(".mud-picker-color-inputfield input");
            inputs.Should().ContainSingle();
            inputs.Should().AllBeAssignableTo<IHtmlInputElement>();
            ((IHtmlInputElement)inputs[0]).Value.Should().Be("#0cdc7c");
            ((IHtmlInputElement)inputs[0]).MaxLength.Should().Be(7);

            comp.Instance.TextValue.Should().Be("#0cdc7c");

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ShowAlpha, true));
            comp.Instance.ColorValue.Should().Be(expectedColor);
            inputs = comp.FindAll(".mud-picker-color-inputfield input");
            inputs.Should().ContainSingle();
            inputs.Should().AllBeAssignableTo<IHtmlInputElement>();
            ((IHtmlInputElement)inputs[0]).Value.Should().Be("#0cdc7c78");
            ((IHtmlInputElement)inputs[0]).MaxLength.Should().Be(9);

            comp.Instance.TextValue.Should().Be("#0cdc7c78");
        }

        [Test]
        public async Task ToggleViewMode()
        {
            var comp = Context.Render<MudColorPicker>(p =>
            {
                p.Add(x => x.ShowToolbar, true);
                p.Add(x => x.PickerVariant, PickerVariant.Static);
            });

            IReadOnlyList<IElement> Buttons() => comp.FindAll(_mudToolbarButtonsCssSelector);

            Dictionary<int, (ColorPickerView, string)> buttonMapper = new()
            {
                { 2, (ColorPickerView.Palette, ".mud-picker-color-view-collection") },
                { 1, (ColorPickerView.Grid, ".mud-picker-color-grid") },
                { 0, (ColorPickerView.Spectrum, ".mud-picker-color-overlay") },
            };

            foreach (var item in buttonMapper)
            {
                await Buttons()[item.Key].ClickAsync();

                _ = comp.Find(item.Value.Item2);
            }
        }

        [Test]
        [TestCase(PickerVariant.Static, false)]
        [TestCase(PickerVariant.Inline, true)]
        [TestCase(PickerVariant.Dialog, true)]
        public async Task CloseButtonInToolbarVisible(PickerVariant variant, bool expectedVisibility)
        {
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.Variant, variant);
                p.Add(x => x.ShowToolbar, true);
            });

            if (variant is PickerVariant.Dialog or PickerVariant.Inline)
            {
                await comp.Instance.OpenPicker();
            }

            if (!expectedVisibility)
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
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, view);
                p.Add(x => x.ShowToolbar, true);
            });

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
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, ColorPickerView.Palette);
                p.Add(x => x.Palette, _mudGridPaletteDefaultColors);
            });

            var expectedColors = _mudGridPaletteDefaultColors;

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
        public async Task PaletteView_ChooseColor()
        {
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, ColorPickerView.Palette);
                p.Add(x => x.Palette, _mudGridPaletteDefaultColors);
            });

            var expectedColors = _mudGridPaletteDefaultColors;
            var collectionView = comp.Find(".mud-picker-color-view-collection");

            for (var i = 0; i < collectionView.Children.Length; i++)
            {
                var expectedColor = expectedColors[i];
                var colorElement = collectionView.Children[i];
                await colorElement.ClickAsync();

                await comp.WaitForAssertionAsync(() => comp.Instance.ColorValue.Should().Be(expectedColor));
                comp.Find(".mud-picker-color-view-collection").Children[i].ClassList.Should().BeEquivalentTo("mud-picker-color-dot", "selected");

            }
        }

        [Test]
        public void GridView()
        {
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, ColorPickerView.Grid);
            });

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
        public async Task GridView_ChooseColor()
        {
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, ColorPickerView.Grid);
            });

            var expectedColors = _mudGridDefaultColors;

            var collectionView = comp.Find(".mud-picker-color-grid");

            for (var i = 0; i < collectionView.Children.Length; i++)
            {
                var expectedColor = expectedColors[i];
                var colorElement = collectionView.Children[i];
                colorElement.ClassList.Should().Contain("mud-picker-color-dot");

                await comp.InvokeAsync(() => colorElement.ClickAsync());
                await comp.WaitForAssertionAsync(() => comp.Instance.ColorValue.Should().Be(expectedColor));

                comp.Find(".mud-picker-color-grid").Children[i].ClassList.Should().BeEquivalentTo("mud-picker-color-dot", "selected");
            }
        }

        [Test]
        public void GridCompactView()
        {
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, ColorPickerView.GridCompact);
            });

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
        public async Task GridCompactView_ChooseColor()
        {
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, ColorPickerView.GridCompact);
            });

            var expectedColors = _mudGridDefaultColors;

            var gridView = comp.Find(".mud-picker-color-grid");

            gridView.Children.Should().HaveCount(expectedColors.Length);

            for (var i = 0; i < gridView.Children.Length; i++)
            {
                var expectedColor = expectedColors[i];
                var colorElement = gridView.Children[i];
                colorElement.ClassList.Should().Contain("mud-picker-color-dot");

                await colorElement.ClickAsync();
                await comp.WaitForAssertionAsync(() => comp.Instance.ColorValue.Should().Be(expectedColor));

                comp.Find(".mud-picker-color-grid").Children[i].ClassList.Should().BeEquivalentTo("mud-picker-color-dot", "selected");
            }
        }

        [Test]
        [TestCase(PickerVariant.Inline)]
        [TestCase(PickerVariant.Dialog)]
        public async Task GridCompact_CloseOnSelect(PickerVariant variant)
        {
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, ColorPickerView.GridCompact);
                p.Add(x => x.Variant, variant);
            });

            await comp.Instance.OpenPicker();

            var expectedColors = _mudGridDefaultColors;

            await (await comp.WaitForElementAsync(".mud-picker-color-grid")).Children[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.Instance.ColorValue.Should().Be(expectedColors[0]));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-picker-container"));
        }

        [Test]
        [TestCase(PickerVariant.Inline)]
        [TestCase(PickerVariant.Dialog)]
        public async Task Palette_CloseOnSelect(PickerVariant variant)
        {
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, ColorPickerView.Palette);
                p.Add(x => x.Variant, variant);
                p.Add(x => x.Palette, _mudGridPaletteDefaultColors);
            });

            await comp.Instance.OpenPicker();

            var expectedColors = _mudGridPaletteDefaultColors;

            await (await comp.WaitForElementAsync(".mud-picker-color-view-collection")).Children[0].ClickAsync();
            await comp.WaitForAssertionAsync(() => comp.Instance.ColorValue.Should().Be(expectedColors[0]));

            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-picker-container"));
        }

        [Test]
        [TestCase(PickerVariant.Inline, ColorPickerView.Grid)]
        [TestCase(PickerVariant.Dialog, ColorPickerView.Grid)]
        [TestCase(PickerVariant.Inline, ColorPickerView.Spectrum)]
        [TestCase(PickerVariant.Dialog, ColorPickerView.Spectrum)]
        public async Task NoControls_CloseOnSelect(PickerVariant variant, ColorPickerView view)
        {
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, view);
                p.Add(x => x.Variant, variant);
                p.Add(x => x.ShowPreview, false);
                p.Add(x => x.ShowSliders, false);
                p.Add(x => x.DisableInput, true);
            });

            await comp.Instance.OpenPicker();

            var expectedColors = view == ColorPickerView.Grid ? _mudGridDefaultColors : _mudGridPaletteDefaultColors;

            IElement item;
            if (view == ColorPickerView.Grid)
            {
                item = comp.Find(".mud-picker-color-grid").Children[0];
            }
            else
            {
                item = comp.Find("[class=\"mud-picker-color-overlay\"][id]");
            }

            if (view == ColorPickerView.Spectrum)
            {
                await item.PointerDownAsync();
            }
            else
            {
                await item.ClickAsync();
            }

            await comp.WaitForAssertionAsync(() => comp.Instance.ColorValue.Should().NotBe(_defaultColor));

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
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ViewMode, view);
                p.Add(x => x.ShowAlpha, !disableAlpha);
                p.Add(x => x.ColorValue, new MudColor(120, 121, 122, 1.0));
            });

            comp.Instance.TextValue.Should().Be(expectedOutput);
        }

        [Test]
        public async Task SetNullColor_NothingChanged()
        {
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ColorPickerMode, ColorPickerMode.HSL);
            });

            await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ColorValue, null));

            var lColor = GetColorInput(comp, 2);
            var expectedColor = _defaultColor;

            await CheckColorRelatedValues(comp, _defaultXForColorPanel, _defaultYForColorPanel, expectedColor, ColorPickerMode.HSL, false);
            comp.FindComponent<MudColorPicker>().Instance.ReadValue.Should().Be(_defaultColor);
        }

        [Test]
        public async Task SetHLS_NotChangeRBG_ButCallbackFired()
        {
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.ColorPickerMode, ColorPickerMode.HSL);
                p.Add(x => x.ColorValue, new MudColor(245, 0.35, 0.95, 1.0));
            });

            var sColor = GetColorInput(comp, 1);

            var colorValue = comp.Instance.ColorValue.ToString(MudColorOutputFormats.HexA);
            await CheckColorRelatedValues(comp, 11.37, 7.84, comp.Instance.ColorValue, ColorPickerMode.HSL);

            var expectedColor = comp.Instance.ColorValue.SetS(comp.Instance.ColorValue.S - 0.01);
            await sColor.ChangeAsync(expectedColor.S.ToString(CultureInfo.CurrentUICulture));

            await CheckColorRelatedValues(comp, 11.37, 7.84, expectedColor, ColorPickerMode.HSL);
            var colorValueAfterChange = comp.Instance.ColorValue.ToString(MudColorOutputFormats.HexA);

            colorValue.Should().Be(colorValueAfterChange);
            comp.Instance.ValueChangeCallbackCounter.Should().Be(1);
        }

        [Test]
        public async Task RTL_AlphaSliderInverseStyle()
        {
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.AddCascadingValue("RightToLeft", true);
            });

            await CheckColorRelatedValues(comp, _defaultXForColorPanel, _defaultYForColorPanel, comp.Instance.ColorValue, ColorPickerMode.RGB, true, true);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public async Task Spectrum_DragPointer(bool disableDragEffect)
        {
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                //p.Add(x => x.Variant, PickerVariant.Static);
                //p.Add(x => x.ViewMode, ColorPickerView.Spectrum);
                p.Add(x => x.DragEffect, !disableDragEffect);
            });

            const double x1 = 99.2;
            const double y1 = 200.98;
            var expectedColor1 = new MudColor(35, 34, 50, _defaultColor);

            var overlay = comp.Find(CssSelector);

            // Color should update as soon as pointer is down.
            await overlay.PointerDownAsync(new PointerEventArgs { OffsetX = x1, OffsetY = y1 });
            await CheckColorRelatedValues(comp, x1, y1, expectedColor1, ColorPickerMode.RGB);

            const double x2 = 117.0;
            const double y2 = 140.0;
            var expectedColor2 = new MudColor(74, 70, 112, 255);

            await overlay.PointerMoveAsync(new PointerEventArgs { OffsetX = x2, OffsetY = y2, Buttons = 1 });

            // Color shouldn't update if the drag effect is disabled.
            if (disableDragEffect)
            {
                await CheckColorRelatedValues(comp, x1, y1, expectedColor1, ColorPickerMode.RGB);
            }
            else
            {
                await CheckColorRelatedValues(comp, x2, y2, expectedColor2, ColorPickerMode.RGB);
            }

            // Color should update when pointer is released regardless of drag being enabled.
            await overlay.PointerUpAsync(new PointerEventArgs { OffsetX = x2, OffsetY = y2 });
            await CheckColorRelatedValues(comp, x2, y2, expectedColor2, ColorPickerMode.RGB);
        }

        [Test]
        public void StableHue_WhenColorSpectrumClicked()
        {
            var comp = Context.Render<MudColorPicker>(p =>
            {
                p.Add(x => x.PickerVariant, PickerVariant.Static);
                p.Add(x => x.ColorPickerView, ColorPickerView.Spectrum);
            });

            var overlay = comp.Find(CssSelector);

            var expectedHue = _defaultColor.H;

            for (var x = 0; x < 312; x += 5)
            {
                for (var y = 0; y < 250; y += 5)
                {
                    overlay.PointerDown(new PointerEventArgs { OffsetX = x, OffsetY = y });

                    comp.Instance.ReadValue.H.Should().Be(expectedHue);
                }
            }
        }

        [Test]
        public async Task CheckPickerPopover()
        {
            var comp = Context.Render<SimpleColorPickerTest>(p =>
            {
                p.Add(x => x.Variant, PickerVariant.Inline);
            });

            await comp.Instance.OpenPicker();

            var providerNode = comp.Find(".mud-popover-provider");
            providerNode.Children.Length.Should().BeGreaterThanOrEqualTo(2);

            var popoverNode = providerNode.Children
                .FirstOrDefault(child => child.ClassList.Contains("mud-picker-popover"));

            popoverNode.Should().NotBeNull();

            popoverNode!.ClassList.Should().BeEquivalentTo(
            [
                "mud-popover",
                "mud-popover-fixed",
                "mud-popover-open",
                "mud-popover-top-left",
                "mud-popover-anchor-bottom-left",
                "mud-popover-overflow-flip-onopen",
                "mud-picker-popover",
                "mud-elevation-8",
            ]);
        }

        //https://github.com/MudBlazor/MudBlazor/issues/4899
        [Test]
        public async Task DistinguishBetweenInternalAndExternalView()
        {
            var comp = Context.Render<PickerWithFixedView>();

            //open the color picker
            var inputField = comp.Find(".mud-input-slot");
            await inputField.ClickAsync();

            //asset that the picker is open in grid mode
            var grid = comp.Find(".mud-picker-color-grid");

            //find spectrum button and click
            var spectrumButton = comp.FindAll(_mudToolbarButtonsCssSelector)[1];
            await spectrumButton.ClickAsync();

            //find the overlay and click any position
            var overlay = comp.Find(CssSelector);
            await overlay.PointerDownAsync(new PointerEventArgs { OffsetX = 10.5, OffsetY = 10.5 });

            //ensure that the spectrum mode is still open and not the color grid
            _ = comp.Find(".mud-picker-color-overlay");
            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-picker-color-grid"));

            //change the view per parameter
            await comp.SetParametersAndRenderAsync(x => x.Add(y => y.ColorPickerView, ColorPickerView.GridCompact));

            //now the grid view should be visible
            Assert.Throws<ElementNotFoundException>(() => comp.Find(".mud-picker-color-overlay"));
            _ = comp.Find(".mud-picker-color-grid");
        }

        /// <summary>
        /// Ensures both the text and value update when the text is changed
        /// </summary>
        [Test]
        public async Task ColorPickerValueShouldUpdateOnTextTestAsync()
        {
            var comp = Context.Render<MudColorPicker>(parameters => parameters
                .Add(x => x.Editable, true));

            await comp.Find("input").ChangeAsync("#180f6fff");
            comp.Instance.GetState(x => x.Text).Should().Be("#180f6fff");
            comp.Instance.GetState(x => x.Value).Should().Be(new MudColor("#180f6fff"));
        }

        /// <summary>
        /// A color picker with a label should auto-generate an id and use that id on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void ColorPickerWithLabel_Should_GenerateIdForInputAndAccompanyingLabel()
        {
            var comp = Context.Render<MudColorPicker>(parameters
                => parameters.Add(p => p.Label, "Test Label"));

            comp.Find("input").Id.Should().NotBeNullOrEmpty();
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(comp.Find("input").Id);
        }

        /// <summary>
        /// A color picker with a label and UserAttributesId should use the UserAttributesId on the input element and the label's for attribute.
        /// </summary>
        [Test]
        public void ColorPickerWithLabelAndUserAttributesId_Should_UseUserAttributesIdForInputAndAccompanyingLabel()
        {
            var expectedId = "userattributes-id";
            var comp = Context.Render<MudColorPicker>(parameters
                => parameters
                    .Add(p => p.Label, "Test Label")
                    .Add(p => p.UserAttributes, new Dictionary<string, object>
                    {
                        { "Id", expectedId }
                    }));

            comp.Find("input").Id.Should().Be(expectedId);
            comp.Find("label").Attributes.GetNamedItem("for").Should().NotBeNull();
            comp.Find("label").Attributes.GetNamedItem("for")!.Value.Should().Be(expectedId);
        }

        /// <summary>
        /// Optional RGB ColorPicker should not have required attribute and aria-required should be false.
        /// </summary>
        [Test]
        public void OptionalRGBColorPicker_Should_NotHaveRequiredAttributeAndAriaRequiredShouldBeFalse()
        {
            var comp = Context.Render<SimpleColorPickerTest>(parameters => parameters
                .Add(p => p.ColorPickerMode, ColorPickerMode.RGB));

            comp.FindAll("input:not(.mud-slider-input)").Should().AllSatisfy(input =>
            {
                input.HasAttribute("required").Should().BeFalse();
                input.GetAttribute("aria-required").Should().Be("false");
            });
        }

        /// <summary>
        /// Required RGB ColorPicker should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredRGBColorPicker_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.Render<SimpleColorPickerTest>(parameters => parameters
                .Add(p => p.Required, true)
                .Add(p => p.ColorPickerMode, ColorPickerMode.RGB));

            comp.FindAll("input:not(.mud-slider-input)").Should().AllSatisfy(input =>
            {
                input.HasAttribute("required").Should().BeTrue();
                input.GetAttribute("aria-required").Should().Be("true");
            });
        }

        /// <summary>
        /// Required and aria-required RGB ColorPicker attributes should be dynamic.
        /// </summary>
        [Test]
        public async Task RequiredAndAriaRequiredRGBColorPickerAttributes_Should_BeDynamic()
        {
            var comp = Context.Render<SimpleColorPickerTest>(parameters => parameters
                .Add(p => p.ColorPickerMode, ColorPickerMode.RGB));

            comp.FindAll("input:not(.mud-slider-input)").Should().AllSatisfy(input =>
            {
                input.HasAttribute("required").Should().BeFalse();
                input.GetAttribute("aria-required").Should().Be("false");
            });

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Required, true));

            comp.FindAll("input:not(.mud-slider-input)").Should().AllSatisfy(input =>
            {
                input.HasAttribute("required").Should().BeTrue();
                input.GetAttribute("aria-required").Should().Be("true");
            });
        }

        /// <summary>
        /// Optional HSL ColorPicker should not have required attribute and aria-required should be false.
        /// </summary>
        [Test]
        public void OptionalHSLColorPicker_Should_NotHaveRequiredAttributeAndAriaRequiredShouldBeFalse()
        {
            var comp = Context.Render<SimpleColorPickerTest>(parameters => parameters
                .Add(p => p.ColorPickerMode, ColorPickerMode.HSL));

            comp.FindAll("input:not(.mud-slider-input)").Should().AllSatisfy(input =>
            {
                input.HasAttribute("required").Should().BeFalse();
                input.GetAttribute("aria-required").Should().Be("false");
            });
        }

        /// <summary>
        /// Required HSL ColorPicker should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredHSLColorPicker_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.Render<SimpleColorPickerTest>(parameters => parameters
                .Add(p => p.Required, true)
                .Add(p => p.ColorPickerMode, ColorPickerMode.HSL));

            comp.FindAll("input:not(.mud-slider-input)").Should().AllSatisfy(input =>
            {
                input.HasAttribute("required").Should().BeTrue();
                input.GetAttribute("aria-required").Should().Be("true");
            });
        }

        /// <summary>
        /// Required and aria-required HSL ColorPicker attributes should be dynamic.
        /// </summary>
        [Test]
        public async Task RequiredAndAriaRequiredHSLColorPickerAttributes_Should_BeDynamic()
        {
            var comp = Context.Render<SimpleColorPickerTest>(parameters => parameters
                .Add(p => p.ColorPickerMode, ColorPickerMode.HSL));

            comp.FindAll("input:not(.mud-slider-input)").Should().AllSatisfy(input =>
            {
                input.HasAttribute("required").Should().BeFalse();
                input.GetAttribute("aria-required").Should().Be("false");
            });

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Required, true));

            comp.FindAll("input:not(.mud-slider-input)").Should().AllSatisfy(input =>
            {
                input.HasAttribute("required").Should().BeTrue();
                input.GetAttribute("aria-required").Should().Be("true");
            });
        }

        /// <summary>
        /// Optional HEX ColorPicker should not have required attribute and aria-required should be false.
        /// </summary>
        [Test]
        public void OptionalHEXColorPicker_Should_NotHaveRequiredAttributeAndAriaRequiredShouldBeFalse()
        {
            var comp = Context.Render<SimpleColorPickerTest>(parameters => parameters
                .Add(p => p.ColorPickerMode, ColorPickerMode.HEX));

            comp.FindAll("input:not(.mud-slider-input)").Should().AllSatisfy(input =>
            {
                input.HasAttribute("required").Should().BeFalse();
                input.GetAttribute("aria-required").Should().Be("false");
            });
        }

        /// <summary>
        /// Required HEX ColorPicker should have required and aria-required attributes.
        /// </summary>
        [Test]
        public void RequiredHEXColorPicker_Should_HaveRequiredAndAriaRequiredAttributes()
        {
            var comp = Context.Render<SimpleColorPickerTest>(parameters => parameters
                .Add(p => p.Required, true)
                .Add(p => p.ColorPickerMode, ColorPickerMode.HEX));

            comp.FindAll("input:not(.mud-slider-input)").Should().AllSatisfy(input =>
            {
                input.HasAttribute("required").Should().BeTrue();
                input.GetAttribute("aria-required").Should().Be("true");
            });
        }

        /// <summary>
        /// Required and aria-required HEX ColorPicker attributes should be dynamic.
        /// </summary>
        [Test]
        public async Task RequiredAndAriaRequiredHEXColorPickerAttributes_Should_BeDynamic()
        {
            var comp = Context.Render<SimpleColorPickerTest>(parameters => parameters
                .Add(p => p.ColorPickerMode, ColorPickerMode.HEX));

            comp.FindAll("input:not(.mud-slider-input)").Should().AllSatisfy(input =>
            {
                input.HasAttribute("required").Should().BeFalse();
                input.GetAttribute("aria-required").Should().Be("false");
            });

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.Required, true));

            // must be re-fetched after re-render
            comp.FindAll("input:not(.mud-slider-input)").Should().AllSatisfy(input =>
            {
                input.HasAttribute("required").Should().BeTrue();
                input.GetAttribute("aria-required").Should().Be("true");
            });
        }

        [Test]
        public void ColorPickerInputId()
        {
            var comp = Context.Render<SimpleColorPickerTest>(parameters => parameters
                .Add(c => c.Variant, PickerVariant.Inline)
                .Add(c => c.InputId, "primary-color"));

            comp.Find("input[id='primary-color']").Should().NotBeNull();
        }

        [Test]
        public void ColorPicker_CustomClearIcon_Should_BeRenderedInMarkup()
        {
            var comp = Context.Render<MudColorPicker>(parameters => parameters
                .Add(p => p.Value, new MudColor("#180f6fff"))
                .Add(p => p.Editable, true)
                .Add(p => p.Clearable, true)
                .Add(p => p.ClearIcon, Icons.Custom.Brands.MudBlazor));

            comp.Markup.Should().Contain(comp.Instance.ClearIcon);
        }
    }
}
