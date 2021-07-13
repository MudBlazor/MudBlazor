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

namespace MudBlazor
{
    public partial class MudColorPicker : MudPicker<MudColor>
    {
        public MudColorPicker() : base(new DefaultConverter<MudColor>())
        {
            AdornmentIcon = Icons.Material.Outlined.Palette;
            Value = "#594ae2"; //MudBlazor Blue
        }

        #region Fields

        private static Dictionary<int, (Func<int, int> r, Func<int, int> g, Func<int, int> b, string dominantColorPart)> _rgbToHueMapper = new()
        {
            { 0, ((x) => 255, x => x, x => 0, "rb") },
            { 1, ((x) => 255 - x, x => 255, x => 0, "gb") },
            { 2, ((x) => 0, x => 255, x => x, "gr") },
            { 3, ((x) => 0, x => 255 - x, x => 255, "br") },
            { 4, ((x) => x, x => 0, x => 255, "bg") },
            { 5, ((x) => 255, x => 0, x => 255 - x, "rg") },
        };

        private const double _maxY = 250;
        private const double _maxX = 310;

        private bool _isMouseDown;
        private double _selectorX;
        private double _selectorY;
        private bool _skipFeedback = false;

        private MudColor _baseColor;
        private MudColor _color;

        #endregion

        #region Parameters

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

        /// <summary>
        /// The inital mode (RGB, HSL or HEX) the picker should open. Defaults to RGB 
        /// </summary>
        [Parameter] public ColorPickerMode ColorPickerMode { get; set; } = ColorPickerMode.RGB;

        /// <summary>
        /// If true, binding changes occure also when HSL values changed without a corresponding RGB change 
        /// </summary>
        [Parameter] public bool AlwaysUpdateBinding { get; set; } = false;

        /// <summary>
        /// A two-way bindable property representing the selected value. MudColor is a utility class that can be used to get the value as RGB, HSL, hex or other value
        /// </summary>
        [Parameter]
        public MudColor Value
        {
            get => _color;
            set
            {
                bool changed = value != _color;
                _color = value;

                if (changed)
                {
                    if (_skipFeedback == false)
                    {
                        UpdateBaseColor();
                        UpdateColorSelectorBasedOnRgb();
                    }

                    SetTextAsync(_color.Value, true).AndForget();
                }

                if (changed == false && AlwaysUpdateBinding)
                {
                    ColorChanged.InvokeAsync(value).AndForget();
                }
            }
        }

        [Parameter] public EventCallback<MudColor> ColorChanged { get; set; }

        #endregion

        public void ChangeMode() =>
            ColorPickerMode = ColorPickerMode switch
            {
                ColorPickerMode.RGB => ColorPickerMode.HSL,
                ColorPickerMode.HSL => ColorPickerMode.HEX,
                ColorPickerMode.HEX => ColorPickerMode.RGB,
                _ => ColorPickerMode.RGB,
            };

        private void UpdateBaseColorSlider(int value) => Value = Value.SetH(MathExtensions.Map(0, 6 * 255, 0, 360, value));

        private void UpdateBaseColor()
        {
            var hueValue = GetSliderHueValue();

            int index = hueValue / 255;
            int value = hueValue - (index * 255);

            var section = _rgbToHueMapper[index];

            _baseColor = new(section.r(value), section.g(value), section.b(value), 255);
        }

        private int GetSliderHueValue() => (int)MathExtensions.Map(0, 360, 0, 6 * 255, _color.H);

        private void UpdateColorBaseOnSelection()
        {
            double x = _selectorX / _maxX;

            int r_x = 255 - (int)((255 - _baseColor.R) * x);
            int g_x = 255 - (int)((255 - _baseColor.G) * x);
            int b_x = 255 - (int)((255 - _baseColor.B) * x);

            double y = 1.0 - _selectorY / _maxY;

            double r = r_x * y;
            double g = g_x * y;
            double b = b_x * y;

            _skipFeedback = true;
            Value = new MudColor((byte)r, (byte)g, (byte)b, _color.A);
            _skipFeedback = false;
        }

