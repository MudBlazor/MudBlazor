// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using MudColor = System.Drawing.Color;

namespace MudBlazor
{
    public partial class MudColorPicker : MudPicker<MudColor>
    {
        public MudColorPicker() : base(new DefaultConverter<MudColor>())
        {
            AdornmentIcon = Icons.Custom.Uncategorized.ColorPalette;
        }

        private record RGBColor(int R, int G, int B);

        #region Fields

        private const double _maxY = 250;
        private const double _maxX = 310;

        private bool _isMouseDown;
        private double _selectorX;
        private double _selectorY;

        private int _pickerBaseColorValue;
        private double _pickerAlpha = 1;

        private RGBColor _baseColor = new(255, 0, 0);

        private int _r = 255;
        private int _g = 0;
        private int _b = 0;
        private string _rgbAHex = "#ffffffff";

        private double _h = 0;
        private double _s = 0;
        private double _l = 0;

        #endregion

        #region Parameters

        #endregion


        /// <summary>
        /// If true, Alpha options will not be displayed and color output will be RGB, HSL or HEX and not RGBA, HSLA or HEXA.
        /// </summary>
        [Parameter] public bool DisableAlpha { get; set; }

        /// <summary>
        /// If true, the color field will not be displayed.
        /// </summary>
        [Parameter] public bool DisableColorField { get; set; }

        /// <summary>
        /// If true, the switch to change color mode will not be displayed.
        /// </summary>
        [Parameter] public bool DisableModeSwitch { get; set; }

        /// <summary>
        /// If true, textfield inputs and color mode switch will not be displayed.
        /// </summary>
        [Parameter] public bool DisableInputs { get; set; }

        /// <summary>
        /// If true, hue and alpha sliders will not be displayed.
        /// </summary>
        [Parameter] public bool DisableSliders { get; set; }

        [Parameter] public ColorPickerMode ColorPickerMode { get; set; } = ColorPickerMode.RGB;

        public ColorPickerRgbA ColorPickerRgb = new ColorPickerRgbA();
        public ColorPickerHslA ColorPickerHsl = new ColorPickerHslA();
        public string ColorPickerHex { get; set; } = "#ff0000"; //Remove default here only used for fake content.

        public class ColorPickerRgbA
        {
            public int R { get; set; } = 211; //Remove default here only used for fake content.
            public int G { get; set; }
            public int B { get; set; }
            public double A { get; set; }
        }

        public class ColorPickerHslA
        {
            public int H { get; set; }
            public int S { get; set; } = 100; //Remove default here only used for fake content.
            public int L { get; set; } = 50; //Remove default here only used for fake content.
            public double A { get; set; }
        }

        public void ChangeMode() =>
            ColorPickerMode = ColorPickerMode switch
            {
                ColorPickerMode.RGB => ColorPickerMode.HSL,
                ColorPickerMode.HSL => ColorPickerMode.HEX,
                ColorPickerMode.HEX => ColorPickerMode.RGB,
                _ => ColorPickerMode.RGB,
            };

        private void UpdateColor(Boolean fireStateHasChanged) => UpdateColor(_selectorX, _selectorY, fireStateHasChanged);

        private void UpdateBaseColor(int input) => UpdateBaseColor(input, true);

        private static Dictionary<Int32, (Func<int, int> r, Func<int, int> g, Func<int, int> b, char dominantColorPart)> rgbMapper = new()
        {
            { 0, ((x) => 255, x => x, x => 0, 'r') },
            { 1, ((x) => 255 - x, x => 255, x => 0, 'g') },
            { 2, ((x) => 0, x => 255, x => x, 'g') },
            { 3, ((x) => 0, x => 255 - x, x => 255, 'b') },
            { 4, ((x) => x, x => 0, x => 255, 'b') },
            { 5, ((x) => 255, x => 0, x => 255 - x, 'r') },
        };

        private void UpdateBaseColor(int input, bool withColorSelector)
        {
            _pickerBaseColorValue = input;

            Int32 index = input / 255;
            Int32 value = input - (index * 255);

            var section = rgbMapper[index];

            _baseColor = new RGBColor(section.r(value), section.g(value), section.b(value));
            if (withColorSelector == true)
            {
                UpdateColor(false);
            }
        }

        private void UpdateColor(Double selectedX, Double selectedY, Boolean fireStateHasChanged)
        {
            double x = selectedX / _maxX;

            double r_x = 255 - (255.0 - _baseColor.R) * x;
            double g_x = 255 - (255.0 - _baseColor.G) * x;
            double b_x = 255 - (255.0 - _baseColor.B) * x;

            double y = 1.0 - selectedY / _maxY;

            double r = r_x * y;
            double g = g_x * y;
            double b = b_x * y;

            _r = (int)r;
            _g = (int)g;
            _b = (int)b;

            UpdateColorHexString();
            UpdateHLsValues();
            SetTextAsync(_rgbAHex, true).AndForget();

            if (fireStateHasChanged == true)
            {
                StateHasChanged();
            }
        }

