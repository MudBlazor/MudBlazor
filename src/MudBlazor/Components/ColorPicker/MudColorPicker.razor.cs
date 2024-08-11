// Copyright (c) MudBlazor 2021
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
    /// <summary>
    /// Represents a sophisticated and customizable pop-up for choosing a color.
    /// </summary>
    public partial class MudColorPicker : MudPicker<MudColor>
    {
        private readonly ParameterState<int> _throttleIntervalState;

        public MudColorPicker() : base(new DefaultConverter<MudColor>())
        {
            AdornmentIcon = Icons.Material.Outlined.Palette;
            ShowToolbar = false;
            Value = "#594ae2"; // MudBlazor Blue
            Text = GetColorTextValue();
            AdornmentAriaLabel = "Open Color Picker";
            using var registerScope = CreateRegisterScope();
            _throttleIntervalState = registerScope.RegisterParameter<int>(nameof(ThrottleInterval))
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

        private readonly string _id = Identifier.Create();

        private ThrottleDispatcher _throttleDispatcher;

        #endregion

        #region Parameters

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        private bool _alpha = true;

        /// <summary>
        /// Shows alpha transparency options.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  When <c>true</c>, alpha options will be displayed and color output will be <c>RGBA</c>, <c>HSLA</c> or <c>HEXA</c> instead of <c>RGB</c>, <c>HSL</c> or <c>HEX</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool ShowAlpha
        {
            get => _alpha;
            set
            {
                if (value != _alpha)
                {
                    _alpha = value;

                    if (!_alpha)
                    {
                        Value = Value.SetAlpha(1.0);
                    }

                    Text = GetColorTextValue();
                }
            }
        }

        /// <summary>
        /// Displays the color field.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool ShowColorField { get; set; } = true;

        /// <summary>
        /// Displays the switch to change the color mode.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool ShowModeSwitch { get; set; } = true;

        /// <summary>
        /// Displays the text inputs, current mode, and mode switch.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool ShowInputs { get; set; } = true;

        /// <summary>
        /// Displays hue and alpha sliders.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool ShowSliders { get; set; } = true;

        /// <summary>
        /// Displays a preview of the color.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  When <c>true</c>, the preview color can be used as a button for collection colors.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool ShowPreview { get; set; } = true;

        /// <summary>
        /// The initial color channels shown.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="ColorPickerMode.RGB"/>.  Other values are <see cref="ColorPickerMode.HEX"/> for hexadecimal values and <see cref="ColorPickerMode.HSL"/> for hue/saturation/lightness mode.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public ColorPickerMode ColorPickerMode { get; set; } = ColorPickerMode.RGB;

        private ColorPickerView _colorPickerView = ColorPickerView.Spectrum;
        private ColorPickerView _activeColorPickerView = ColorPickerView.Spectrum;

        /// <summary>
        /// The initial view.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="ColorPickerView.Spectrum"/>.   The view can be changed if <c>ShowToolbar</c> is <c>true</c>.
        /// </remarks>
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
        /// Limits updates to the bound value to when HSL values change.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the bound value changes when HSL values change, even if the RGB values have not changed.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool UpdateBindingIfOnlyHSLChanged { get; set; } = false;

        /// <summary>
        /// The currently selected color as a <see cref="MudColor"/>.
        /// </summary>
        /// <remarks>
        /// You can use properties in <see cref="MudColor"/> to get color channel values such as <c>RGB</c>, <c>HSL</c>, <c>HEX</c> and more.  When this value changes, the <see cref="ValueChanged"/> event occurs.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public MudColor Value
        {
            get => _value;
            set => SetColorAsync(value).CatchAndLog();
        }

        /// <summary>
        /// Occurs when the <see cref="Value"/> property has changed.
        /// </summary>
        [Parameter]
        public EventCallback<MudColor> ValueChanged { get; set; }

        /// <summary>
        /// The list of quick colors to display.
        /// </summary>
        /// <remarks>
        /// Defaults to a list of <c>35</c> colors.  The first five colors show as the quick colors when the preview dot is clicked.
        /// </remarks>
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
        /// Continues to update the selected color while the mouse button is down.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  When <c>false</c>, conditions like long latency are better supported and can be adjusted via the <see cref="ThrottleInterval"/> property.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool DragEffect { get; set; } = true;

        /// <summary>
        /// The custom icon to dislay for the close button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Close"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string CloseIcon { get; set; } = Icons.Material.Filled.Close;

        /// <summary>
        /// The icon to display for the spectrum mode button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Tune"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string SpectrumIcon { get; set; } = Icons.Material.Filled.Tune;

        /// <summary>
        /// The icon to display for the grid mode button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Apps"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string GridIcon { get; set; } = Icons.Material.Filled.Apps;

        /// <summary>
        /// The icon to display for the custom palette button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Palette"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string PaletteIcon { get; set; } = Icons.Material.Filled.Palette;

        /// <summary>
        /// The icon to display for the import/export button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ImportExport"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string ImportExportIcon { get; set; } = Icons.Material.Filled.ImportExport;

        /// <summary>
        /// The delay, in milliseconds, between updates to the selected color when <see cref="DragEffect"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>300</c> milliseconds between updates.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int ThrottleInterval { get; set; } = 300;

        #endregion

        /// <inheritdoc />
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

        /// <summary>
        /// Refreshes the current color change mode.
        /// </summary>
        public void ChangeMode() =>
            ColorPickerMode = ColorPickerMode switch
            {
                ColorPickerMode.RGB => ColorPickerMode.HSL,
                ColorPickerMode.HSL => ColorPickerMode.HEX,
                ColorPickerMode.HEX => ColorPickerMode.RGB,
                _ => ColorPickerMode.RGB,
            };

        /// <summary>
        /// Changes to the specified color selection view.
        /// </summary>
        /// <param name="value">
        /// The new view to display.
        /// </param>
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
            if (e.Buttons == 1 && DragEffect)
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
        /// Sets the red channel of the selected color.
        /// </summary>
        /// <param name="value">
        /// A value between <c>0</c> (no red) and <c>255</c> (max red).
        /// </param>
        /// <remarks>
        /// Often used with <see cref="SetG(int)"/> and <see cref="SetB(int)"/>.
        /// </remarks>
        public void SetR(int value) => Value = Value.SetR(value);

        /// <summary>
        /// Sets the green channel of the selected color.
        /// </summary>
        /// <param name="value">
        /// A value between <c>0</c> (no green) and <c>255</c> (max green).
        /// </param>
        /// <remarks>
        /// Often used with <see cref="SetR(int)"/> and <see cref="SetB(int)"/>.
        /// </remarks>
        public void SetG(int value) => Value = Value.SetG(value);

        /// <summary>
        /// Sets the blue channel of the selected color.
        /// </summary>
        /// <param name="value">
        /// A value between <c>0</c> (no blue) and <c>255</c> (max blue).
        /// </param>
        /// <remarks>
        /// Often used with <see cref="SetR(int)"/> and <see cref="SetG(int)"/>.
        /// </remarks>
        public void SetB(int value) => Value = Value.SetB(value);

        /// <summary>
        /// Sets the hue channel of the selected color.
        /// </summary>
        /// <param name="value">
        /// A value between <c>0.0</c> and <c>360.0</c>, in degrees.
        /// </param>
        /// <remarks>
        /// Often used with <see cref="SetS(double)"/> and <see cref="SetL(double)"/>.
        /// </remarks>
        public void SetH(double value) => Value = Value.SetH(value);

        /// <summary>
        /// Sets the saturation channel of the selected color.
        /// </summary>
        /// <param name="value">
        /// A value between <c>0.0</c> (no saturation) and <c>1.0</c> (max saturation).
        /// </param>
        /// <remarks>
        /// Often used with <see cref="SetH(double)"/> and <see cref="SetL(double)"/>.
        /// </remarks>
        public void SetS(double value) => Value = Value.SetS(value);

        /// <summary>
        /// Sets the lightness channel of the selected color.
        /// </summary>
        /// <param name="value">
        /// A value between <c>0.0</c> (darkest/black) and <c>1.0</c> (brightest/white).
        /// </param>
        /// <remarks>
        /// Often used with <see cref="SetH(double)"/> and <see cref="SetS(double)"/>.
        /// </remarks>
        public void SetL(double value) => Value = Value.SetL(value);

        /// <summary>
        /// Sets the transparency channel of the selected color.
        /// </summary>
        /// <param name="value">
        /// A value between <c>0.0</c> (fully transparent) and <c>1.0</c> (solid).
        /// </param>
        public void SetAlpha(double value) => Value = Value.SetAlpha(value);

        /// <summary>
        /// Sets the transparency channel of the selected color.
        /// </summary>
        /// <param name="value">
        /// A value between <c>0</c> (fully transparent) and <c>1</c> (solid).</param>
        public void SetAlpha(int value) => Value = Value.SetAlpha(value);

        /// <summary>
        /// Sets the selected color to the specified value.
        /// </summary>
        /// <param name="input">
        /// A string value formatted as hexadecimal (<c>#FF0000</c>), RGB (<c>rgb(255,0,0)</c>), or RGBA (<c>rgba(255,0,0,255)</c>.
        /// </param>
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
        private string GetColorTextValue() => (!ShowAlpha || _activeColorPickerView is ColorPickerView.Palette or ColorPickerView.GridCompact) ? _value.ToString(MudColorOutputFormats.Hex) : _value.ToString(MudColorOutputFormats.HexA);
        private int GetHexColorInputMaxLength() => !ShowAlpha ? 7 : 9;

        private EventCallback<MouseEventArgs> GetEventCallback() => EventCallback.Factory.Create<MouseEventArgs>(this, () => CloseAsync());
        private bool IsAnyControlVisible() => ShowPreview || ShowSliders || ShowInputs;
        private EventCallback<MouseEventArgs> GetSelectPaletteColorCallback(MudColor color) => new EventCallbackFactory().Create(this, (MouseEventArgs _) => SelectPaletteColorAsync(color));

        private Color GetButtonColor(ColorPickerView view) => _activeColorPickerView == view ? Color.Primary : Color.Inherit;
        private string GetColorDotClass(MudColor color) => new CssBuilder("mud-picker-color-dot").AddClass("selected", color == Value).ToString();
        private string AlphaSliderStyle => new StyleBuilder().AddStyle($"background-image: linear-gradient(to {(RightToLeft ? "left" : "right")}, transparent, {_value.ToString(MudColorOutputFormats.RGB)})").Build();

        #endregion
    }
}
