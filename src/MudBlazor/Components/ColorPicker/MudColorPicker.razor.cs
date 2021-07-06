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
    public partial class MudColorPicker : MudPicker<System.Drawing.Color?>
    {
        private record RGBColor(int R, int G, int B);

        #region Fields

        private const double _maxY = 250;
        private const double _maxX = 300;

        private bool _isMouseDown;
        private double _selectorX;
        private double _selectorY;

        private int _pickerBaseColorValue;
        private double _pickerAlpha = 1;

        private RGBColor _baseColor = new(255, 0, 0);

        private Int32 _r = 255;
        private Int32 _g = 0;
        private Int32 _b = 0;

        #endregion

        #region Parameters

        #endregion


        /// <summary>
        /// If true, Alpha options will not be displayed.
        /// </summary>
        [Parameter] public bool DisableAlpha { get; set; }

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

        private void UpdateBaseColor(int input)
        {
            _pickerBaseColorValue = input;

            Int32 index = input / 255;
            Int32 value = input - (index * 255);

            Dictionary<Int32, (Func<int, int> r, Func<int, int> g, Func<int, int> b)> rgbMapper = new()
            {
                { 0, ((x) => 255, x => x, x => 0) },
                { 1, ((x) => 255 - x, x => 255, x => 0) },
                { 2, ((x) => 0, x => 255, x => x) },
                { 3, ((x) => 0, x => 255 - x, x => 255) },
                { 4, ((x) => x, x => 0, x => 255) },
                { 5, ((x) => 255, x => 0, x => 255 - x) },
            };

            var section = rgbMapper[index];

            _baseColor = new RGBColor(section.r(value), section.g(value), section.b(value));
            UpdateColor(false);
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

            if (fireStateHasChanged == true)
            {
                StateHasChanged();
            }
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

        private string GetSelectorLocation() => $"translate({_selectorX}px, {_selectorY}px);";
    }
}
