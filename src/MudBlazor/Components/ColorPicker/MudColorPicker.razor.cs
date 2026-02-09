// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
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
        private const double MaxY = 250;
        private const double MaxX = 312;
        private const double SelectorSize = 26.0;

        private double _selectorX;
        private double _selectorY;
        private bool _skipFeedback;
        private MudColor? _lastColor;
        private MudColor? _baseColor;
        private bool _collectionOpen;
        private readonly string _id = Identifier.Create();
        private ThrottleDispatcher? _throttleDispatcher;
        private readonly ParameterState<bool> _alphaState;
        private readonly ParameterState<string?> _textState;
        private readonly ParameterState<MudColor?> _valueState;
        private readonly ParameterState<int> _throttleIntervalState;
        private readonly ParameterState<ColorPickerView> _colorPickerViewState;
        private int _inputResetKey = 0; // Used to force TextField re-render on invalid input

        private readonly IReadOnlyList<MudColor> _gridList = new MudColor[]
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

        private readonly MudColor _defaultColor = "#594ae2";

        [Inject]
        private TimeProvider TimeProvider { get; set; } = null!;

        public MudColorPicker()
        {
            AdornmentIcon = Icons.Material.Outlined.Palette;
            ShowToolbar = false;

            using var registerScope = CreateRegisterScope();
            _valueState = registerScope.RegisterParameter<MudColor?>(nameof(Value))
                .WithParameter(() => Value)
                .WithEventCallback(() => ValueChanged)
                .WithChangeHandler(OnValueChangeHandlerAsync)
                .WithComparer(MudColor.MudColorComparer.RgbaAndHsl);
            _textState = registerScope.RegisterParameter<string?>(nameof(Text))
                .WithParameter(() => Text)
                .WithEventCallback(() => TextChanged);
            _colorPickerViewState = registerScope.RegisterParameter<ColorPickerView>(nameof(ColorPickerView))
                .WithParameter(() => ColorPickerView);
            _alphaState = registerScope.RegisterParameter<bool>(nameof(ShowAlpha))
                .WithParameter(() => ShowAlpha)
                .WithChangeHandler(OnAlphaChangeHandlerAsync);
            _throttleIntervalState = registerScope.RegisterParameter<int>(nameof(ThrottleInterval))
                .WithParameter(() => ThrottleInterval)
                .WithChangeHandler(OnThrottleIntervalParameterChanged);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            var workingColor = _lastColor = ValueOrDefault; // initialize color picker with Value or default
            _baseColor = UpdateBaseColor(workingColor);
            var (x, y) = UpdateColorSelectorBasedOnRgb(workingColor);
            _selectorX = x;
            _selectorY = y;
        }

        private Task OnValueChangeHandlerAsync(ParameterChangedEventArgs<MudColor?> args)
        {
            // TODO: Revisit this when the state of input components / validation improves, for now mimic old behavior
            var forceUpdate = _valueState.IsInitialized && HasRendered;
            return SetColorAsync(args.Value, forceUpdate);
        }

        private async Task OnAlphaChangeHandlerAsync(ParameterChangedEventArgs<bool> args)
        {
            if (_valueState.Value is null)
            {
                return;
            }
            // TODO: To be refactored, for now we replicate old behavior that was without ParameterState
            if (!args.Value)
            {
                var colorWithoutAlpha = ValueOrDefault.SetAlpha(1.0);
                await _textState.SetValueAsync(GetColorTextValue(colorWithoutAlpha));
                if (!ValueChanged.HasDelegate)
                {
                    await SetColorAsync(colorWithoutAlpha);
                }
            }
            else
            {
                await _textState.SetValueAsync(GetColorTextValue(ValueOrDefault));
            }
        }

        private static Dictionary<int, (Func<int, int> r, Func<int, int> g, Func<int, int> b, string dominantColorPart)> _rgbToHueMapper = new()
        {
            { 0, ((x) => 255, x => x, x => 0, "rb") },
            { 1, ((x) => 255 - x, x => 255, x => 0, "gb") },
            { 2, ((x) => 0, x => 255, x => x, "gr") },
            { 3, ((x) => 0, x => 255 - x, x => 255, "br") },
            { 4, ((x) => x, x => 0, x => 255, "bg") },
            { 5, ((x) => 255, x => 0, x => 255 - x, "rg") },
        };

        /// <summary>
        /// Displays this color picker right-to-left.
        /// </summary>
        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Shows alpha transparency options.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  When <c>true</c>, alpha options will be displayed and color output will be <c>RGBA</c>, <c>HSLA</c> or <c>HEXA</c> instead of <c>RGB</c>, <c>HSL</c> or <c>HEX</c>.
        /// </remarks>
        [Parameter, ParameterState]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool ShowAlpha { get; set; } = true;

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

        /// <summary>
        /// The initial view.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="ColorPickerView.Spectrum"/>.   The view can be changed if <c>ShowToolbar</c> is <c>true</c>.
        /// </remarks>
        [Parameter, ParameterState]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public ColorPickerView ColorPickerView { get; set; } = ColorPickerView.Spectrum;

        /// <summary>
        /// Limits updates to the bound value to when HSL values change.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, the bound value changes when HSL values change, even if the RGB values have not changed.
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
        [Parameter, ParameterState]
        [Category(CategoryTypes.FormComponent.Data)]
        public MudColor? Value { get; set; }

        /// <summary>
        /// The currently selected value, as a string.
        /// </summary>
        [Parameter, ParameterState]
        [Category(CategoryTypes.FormComponent.Data)]
        public override string? Text { get; set; }

        /// <summary>
        /// Occurs when the <see cref="Value"/> property has changed.
        /// </summary>
        [Parameter]
        public EventCallback<MudColor?> ValueChanged { get; set; }

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
        /// The custom icon to display for the close button.
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
        /// Defaults to <c>50</c> milliseconds between updates.
        /// </remarks>
        [Parameter, ParameterState]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public int ThrottleInterval { get; set; } = 50;

        /// <summary>
        /// Enables tooltips for icon buttons.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public bool ShowTooltips { get; set; } = true;

        /// <inheritdoc />
        protected override void OnInitialized()
        {
            base.OnInitialized();
            SetThrottle(_throttleIntervalState.Value);
            AdornmentAriaLabel ??= Localizer[Resources.LanguageResource.MudColorPicker_Open];
        }

        private void OnThrottleIntervalParameterChanged(ParameterChangedEventArgs<int> args) => SetThrottle(args.Value);

        private void SetThrottle(int interval)
        {
            _throttleDispatcher?.Dispose();
            _throttleDispatcher = interval > 0
                ? new ThrottleDispatcher(interval, TimeProvider)
                : null;
        }

        private void ToggleCollection() => _collectionOpen = !_collectionOpen;

        private async Task SelectPaletteColorAsync(MudColor color)
        {
            await SetColorAsync(color);
            _collectionOpen = false;

            if (!IsAnyControlVisible() || _colorPickerViewState.Value is ColorPickerView.GridCompact or ColorPickerView.Palette)
            {
                await CloseAsync();
            }
        }

        /// <summary>
        /// Refreshes the current color change mode.
        /// </summary>
        public void ChangeMode()
        {
            ColorPickerMode = ColorPickerMode switch
            {
                ColorPickerMode.RGB => ColorPickerMode.HSL,
                ColorPickerMode.HSL => ColorPickerMode.HEX,
                ColorPickerMode.HEX => ColorPickerMode.RGB,
                _ => ColorPickerMode.RGB,
            };
        }

        /// <summary>
        /// Changes to the specified color selection view.
        /// </summary>
        /// <param name="value">
        /// The new view to display.
        /// </param>
        public Task ChangeViewAsync(ColorPickerView value)
        {
            return _colorPickerViewState.SetValueAsync(value);
        }

        /// <inheritdoc />
        protected override IConverter<MudColor?, string?> GetDefaultConverter()
        {
            return new DefaultConverter<MudColor>
            {
                Culture = GetCulture,
                Format = GetFormat
            };
        }

        private async Task SetColorAsync(MudColor? newColor, bool forceUpdate = false)
        {
            var rgbChanged = newColor is null || !newColor.Equals(_valueState.Value);
            var hslChanged = newColor is null || !newColor.HslEquals(_valueState.Value);
            var colorChanged = rgbChanged || hslChanged;
            var shouldUpdateBinding = rgbChanged || (UpdateBindingIfOnlyHSLChanged && hslChanged);

            //if color is cleared, keep _baseColor so that the picker uses the last value
            if (newColor is not null && colorChanged)
            {
                _lastColor = newColor;
                if (!_skipFeedback)
                {
                    _baseColor = UpdateBaseColor(newColor);
                    var (x, y) = UpdateColorSelectorBasedOnRgb(newColor);
                    _selectorX = x;
                    _selectorY = y;
                }
            }

            if (shouldUpdateBinding || forceUpdate)
            {
                Touched = true;
                await SetTextAsync(GetColorTextValue(newColor), false);
                await _valueState.SetValueAsync(newColor);
                await BeginValidateAsync();
                FieldChanged(newColor);
            }
            else if (colorChanged)
            {
                await SetTextAsync(GetColorTextValue(newColor), false);
                await _valueState.SetValueAsync(newColor);
            }
            else
            {
                var colorText = GetColorTextValue(newColor);
                await SetTextAsync(colorText, false);
            }
        }

        protected override async Task SetTextAsync(string? value, bool callback)
        {
            if (callback)
            {
                await StringValueChangedAsync(value);
            }
            await _textState.SetValueAsync(value);
        }

        protected override string? ReadText => GetColorTextValue(_valueState.Value);

        protected override Task WriteTextAsync(string? value) => SetInputStringAsync(value);

        protected internal override MudColor? ReadValue => _valueState.Value;

        protected override Task SetValueCoreAsync(MudColor? value) => SetColorAsync(value);

        protected override Task StringValueChangedAsync(string? value) => SetInputStringAsync(value);

        private Task UpdateBaseColorSliderAsync(int value)
        {
            var diff = Math.Abs(value - (int)ReadHue);
            if (diff == 0)
            {
                return Task.CompletedTask;
            }

            return SetHueAsync(value);
        }

        private static MudColor UpdateBaseColor(MudColor newColor)
        {
            var index = (int)newColor.H / 60;
            if (index == 6)
            {
                index = 5;
            }

            var valueInDeg = (int)newColor.H - (index * 60);
            var value = (int)MathExtensions.Map(0, 60, 0, 255, valueInDeg);
            var (r, g, b, _) = _rgbToHueMapper[index];
            var newBaseColor = new MudColor(r(value), g(value), b(value), 255);
            return newBaseColor;
        }

        private async Task UpdateColorBaseOnSelectionAsync()
        {
            //if underlying value is null, initialize color selector
            _baseColor ??= ValueOrDefault;

            var x = _selectorX / MaxX;
            var rX = 255 - (int)((255 - _baseColor.R) * x);
            var gX = 255 - (int)((255 - _baseColor.G) * x);
            var bX = 255 - (int)((255 - _baseColor.B) * x);

            var y = 1.0 - _selectorY / MaxY;

            var r = rX * y;
            var g = gX * y;
            var b = bX * y;

            _skipFeedback = true;

            //in this mode, H is expected to be stable, so copy H value
            //if null, reuse existing hue
            var newColor = new MudColor((byte)r, (byte)g, (byte)b, _valueState.Value ?? _baseColor);
            await SetColorAsync(newColor);

            _skipFeedback = false;
        }

        private static (double x, double y) UpdateColorSelectorBasedOnRgb(MudColor newColor)
        {
            var hueValue = (int)MathExtensions.Map(0, 360, 0, 6 * 255, newColor.H);
            var index = hueValue / 255;
            if (index == 6)
            {
                index = 5;
            }

            var (_, _, _, dominantColorPart) = _rgbToHueMapper[index];

            var colorValues = dominantColorPart switch
            {
                "rb" => (newColor.R, newColor.B),
                "rg" => (newColor.R, newColor.G),
                "gb" => (newColor.G, newColor.B),
                "gr" => (newColor.G, newColor.R),
                "br" => (newColor.B, newColor.R),
                "bg" => (newColor.B, newColor.G),
                _ => (255, 255)
            };

            var primaryDiff = 255 - colorValues.Item1;
            var primaryDiffDelta = colorValues.Item1 / 255.0;

            var selectorY = MathExtensions.Map(0, 255, 0, MaxY, primaryDiff);

            var secondaryColorX = colorValues.Item2 * (1.0 / primaryDiffDelta);
            var relation = (255 - secondaryColorX) / 255.0;

            var selectorX = relation * MaxX;

            return (selectorX, selectorY);
        }
        private async Task HandleColorOverlayClickedAsync()
        {
            await UpdateColorBaseOnSelectionAsync();

            if (!IsAnyControlVisible())
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
                    await UpdateColorBaseOnSelectionAsync();
                }
                else
                {
                    await _throttleDispatcher.ThrottleAsync(() => InvokeAsync(UpdateColorBaseOnSelectionAsync));
                }
            }
        }

        private Task OnPointerLeaveAsync(PointerEventArgs e)
        {
            // Flush the final color update when the pointer leaves during a drag,
            // since pointermove/pointerup won't fire on this element anymore.
            if (e.Buttons == 1 && DragEffect)
            {
                return UpdateColorBaseOnSelectionAsync();
            }

            return Task.CompletedTask;
        }

        private void SetSelectorBasedOnPointerEvents(PointerEventArgs e, bool offsetIsAbsolute)
        {
            _selectorX = (offsetIsAbsolute ? e.OffsetX : e.OffsetX - (SelectorSize / 2.0) + _selectorX).EnsureRange(MaxX);
            _selectorY = (offsetIsAbsolute ? e.OffsetY : e.OffsetY - (SelectorSize / 2.0) + _selectorY).EnsureRange(MaxY);
        }

        /// <summary>
        /// Gets the current value, or if null returns the last valid value.
        /// Defaults to <see cref="_defaultColor"/>.
        /// </summary>
        private MudColor ValueOrDefault => _valueState.Value ?? _lastColor ?? _defaultColor;

        private int ReadRed => ValueOrDefault.R;

        private int ReadGreen => ValueOrDefault.G;

        private int ReadBlue => ValueOrDefault.B;

        private int ReadAlpha => ValueOrDefault.A;

        private double ReadAlphaPercentage => ValueOrDefault.APercentage;

        private Task SetRedAsync(int value) => SetColorAsync(ValueOrDefault.SetR(value));

        private Task SetGreenAsync(int value) => SetColorAsync(ValueOrDefault.SetG(value));

        private Task SetBlueAsync(int value) => SetColorAsync(ValueOrDefault.SetB(value));

        private Task SetAlphaAsync(int value) => SetColorAsync(ValueOrDefault.SetAlpha(value));

        private Task SetAlphaAsync(double value) => SetColorAsync(ValueOrDefault.SetAlpha(value));

        private double ReadHue => ValueOrDefault.H;

        private int ReadHueInt => (int)ReadHue;

        private double ReadSaturation => ValueOrDefault.S;

        private double ReadLightness => ValueOrDefault.L;

        private Task SetHueAsync(double value) => SetColorAsync(ValueOrDefault.SetH(value));

        private Task SetSaturationAsync(double value) => SetColorAsync(ValueOrDefault.SetS(value));

        private Task SetLightnessAsync(double value) => SetColorAsync(ValueOrDefault.SetL(value));

        /// <summary>
        /// Sets the selected color to the specified value.
        /// </summary>
        /// <param name="input">
        /// A string value formatted as hexadecimal (<c>#FF0000</c>), RGB (<c>rgb(255,0,0)</c>), or RGBA (<c>rgba(255,0,0,255)</c>.
        /// </param>
        private async Task SetInputStringAsync(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                await SetColorAsync(null);
            }
            else if (MudColor.TryParse(input, out var result))
            {
                await SetColorAsync(result);
            }
            else
            {
                // If parsing fails, we need to force the TextField to reset its display
                // Increment the key to force Blazor to recreate the TextField component
                // This ensures it resets to show the valid color value
                _inputResetKey++;
                await _textState.SetValueAsync(GetColorTextValue(_valueState.Value));
            }
        }

        private string GetSelectorLocation() => $"translate({Math.Round(_selectorX, 2).ToString(CultureInfo.InvariantCulture)}px, {Math.Round(_selectorY, 2).ToString(CultureInfo.InvariantCulture)}px);";

        private string? GetColorTextValue(MudColor? color) => !_alphaState.Value || _colorPickerViewState.Value is ColorPickerView.Palette or ColorPickerView.GridCompact
            ? color?.ToString(MudColorOutputFormats.Hex)
            : color?.ToString(MudColorOutputFormats.HexA);

        private int GetHexColorInputMaxLength() => !_alphaState.Value ? 7 : 9;

        private EventCallback<MouseEventArgs> GetEventCallback() => EventCallback.Factory.Create<MouseEventArgs>(this, () => CloseAsync());

        private bool IsAnyControlVisible() => ShowPreview || ShowSliders || ShowInputs;

        private EventCallback<MouseEventArgs> GetSelectPaletteColorCallback(MudColor color) => new EventCallbackFactory().Create(this, (MouseEventArgs _) => SelectPaletteColorAsync(color));

        private Color GetButtonColor(ColorPickerView view) => _colorPickerViewState.Value == view ? Color.Primary : Color.Inherit;

        private string GetColorDotClass(MudColor color) => new CssBuilder("mud-picker-color-dot").AddClass("selected", color == _valueState.Value).ToString();

        private string AlphaSliderStyle => new StyleBuilder()
            .AddStyle($"background-image: linear-gradient(to {(RightToLeft ? "left" : "right")}, transparent, {ValueOrDefault.ToString(MudColorOutputFormats.RGB)})")
            .Build();
    }
}
