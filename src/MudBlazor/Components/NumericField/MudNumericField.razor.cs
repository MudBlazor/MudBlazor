// Copyright (c) MudBlazor 2022
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// A field for numeric values from users. 
    /// </summary>
    /// <typeparam name="T">The type of number being collected.</typeparam>
    public partial class MudNumericField<T> : MudDebouncedInput<T>
    {
        private T? _step;
        private T? _max;
        private T? _min;
        private readonly T? _minDefault;
        private readonly T? _maxDefault;
        private readonly T? _stepDefault;
        private bool _maxHasValue = false;
        private bool _minHasValue = false;
        private bool _stepHasValue = false;
        private bool _cultureParameterSpecified;
        private MudInput<string> _elementReference = null!;
        private readonly string _elementId = Identifier.Create("numericField");
        private const string DefaultKeyFilterPattern = @"[0-9,.\-]";

        private readonly Comparer<T> _comparer = Comparer<T>.Default;

        [Inject]
        private IKeyInterceptorService KeyInterceptorService { get; set; } = null!;

        public MudNumericField()
        {
            Validation = new Func<T, Task<bool>>(ValidateInput);
            #region parameters default depending on T

            //sbyte
            if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(sbyte?))
            {
                _minDefault = (T)(object)sbyte.MinValue;
                _maxDefault = (T)(object)sbyte.MaxValue;
                _stepDefault = (T)(object)(sbyte)1;
            }
            // byte
            else if (typeof(T) == typeof(byte) || typeof(T) == typeof(byte?))
            {
                _minDefault = (T)(object)byte.MinValue;
                _maxDefault = (T)(object)byte.MaxValue;
                _stepDefault = (T)(object)(byte)1;
            }
            // short
            else if (typeof(T) == typeof(short) || typeof(T) == typeof(short?))
            {
                _minDefault = (T)(object)short.MinValue;
                _maxDefault = (T)(object)short.MaxValue;
                _stepDefault = (T)(object)(short)1;
            }
            // ushort
            else if (typeof(T) == typeof(ushort) || typeof(T) == typeof(ushort?))
            {
                _minDefault = (T)(object)ushort.MinValue;
                _maxDefault = (T)(object)ushort.MaxValue;
                _stepDefault = (T)(object)(ushort)1;
            }
            // int
            else if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
            {
                _minDefault = (T)(object)int.MinValue;
                _maxDefault = (T)(object)int.MaxValue;
                _stepDefault = (T)(object)1;
            }
            // uint
            else if (typeof(T) == typeof(uint) || typeof(T) == typeof(uint?))
            {
                _minDefault = (T)(object)uint.MinValue;
                _maxDefault = (T)(object)uint.MaxValue;
                _stepDefault = (T)(object)1u;
            }
            // long
            else if (typeof(T) == typeof(long) || typeof(T) == typeof(long?))
            {
                _minDefault = (T)(object)long.MinValue;
                _maxDefault = (T)(object)long.MaxValue;
                _stepDefault = (T)(object)1L;
            }
            // ulong
            else if (typeof(T) == typeof(ulong) || typeof(T) == typeof(ulong?))
            {
                _minDefault = (T)(object)ulong.MinValue;
                _maxDefault = (T)(object)ulong.MaxValue;
                _stepDefault = (T)(object)1ul;
            }
            // float
            else if (typeof(T) == typeof(float) || typeof(T) == typeof(float?))
            {
                _minDefault = (T)(object)float.MinValue;
                _maxDefault = (T)(object)float.MaxValue;
                _stepDefault = (T)(object)1.0f;
                InputMode = InputMode.@decimal;
            }
            // double
            else if (typeof(T) == typeof(double) || typeof(T) == typeof(double?))
            {
                _minDefault = (T)(object)double.MinValue;
                _maxDefault = (T)(object)double.MaxValue;
                _stepDefault = (T)(object)1.0;
                InputMode = InputMode.@decimal;
            }
            // decimal
            else if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?))
            {
                _minDefault = (T)(object)decimal.MinValue;
                _maxDefault = (T)(object)decimal.MaxValue;
                _stepDefault = (T)(object)1M;
                InputMode = InputMode.@decimal;
            }

            #endregion parameters default depending on T
        }

        protected string Classname =>
            new CssBuilder("mud-input-input-control mud-input-number-control")
                .AddClass(HideSpinButtons ? "mud-input-nospin" : "mud-input-showspin")
                .AddClass(Class)
                .Build();

        private Dictionary<string, object?> InputAttributes
        {
            get
            {
                var attributes = new Dictionary<string, object?>(UserAttributes, StringComparer.OrdinalIgnoreCase)
                {
                    ["role"] = "spinbutton"
                };

                if (TryFormatAriaValue(ReadValue, out var ariaValueNow))
                {
                    attributes["aria-valuenow"] = ariaValueNow;
                }

                if (_minHasValue && TryFormatAriaValue(_min, out var ariaValueMin))
                {
                    attributes["aria-valuemin"] = ariaValueMin;
                }

                if (_maxHasValue && TryFormatAriaValue(_max, out var ariaValueMax))
                {
                    attributes["aria-valuemax"] = ariaValueMax;
                }

                if (!string.IsNullOrWhiteSpace(ReadText) &&
                    (!attributes.TryGetValue("aria-valuenow", out var currentAriaValue) || !string.Equals(ReadText, currentAriaValue?.ToString(), StringComparison.Ordinal)))
                {
                    attributes["aria-valuetext"] = ReadText;
                }

                return attributes;
            }
        }

        private bool UsesManagedFormatting =>
            Pattern is not null ||
            GetFormat() is not null ||
            _cultureParameterSpecified;

        private string EffectiveKeyFilterPattern => (Pattern ?? DefaultKeyFilterPattern).TrimEnd('*');

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override ValueTask BlurAsync()
        {
            return _elementReference.BlurAsync();
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override ValueTask SelectAsync()
        {
            return _elementReference.SelectAsync();
        }

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _elementReference.SelectRangeAsync(pos1, pos2);
        }

        /// <inheritdoc />
        protected override Task SetValueAndUpdateTextAsync(T? value, bool updateText = true, bool force = false)
        {
            (value, var valueChanged) = ConstrainBoundaries(value);
            return base.SetValueAndUpdateTextAsync(value, valueChanged || updateText, force);
        }

        /// <inheritdoc />
        protected internal override async Task OnBlurredAsync(FocusEventArgs obj)
        {
            await base.OnBlurredAsync(obj);

            if (Immediate || DebounceInterval > 0)
            {
                await UpdateValuePropertyAsync(true); //Required to set the value after a blur before the debounce period has elapsed
            }
            else
            {
                // For non-immediate, non-debounced inputs, browser onchange timing can race with blur handlers.
                // Parse current text only when it is not already the formatted representation of the current value.
                var formattedValueText = ConvertSet(ReadValue);
                if (!string.Equals(ReadText, formattedValueText, StringComparison.Ordinal))
                {
                    await UpdateValuePropertyAsync(true);
                }
            }

            await UpdateTextPropertyAsync(false); //Required to update the string formatting after a blur before the debounce period has elapsed

            if (UsesManagedFormatting && DebounceInterval <= 0 && !ConversionError)
            {
                await _elementReference.SetText(ReadText, updateValue: false);
            }
        }

        protected async Task<bool> ValidateInput(T? value)
        {
            (value, var valueChanged) = ConstrainBoundaries(value);
            if (valueChanged)
                await SetValueAndUpdateTextAsync(value, true);
            return true; //Don't show errors
        }

        /// <summary>
        /// Shows a button to clear the value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Clearable { get; set; } = false;

        /// <summary>
        /// The icon of the clear button when <see cref="Clearable"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Clear"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string ClearIcon { get; set; } = Icons.Material.Filled.Clear;

        /// <summary>
        /// Decrements or increments depending on factor
        /// </summary>
        /// <param name="factor">Multiplication factor (1 or -1) will be applied to the step</param>
        private async Task Change(double factor = 1)
        {
            try
            {
                var nextValue = GetNextValue(factor) ?? Num.To<T>(0);

                // validate that the data type is a value type before we compare them
                if (typeof(T).IsValueType && ReadValue is not null)
                {
                    if (factor > 0 && _comparer.Compare(nextValue, ReadValue) < 0)
                        nextValue = Max;
                    else if (factor < 0 && _comparer.Compare(nextValue, ReadValue) > 0)
                        nextValue = Min;
                }

                await SetValueAndUpdateTextAsync(ConstrainBoundaries(nextValue).value);
                await _elementReference.SetText(ReadText);
            }
            catch (OverflowException)
            {
                // if next value overflows the primitive type, lets set it to Min or Max depending on if factor is positive or negative
                await SetValueAndUpdateTextAsync(factor > 0 ? Max : Min, true);
            }
        }

        private T? GetNextValue(double factor)
        {
            if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?))
                return (T)(object)Convert.ToDecimal(FromDecimal(ReadValue) + (FromDecimal(Step) * (decimal)factor));
            if (typeof(T) == typeof(long) || typeof(T) == typeof(long?))
                return (T)(object)Convert.ToInt64(FromInt64(ReadValue) + (FromInt64(Step) * factor));
            if (typeof(T) == typeof(ulong) || typeof(T) == typeof(ulong?))
                return (T)(object)Convert.ToUInt64(FromUInt64(ReadValue) + (FromUInt64(Step) * factor));
            return Num.To<T>(Num.From(ReadValue) + (Num.From(Step) * factor));
        }

        /// <summary>
        /// Increases the current value by <see cref="Step"/>.
        /// </summary>
        public Task Increment() => Change(factor: 1);

        /// <summary>
        /// Decreases the current value by <see cref="Step"/>.
        /// </summary>
        public Task Decrement() => Change(factor: -1);

        /// <summary>
        /// Checks if the value respects the boundaries set for this instance.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>Returns a valid value and if it has been changed.</returns>
        protected (T? value, bool changed) ConstrainBoundaries(T? value)
        {
            if (value == null)
                return (default(T), false);

            // validate that the data type is a value type before we compare them
            if (typeof(T).IsValueType)
            {
                // check if value is bigger than defined MAX, if so take the defined MAX value instead
                if (_comparer.Compare(value, Max) > 0)
                    return (Max, true);

                // check if value is lower than defined MIN, if so take the defined MIN value instead
                if (_comparer.Compare(value, Min) < 0)
                    return (Min, true);
            }

            return (value, false);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var keyOptions = new List<KeyOptions>
                {
                    // prevent scrolling page, instead increment
                    new("ArrowUp", preventDown: "key+none"),
                    // prevent scrolling page, instead decrement
                    new("ArrowDown", preventDown: "key+none"),
                     // prevent dead keys like ^ ` ´ etc
                    new("Dead", preventDown: "key+any"),
                    // keep the default numeric input constrained even though the field now renders as type="text"
                    new($"/^(?!{EffectiveKeyFilterPattern}).$/", preventDown: "key+none|key+shift|key+alt"),
                };

                var options = new KeyInterceptorOptions("mud-input-slot", keyOptions.ToArray());

                await KeyInterceptorService.SubscribeAsync(_elementId, options, keys => keys
                    .When(CanHandleKeys, builder => builder
                        .OnKeyDown("ArrowUp", Increment)
                        .OnKeyDown("ArrowDown", Decrement)));
            }

            await base.OnAfterRenderAsync(firstRender);

            if (!firstRender)
            {
                return;
            }

            // Numeric fields default to an invariant text representation unless Culture, Pattern, or Format is supplied explicitly.
            if (!UsesManagedFormatting)
            {
                await SetCultureAsync(CultureInfo.InvariantCulture);
            }
        }

        private bool CanHandleKeys() => !GetDisabledState() && !GetReadOnlyState();

        protected async Task HandleKeyDownAsync(KeyboardEventArgs obj)
        {
            await KeyInterceptorService.DispatchAsync(_elementId, KeyEventKind.Down, obj);
            await OnKeyDown.InvokeAsync(obj);
        }

        protected Task HandleKeyUpAsync(KeyboardEventArgs obj)
        {
            if (GetDisabledState() || GetReadOnlyState())
                return Task.CompletedTask;

            return OnKeyUp.InvokeAsync(obj);
        }

        protected async Task OnMouseWheelAsync(WheelEventArgs obj)
        {
            if (!obj.ShiftKey || GetDisabledState() || GetReadOnlyState())
                return;
            if (obj.DeltaY < 0)
            {
                if (InvertMouseWheel == false)
                    await Increment();
                else
                    await Decrement();
            }
            else if (obj.DeltaY > 0)
            {
                if (InvertMouseWheel == false)
                    await Decrement();
                else
                    await Increment();
            }
        }

        /// <summary>
        /// Reverses the mouse wheel direction.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  
        /// When <c>true</c>, moving the mouse wheel up will decrease the value, and down will increase the value.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool InvertMouseWheel { get; set; } = false;

        /// <summary>
        /// The minimum allowed value.
        /// </summary>
        /// <remarks>
        /// Defaults to the minimum value of the numeric type, such as <see cref="int.MinValue"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public T? Min
        {
            get => _minHasValue ? _min : _minDefault;
            set
            {
                _minHasValue = value != null;
                _min = value;
            }
        }

        /// <summary>
        /// The maximum allowed value.
        /// </summary>
        /// <remarks>
        /// Defaults to the maximum value of the numeric type, such as <see cref="int.MaxValue"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public T? Max
        {
            get => _maxHasValue ? _max : _maxDefault;
            set
            {
                _maxHasValue = value != null;
                _max = value;
            }
        }

        /// <summary>
        /// The amount added or subtracted when changing values.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>1</c>.  
        /// This affects changing values via spin buttons or the keyboard.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public T? Step
        {
            get => _stepHasValue ? _step : _stepDefault;
            set
            {
                _stepHasValue = value != null;
                _step = value;
            }
        }

        /// <summary>
        /// Hides the up and down buttons.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>false</c>, the user can still change values with the keyboard arrows and by typing values.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool HideSpinButtons { get; set; }

        /// <summary>
        /// The type of value collected by this field.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="InputMode.numeric"/>.
        /// </remarks>
        [Parameter]
        public override InputMode InputMode { get; set; } = InputMode.numeric;

        /// <summary>
        /// The regular expression used to constrain values.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>, which will show a numerical keyboard on Safari.  Must be a valid JavaScript regular expression.  To allow only numbers (with no signs or commas), you can use <c>[0-9.]</c>.
        /// </remarks>
        [Parameter]
        public override string? Pattern { get; set; } = null;

        private string GetCounterText() => Counter switch
        {
            null => string.Empty,
            0 => string.IsNullOrEmpty(ReadText) ? "0" : $"{ReadText.Length}",
            _ => (string.IsNullOrEmpty(ReadText) ? "0" : $"{ReadText.Length}") + $" / {Counter}"
        };

        private async Task OnInputValueChanged(string text)
        {
            await SetTextAndUpdateValueAsync(text);

            // Keep formatted text in sync with the value when using managed formatting, but only for a
            // committed change (onchange), never for live typing (oninput). When Immediate is true this
            // callback runs on every keystroke; reformatting then would rewrite the text mid-typing and
            // jump the caret to the end, making multi-digit or decimal entry impossible (e.g. typing
            // "1234" with Format="F3" collapses to "1.000", and "1." loses its trailing characters).
            // The parsed value stays correct while typing; the text is reformatted on blur instead
            // (see OnBlurredAsync). This matches the non-Immediate behavior and pre-v9.1 formatting.
            if (!Immediate && UsesManagedFormatting && DebounceInterval <= 0 && !ConversionError)
            {
                var formattedText = ConvertSet(ReadValue);
                if (!string.Equals(ReadText, formattedText, StringComparison.Ordinal))
                {
                    await SetTextCoreAsync(formattedText);
                    await _elementReference.SetText(formattedText, updateValue: false);
                }
            }
        }

        //avoids the format to use scientific notation for large or small number in floating points types, while covering all options
        //https://stackoverflow.com/questions/1546113/double-to-string-conversion-without-scientific-notation
        private const string TagFormat = "0.###################################################################################################################################################################################################################################################################################################################################################";

        private static string? FormatParam(T? value)
        {
            if (value is IFormattable f)
                return f.ToString(TagFormat, CultureInfo.InvariantCulture.NumberFormat);
            return null;
        }

        private static decimal FromDecimal(T? v) => Convert.ToDecimal((decimal?)(object?)v);

        private static long FromInt64(T? v) => Convert.ToInt64((long?)(object?)v);

        private static ulong FromUInt64(T? v) => Convert.ToUInt64((ulong?)(object?)v);

        private static bool TryFormatAriaValue(T? value, [NotNullWhen(true)] out string? ariaValue)
        {
            ariaValue = FormatParam(value);
            return !string.IsNullOrWhiteSpace(ariaValue);
        }

        /// <inheritdoc />
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            _cultureParameterSpecified = parameters.Contains<CultureInfo>(nameof(Culture));
            await base.SetParametersAsync(parameters);
        }

        /// <inheritdoc />
        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            if (IsJSRuntimeAvailable)
            {
                await KeyInterceptorService.UnsubscribeAsync(_elementId);
            }
        }
    }
}
