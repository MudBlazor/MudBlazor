﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.State;
using MudBlazor.Utilities;
using MudBlazor.Utilities.Throttle;

namespace MudBlazor
{
    public partial class MudColorPicker : MudPicker<MudColor>
    {
        private readonly ParameterState<int> _throttleIntervalState;

        public MudColorPicker() : base(new DefaultConverter<MudColor>())
        {
            AdornmentIcon = Icons.Material.Outlined.Palette;
            DisableToolbar = true;
            Value = "#594ae2"; // MudBlazor Blue
            Text = GetColorTextValue();
            AdornmentAriaLabel = "Open Color Picker";
            _throttleIntervalState = RegisterParameterBuilder<int>(nameof(ThrottleInterval))
                .WithParameter(() => ThrottleInterval)
                .WithChangeHandler(OnThrottleIntervalParameterChanged);
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
        private const double _maxX = 312;
        private const double _selectorSize = 26.0;

        private double _selectorX;
        private double _selectorY;
        private bool _skipFeedback;

        private MudColor _baseColor;

        private bool _collectionOpen;

        private readonly Guid _id = Guid.NewGuid();

        private ThrottleDispatcher _throttleDispatcher;

        #endregion

        #region Parameters

        [CascadingParameter(Name = "RightToLeft")] public bool RightToLeft { get; set; }

        private bool _disableAlpha;

        /// <summary>
        /// If true, Alpha options will not be displayed and color output will be RGB, HSL or HEX and not RGBA, HSLA or HEXA.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool DisableAlpha
        {
            get => _disableAlpha;
            set
            {
                if (value != _disableAlpha)
                {
                    _disableAlpha = value;

                    if (value)
                    {
                        Value = Value.SetAlpha(1.0);
                    }

                    Text = GetColorTextValue();
                }
            }
        }

        /// <summary>
        /// If true, the color field will not be displayed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool DisableColorField { get; set; } = false;

        /// <summary>
        /// If true, the switch to change color mode will not be displayed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool DisableModeSwitch { get; set; } = false;

        /// <summary>
        /// If true, textfield inputs and color mode switch will not be displayed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool DisableInputs { get; set; } = false;

        /// <summary>
        /// If true, hue and alpha sliders will not be displayed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool DisableSliders { get; set; } = false;

        /// <summary>
        /// If true, the preview color box will not be displayed, note that the preview color functions as a button as well for collection colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool DisablePreview { get; set; } = false;

        /// <summary>
        /// The initial mode (RGB, HSL or HEX) the picker should open. Defaults to RGB
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public ColorPickerMode ColorPickerMode { get; set; } = ColorPickerMode.RGB;

        private ColorPickerView _colorPickerView = ColorPickerView.Spectrum;
        private ColorPickerView _activeColorPickerView = ColorPickerView.Spectrum;

        /// <summary>
        /// The initial view of the picker. Views can be changed if toolbar is enabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public ColorPickerView ColorPickerView
        {
            get => _colorPickerView;
            set
            {
                if (_colorPickerView != value)
                {
                    _colorPickerView = value;
                    ChangeView(value);
                }
            }
        }

        /// <summary>
        /// If true, binding changes occurred also when HSL values changed without a corresponding RGB change
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool UpdateBindingIfOnlyHSLChanged { get; set; } = false;

        /// <summary>
        /// A two-way bindable property representing the selected value. MudColor is a utility class that can be used to get the value as RGB, HSL, hex or other value
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public MudColor Value
        {
            get => _value;
            set => SetColorAsync(value).AndForget();
        }

        [Parameter] public EventCallback<MudColor> ValueChanged { get; set; }

        /// <summary>
        /// MudColor list of predefined colors. The first five colors will show up as the quick colors on preview dot click.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public IEnumerable<MudColor> Palette { get; set; } = new MudColor[]
        { "#424242", "#2196f3", "#00c853", "#ff9800", "#f44336",
          "#f6f9fb", "#9df1fa", "#bdffcf", "#fff0a3", "#ffd254",
          "#e6e9eb", "#27dbf5", "#7ef7a0", "#ffe273", "#ffb31f",
          "#c9cccf", "#13b8e8", "#14dc71", "#fdd22f", "#ff9102",
          "#858791", "#0989c2", "#1bbd66", "#ebb323", "#fe6800",
          "#585b62", "#17698e", "#17a258", "#d9980d", "#dc3f11",
          "#353940", "#113b53", "#127942", "#bf7d11", "#aa0000"
        };

        private IEnumerable<MudColor> _gridList = new MudColor[]
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

        /// <summary>
        /// <para>
        /// When set to true, no mouse move events in the spectrum mode will be captured, so the selector circle won't fellow the mouse.
        /// Under some conditions like long latency the visual representation might not reflect the user behaviour anymore. So, it can be disabled.
        /// </para>
        /// <para>Enabled by default</para>
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool DisableDragEffect { get; set; } = false;

        /// <summary>
        /// Custom close icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string CloseIcon { get; set; } = Icons.Material.Filled.Close;

        /// <summary>
        /// Custom spectrum icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string SpectrumIcon { get; set; } = Icons.Material.Filled.Tune;

        /// <summary>
        /// Custom grid icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string GridIcon { get; set; } = Icons.Material.Filled.Apps;

        /// <summary>
        /// Custom palette icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string PaletteIcon { get; set; } = Icons.Material.Filled.Palette;

        /// <summary>
        /// Custom import/export icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string ImportExportIcon { get; set; } = Icons.Material.Filled.ImportExport;

        /// <summary>
        /// <para>The delay (in milliseconds) after dragging the pointer before the color binding updates.</para>
        /// <para>Updates are instant if the throttling interval is <c>0</c>.</para>
        /// <para>Default interval is <c>300ms</c>.</para>
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int ThrottleInterval { get; set; } = 300;

        #endregion

        protected override void OnInitialized()
        {
            base.OnInitialized();
            SetThrottle(_throttleIntervalState.Value);
        }

        private void OnThrottleIntervalParameterChanged(ParameterChangedEventArgs<int> args)
        {
            SetThrottle(args.Value);
        }

        private void SetThrottle(int interval)
        {
            _throttleDispatcher = interval > 0
                ? new ThrottleDispatcher(interval)
                : null;
        }

        private void ToggleCollection()
        {
            _collectionOpen = !_collectionOpen;
        }

        private async Task SelectPaletteColorAsync(MudColor color)
        {
            Value = color;
            _collectionOpen = false;

            if (
                IsAnyControlVisible() == false || _activeColorPickerView is ColorPickerView.GridCompact or ColorPickerView.Palette)
            {
                await CloseAsync();
            }
        }

        public void ChangeMode() =>
            ColorPickerMode = ColorPickerMode switch
            {
                ColorPickerMode.RGB => ColorPickerMode.HSL,
                ColorPickerMode.HSL => ColorPickerMode.HEX,
                ColorPickerMode.HEX => ColorPickerMode.RGB,
                _ => ColorPickerMode.RGB,
            };

        public void ChangeView(ColorPickerView value)
        {
            _activeColorPickerView = value;
            Text = GetColorTextValue();
        }

        private async Task SetColorAsync(MudColor value)
        {
            if (value == null)
            {
                return;
            }

            var rgbChanged = value != _value;
            var hslChanged = _value != null && value.HslChanged(_value);
            var shouldUpdateBinding = _value != null
                                      && (rgbChanged || (UpdateBindingIfOnlyHSLChanged && hslChanged));
            _value = value;

            if (rgbChanged && _skipFeedback == false)
            {
                UpdateBaseColor();
                UpdateColorSelectorBasedOnRgb();
            }

            if (shouldUpdateBinding)
            {
                Touched = true;
                await SetTextAsync(GetColorTextValue(), false);
                await ValueChanged.InvokeAsync(value);
                await BeginValidateAsync();
                FieldChanged(value);
            }
        }

        private void UpdateBaseColorSlider(int value)
        {
            var diff = Math.Abs(value - (int)Value.H);
            if (diff == 0)
            {
                return;
            }

            Value = Value.SetH(value);
        }

        private void UpdateBaseColor()
        {
            var index = (int)_value.H / 60;
            if (index == 6)
            {
                index = 5;
            }

            var valueInDeg = (int)_value.H - (index * 60);
            var value = (int)MathExtensions.Map(0, 60, 0, 255, valueInDeg);
            var (r, g, b, dominantColorPart) = _rgbToHueMapper[index];

            _baseColor = new(r(value), g(value), b(value), 255);
        }

        private void UpdateColorBaseOnSelection()
        {
            var x = _selectorX / _maxX;

            var r_x = 255 - (int)((255 - _baseColor.R) * x);
            var g_x = 255 - (int)((255 - _baseColor.G) * x);
            var b_x = 255 - (int)((255 - _baseColor.B) * x);

            var y = 1.0 - (_selectorY / _maxY);

            var r = r_x * y;
            var g = g_x * y;
            var b = b_x * y;

            _skipFeedback = true;
            //in this mode, H is expected to be stable, so copy H value
            Value = new MudColor((byte)r, (byte)g, (byte)b, _value);
            _skipFeedback = false;
        }

        private void UpdateColorSelectorBasedOnRgb()
        {
            var hueValue = (int)MathExtensions.Map(0, 360, 0, 6 * 255, _value.H);
            var index = hueValue / 255;
            if (index == 6)
            {
                index = 5;
            }

            var (r, g, b, dominantColorPart) = _rgbToHueMapper[index];

            var colorValues = dominantColorPart switch
            {
                "rb" => (_value.R, _value.B),
                "rg" => (_value.R, _value.G),
                "gb" => (_value.G, _value.B),
                "gr" => (_value.G, _value.R),
                "br" => (_value.B, _value.R),
                "bg" => (_value.B, _value.G),
                _ => (255, 255)
            };

            var primaryDiff = 255 - colorValues.Item1;
            var primaryDiffDelta = colorValues.Item1 / 255.0;

            _selectorY = MathExtensions.Map(0, 255, 0, _maxY, primaryDiff);

            var secondaryColorX = colorValues.Item2 * (1.0 / primaryDiffDelta);
            var relation = (255 - secondaryColorX) / 255.0;

            _selectorX = relation * _maxX;
        }

        #region mouse interactions

        private async Task HandleColorOverlayClickedAsync()
        {
            UpdateColorBaseOnSelection();

            if (IsAnyControlVisible() == false)
            {
                await CloseAsync();
            }
        }

        private Task OnColorOverlayClick(PointerEventArgs e)
        {
            SetSelectorBasedOnPointerEvents(e, true);

            return HandleColorOverlayClickedAsync();
        }

        private async Task OnPointerMoveAsync(PointerEventArgs e)
        {
            if (e.Buttons == 1 && !DisableDragEffect)
            {
                SetSelectorBasedOnPointerEvents(e, true);

                if (_throttleDispatcher is null)
                {
                    // Update instantly because debounce is not enabled.
                    UpdateColorBaseOnSelection();
                }
                else
                {
                    await _throttleDispatcher.ThrottleAsync(() => InvokeAsync(UpdateColorBaseOnSelection));
                }
            }
        }

        private void SetSelectorBasedOnPointerEvents(PointerEventArgs e, bool offsetIsAbsolute)
        {
            _selectorX = (offsetIsAbsolute ? e.OffsetX : e.OffsetX - (_selectorSize / 2.0) + _selectorX).EnsureRange(_maxX);
            _selectorY = (offsetIsAbsolute ? e.OffsetY : e.OffsetY - (_selectorSize / 2.0) + _selectorY).EnsureRange(_maxY);
        }

        #endregion

        #region updating inputs

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
        /// Set the B (blue) component of the color picker
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
        /// <param name="value">A value between 0.0 (no light, black) and 1.0 (max light, white)</param>
        public void SetL(double value) => Value = Value.SetL(value);

        /// <summary>
        /// Set the Alpha (transparency) component of the color picker
        /// </summary>
        /// <param name="value">A value between 0.0 (full transparent) and 1.0 (solid) </param>
        public void SetAlpha(double value) => Value = Value.SetAlpha(value);

        /// <summary>
        /// Set the Alpha (transparency) component of the color picker
        /// </summary>
        /// <param name="value">A value between 0 (full transparent) and 1 (solid) </param>
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

        protected override Task StringValueChangedAsync(string value)
        {
            SetInputString(value);
            return Task.CompletedTask;
        }

        #endregion

        #region helper

        private string GetSelectorLocation() => $"translate({Math.Round(_selectorX, 2).ToString(CultureInfo.InvariantCulture)}px, {Math.Round(_selectorY, 2).ToString(CultureInfo.InvariantCulture)}px);";
        private string GetColorTextValue() => (DisableAlpha || _activeColorPickerView is ColorPickerView.Palette or ColorPickerView.GridCompact) ? _value.ToString(MudColorOutputFormats.Hex) : _value.ToString(MudColorOutputFormats.HexA);
        private int GetHexColorInputMaxLength() => DisableAlpha ? 7 : 9;

        private EventCallback<MouseEventArgs> GetEventCallback() => EventCallback.Factory.Create<MouseEventArgs>(this, () => CloseAsync());
        private bool IsAnyControlVisible() => !(DisablePreview && DisableSliders && DisableInputs);
        private EventCallback<MouseEventArgs> GetSelectPaletteColorCallback(MudColor color) => new EventCallbackFactory().Create(this, (MouseEventArgs _) => SelectPaletteColorAsync(color));

        private Color GetButtonColor(ColorPickerView view) => _activeColorPickerView == view ? Color.Primary : Color.Inherit;
        private string GetColorDotClass(MudColor color) => new CssBuilder("mud-picker-color-dot").AddClass("selected", color == Value).ToString();
        private string AlphaSliderStyle => new StyleBuilder().AddStyle($"background-image: linear-gradient(to {(RightToLeft ? "left" : "right")}, transparent, {_value.ToString(MudColorOutputFormats.RGB)})").Build();

        #endregion
    }
}
