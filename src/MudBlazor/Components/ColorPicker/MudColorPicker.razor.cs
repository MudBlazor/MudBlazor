// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudColorPicker : MudPicker<System.Drawing.Color?>
    {
        /// <summary>
        /// If true, Alpha options will not be displayed.
        /// </summary>
        [Parameter] public bool DisableAlpha { get; set; }

        private double _pickerHue { get; set; }
        private double _pickerAlpha { get; set; } = 1;

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

        public void ChangeMode()
        {
            switch(ColorPickerMode)
            {
                case ColorPickerMode.RGB:
                    ColorPickerMode = ColorPickerMode.HSL;
                break;
                case ColorPickerMode.HSL:
                    ColorPickerMode = ColorPickerMode.HEX;
                    break;
                case ColorPickerMode.HEX:
                    ColorPickerMode = ColorPickerMode.RGB;
                    break;
                default:
                    ColorPickerMode = ColorPickerMode.RGB;
                    break;
            }
        }

        public bool MouseDown { get; set; }
        private double _selectorX { get; set; }
        private double _selectorY { get; set; }

        /// <summary>
        /// Sets Mouse Down bool to true if mouse is inside the color area.
        /// </summary>
        private void OnMouseDown(MouseEventArgs e)
        {
            MouseDown = true;
        }

        /// <summary>
        /// Sets Mouse Down bool to false if mouse is inside the color area.
        /// </summary>
        private void OnMouseUp(MouseEventArgs e)
        {
            MouseDown = false;
        }

        private void OnMouseClick(MouseEventArgs e)
        {
            _selectorX = e.OffsetX;
            _selectorY = e.OffsetY;
        }

        private void OnMouseOver(MouseEventArgs e)
        {
            if (MouseDown)
            {
                _selectorX = e.OffsetX;
                _selectorY = e.OffsetY;
            }
        }

        private void OnMouseMove(MouseEventArgs e)
        {
            if (MouseDown)
            {
                _selectorX = e.OffsetX;
                _selectorY = e.OffsetY;
            }
        }

        private string GetSelectorLocation()
        {
            return $"translate({_selectorX}px, {_selectorY}px);";
        }
    }
}