        private void UpdateColorSelectorBasedOnRgb()
        {
            var hueValue = (int)MathExtensions.Map(0, 360, 0, 6 * 255, _color.H);
            int index = hueValue / 255;
            var section = _rgbToHueMapper[index];

            var colorValues = section.dominantColorPart switch
            {
                "rb" => (_color.R, _color.B),
                "rg" => (_color.R, _color.G),
                "gb" => (_color.G, _color.B),
                "gr" => (_color.G, _color.R),
                "br" => (_color.B, _color.R),
                "bg" => (_color.B, _color.G),
                _ => (255, 255)
            };

            var primaryDiff = 255 - colorValues.Item1;
            var primaryDiffDelta = colorValues.Item1 / 255.0;

            _selectorY = MathExtensions.Map(0, 255, 0, _maxY, primaryDiff);

            double secondaryColorX = colorValues.Item2 * (1.0 / primaryDiffDelta);
            double relation = (255 - secondaryColorX) / 255.0;

            _selectorX = relation * _maxX;
        }

        private void OnMouseDown(MouseEventArgs e)
        {
            _isMouseDown = true;
        }

        private void OnMouseUp(MouseEventArgs e)
        {
            _isMouseDown = false;
        }

        private void OnMouseClick(MouseEventArgs e)
        {
            _selectorX = e.OffsetX;
            _selectorY = e.OffsetY;
            UpdateColorBaseOnSelection();
        }

        private void OnMouseOver(MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                _selectorX = e.OffsetX;
                _selectorY = e.OffsetY;
                UpdateColorBaseOnSelection();
            }
        }

        private void OnMouseMove(MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                _selectorX = e.OffsetX;
                _selectorY = e.OffsetY;
                UpdateColorBaseOnSelection();
            }
        }

        /// <summary>
        /// Set the R (red) component of the color picker
        /// </summary>
        /// <param name="value">A value between 0 (no red) or 255 (max red)</param>
        public void SetR(int value) => Value = Value.SetR(value);

        /// <summary>
        /// Set the G (green) component of the color picker
        /// </summary>
        /// <param name="value">A value between 0 (no green) or 255 (max green)</param>
        public void SetG(int value) => Value = Value.SetG(value);

        /// <summary>
        /// Set the B (green) component of the color picker
        /// </summary>
        /// <param name="value">A value between 0 (no blue) or 255 (max blue)</param>
        public void SetB(int value) => Value = Value.SetB(value);

        /// <summary>
        /// Set the H (hue) component of the color picker
        /// </summary>
        /// <param name="value">A value between 0 and 360 (degrees)</param>
        public void SetH(double value) => Value = Value.SetH(value);

        /// <summary>
        /// Set the S (saturation) component of the color picker
        /// </summary>
        /// <param name="value">A value between 0.0 (no saturation) and 1.0 (max saturation)</param>
        public void SetS(double value) => Value = Value.SetS(value);

        /// <summary>
        /// Set the L (Lightness) component of the color picker
        /// </summary>
        /// <param name="value">A value between 0.0 (no light, black) and 1.0 (max ligt, white)</param>
        public void SetL(double value) => Value = Value.SetL(value);

        /// <summary>
        /// Set the Alpha (transparency) component of the color picker
        /// </summary>
        /// <param name="value">A value between 0.0 (full transparent) and 1.0 (solid) </param>
        public void SetAlpha(double value) => Value = Value.SetAlpha(value);

        /// <summary>
        /// Set the Alpha (transparency) component of the color picker
        /// </summary>
        /// <param name="value">A value between 0 (full transparent) and 255 (solid) </param>
        public void SetAlpha(int value) => Value = Value.SetAlpha(value);

        /// <summary>
        /// Set the color of the picker based on the string input
        /// </summary>
        /// <param name="input">Accepting different formats for a color representation such as rbg, rgba, #</param>
        public void SetInputString(string input)
        {
            MudColor color;
            try
            {
                color = new MudColor(input);
            }
            catch (Exception)
            {
                return;
            }

            Value = color;
        }

        private string GetSelectorLocation() => $"translate({_selectorX.ToString(CultureInfo.InvariantCulture)}px, {_selectorY.ToString(CultureInfo.InvariantCulture)}px);";
    }
}