        private void UpdateColorSelectorBasedOnRgb(Int32 hueValue)
        {
            Int32 index = hueValue / 255;
            var section = rgbMapper[index];

            var colorValues = section.dominantColorPart switch
            {
                'r' => (_r, _g),
                'g' => (_g, _r),
                'b' => (_b, _r),
                _ => (255, 255)
            };

            var primaryDiff = 255 - colorValues.Item1;
            var primaryDiffDelta = colorValues.Item1 / 255.0;

            _selectorY = MathExtensions.Map(0, 255, 0, _maxY, primaryDiff);

            double secondaryColorX = colorValues.Item2 * (1.0 / primaryDiffDelta);
            double relation = (255 - secondaryColorX) / (255 - colorValues.Item2);

            _selectorX = relation * _maxX;
        }

        private void UpdateHLsValues()
        {
            var hlsColor = ColorTransformation.RgBtoHsl(MudColor.FromArgb((int)MathExtensions.Map(0.0, 1.0, 0, 255, _pickerAlpha), _r, _g, _b));
            _h = hlsColor.H;
            _l = hlsColor.L;
            _s = hlsColor.S;
        }

        private void UpdateColorPanel(bool updateHLS)
        {
            var hsl = ColorTransformation.RgBtoHsl(MudColor.FromArgb(_r, _g, _b));

            var numericValue = MathExtensions.Map(0, 360, 0, 6 * 255, hsl.H);

            UpdateBaseColor((int)numericValue, false);
            UpdateColorSelectorBasedOnRgb((int)numericValue);
            UpdateColorHexString();

            if (updateHLS == true)
            {
                UpdateHLsValues();
            }

            SetTextAsync(_rgbAHex, true).AndForget();
        }

        private void UpdateColorHexString() => _rgbAHex = $"#{_r:X2}{_g:X2}{_b:X2}{ (int)MathExtensions.Map(0.0, 1.0, 0, 255, _pickerAlpha):X2}";

        private void UpdateRGBValueFromHLS()
        {
            var rgbColor = ColorTransformation.HsLtoRgb(new ColorTransformation.HSLColor { H = _h, L = _l, S = _s }, (int)MathExtensions.Map(0.0, 1.0, 0, 255, _pickerAlpha));
            _r = rgbColor.R;
            _b = rgbColor.B;
            _g = rgbColor.G;
        }

        /// <summary>
        /// Sets Mouse Down bool to true if mouse is inside the color area.
        /// </summary>
        private void OnMouseDown(MouseEventArgs e)
        {
            _isMouseDown = true;
        }

        /// <summary>
        /// Sets Mouse Down bool to false if mouse is inside the color area.
        /// </summary>
        private void OnMouseUp(MouseEventArgs e)
        {
            _isMouseDown = false;
        }

        private void OnMouseClick(MouseEventArgs e)
        {
            _selectorX = e.OffsetX;
            _selectorY = e.OffsetY;
            UpdateColor(false);
        }

        private void OnMouseOver(MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                _selectorX = e.OffsetX;
                _selectorY = e.OffsetY;
                UpdateColor(false);
            }
        }

        private void OnMouseMove(MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                _selectorX = e.OffsetX;
                _selectorY = e.OffsetY;
                UpdateColor(false);
            }
        }

        private void RChangedManuell(int value)
        {
            _r = value;
            UpdateColorPanel(true);
        }

        private void GChangedManuell(int value)
        {
            _g = value;
            UpdateColorPanel(true);
        }

        private void BChangedManuell(int value)
        {
            _b = value;
            UpdateColorPanel(true);
        }

        private void HChangedManuell(double value)
        {
            _h = value;
            UpdateRGBValueFromHLS();
            UpdateColorPanel(false);
        }

        private void SChangedManuell(double value)
        {
            _s = value;
            UpdateRGBValueFromHLS();
            UpdateColorPanel(false);
        }

        private void LChangedManuell(double value)
        {
            _l = value;
            UpdateRGBValueFromHLS();
            UpdateColorPanel(false);
        }

        private void RgbaHexChanged(string input)
        {
            try
            {
                if (input.StartsWith("#"))
                {
                    input = input.Substring(1);
                }

                string parsedValue = input;
                switch (input.Length)
                {
                    case 3:
                        parsedValue = new string(new Char[8] { input[0], input[0], input[1], input[1], input[2], input[2], 'F', 'F' });
                        break;
                    case 4:
                        parsedValue = new string(new Char[8] { input[0], input[0], input[1], input[1], input[2], input[2], input[3], input[3] });
                        break;
                    case 6:
                        parsedValue += "FF";
                        break;
                    case 8:
                        break;
                    default:
                        return;
                }

                _r = parsedValue.GetByteValue(0);
                _g = parsedValue.GetByteValue(2);
                _b = parsedValue.GetByteValue(4);
                _pickerAlpha = MathExtensions.Map(0, 255, 0.0, 1.0, parsedValue.GetByteValue(6));
            }
            catch (Exception)
            {
                return;
            }

            UpdateColorPanel(true);
        }

        private void AlphaChanged(double input)
        {
            _pickerAlpha = input;
            UpdateColorHexString();
        }

        private string GetSelectorLocation() => $"translate({_selectorX}px, {_selectorY}px);";
    }
}
