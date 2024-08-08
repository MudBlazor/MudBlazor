// Copyright (c) MudBlazor 2022
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudNumericField<T> : MudDebouncedInput<T>
    {
        private IKeyInterceptor _keyInterceptor;
        private Comparer _comparer = new(CultureInfo.InvariantCulture);

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
            }
            // double
            else if (typeof(T) == typeof(double) || typeof(T) == typeof(double?))
            {
                _minDefault = (T)(object)double.MinValue;
                _maxDefault = (T)(object)double.MaxValue;
                _stepDefault = (T)(object)1.0;
            }
            // decimal
            else if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?))
            {
                _minDefault = (T)(object)decimal.MinValue;
                _maxDefault = (T)(object)decimal.MaxValue;
                _stepDefault = (T)(object)1M;
            }

            #endregion parameters default depending on T
        }

        protected string Classname =>
            new CssBuilder("mud-input-input-control mud-input-number-control")
                .AddClass(HideSpinButtons ? "mud-input-nospin" : "mud-input-showspin")
                .AddClass($"mud-input-{Variant.ToDescriptionString()}-with-label", !string.IsNullOrEmpty(Label))
                .AddClass(Class)
                .Build();


        [Inject] private IKeyInterceptorFactory _keyInterceptorFactory { get; set; }

        private string _elementId = Identifier.Create("numericField");

        private MudInput<string> _elementReference;

        [ExcludeFromCodeCoverage]
        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }

        [ExcludeFromCodeCoverage]
        public override ValueTask BlurAsync()
        {
            return _elementReference.BlurAsync();
        }

        [ExcludeFromCodeCoverage]
        public override ValueTask SelectAsync()
        {
            return _elementReference.SelectAsync();
        }

        [ExcludeFromCodeCoverage]
        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _elementReference.SelectRangeAsync(pos1, pos2);
        }

        protected override Task SetValueAsync(T value, bool updateText = true, bool force = false)
        {
            bool valueChanged;
            (value, valueChanged) = ConstrainBoundaries(value);
            return base.SetValueAsync(value, valueChanged || updateText);
        }

        protected internal override async Task OnBlurredAsync(FocusEventArgs obj)
        {
            await base.OnBlurredAsync(obj);
            await UpdateValuePropertyAsync(true); //Required to set the value after a blur before the debounce period has elapsed
            await UpdateTextPropertyAsync(false); //Required to update the string formatting after a blur before the debouce period has elapsed
        }

        protected async Task<bool> ValidateInput(T value)
        {
            bool valueChanged;
            (value, valueChanged) = ConstrainBoundaries(value);
            if (valueChanged)
                await SetValueAsync(value, true);
            return true; //Don't show errors
        }

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Clearable { get; set; } = false;

        /// <summary>
        /// Custom clear icon when <see cref="Clearable"/> is enabled.
        /// </summary>
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
                if (typeof(T).IsValueType && Value is not null)
                {
                    if (factor > 0 && _comparer.Compare(nextValue, Value) < 0)
                        nextValue = Max;
                    else if (factor < 0 && _comparer.Compare(nextValue, Value) > 0)
                        nextValue = Min;
                }

                await SetValueAsync(ConstrainBoundaries(nextValue).value);
                await _elementReference.SetText(Text);
            }
            catch (OverflowException)
            {
                // if next value overflows the primitive type, lets set it to Min or Max depending if factor is positive or negative
                await SetValueAsync(factor > 0 ? Max : Min, true);
            }
        }

        private T GetNextValue(double factor)
        {
            if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?))
                return (T)(object)Convert.ToDecimal(FromDecimal(Value) + (FromDecimal(Step) * (decimal)factor));
            if (typeof(T) == typeof(long) || typeof(T) == typeof(long?))
                return (T)(object)Convert.ToInt64(FromInt64(Value) + (FromInt64(Step) * factor));
            if (typeof(T) == typeof(ulong) || typeof(T) == typeof(ulong?))
                return (T)(object)Convert.ToUInt64(FromUInt64(Value) + (FromUInt64(Step) * factor));
            return Num.To<T>(Num.From(Value) + (Num.From(Step) * factor));
        }

        /// <summary>
        /// Adds a Step to the Value
        /// </summary>
        public Task Increment() => Change(factor: 1);

        /// <summary>
        /// Substracts a Step from the Value
        /// </summary>
        public Task Decrement() => Change(factor: -1);

        /// <summary>
        /// Checks if the value respects the boundaries set for this instance.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>Returns a valid value and if it has been changed.</returns>
        protected (T value, bool changed) ConstrainBoundaries(T value)
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
            };

            return (value, false);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _keyInterceptor = _keyInterceptorFactory.Create();
                await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
                {
                    //EnableLogging = true,
                    TargetClass = "mud-input-slot",
                    Keys = {
                        new KeyOptions { Key="ArrowUp", PreventDown = "key+none" }, // prevent scrolling page, instead increment
                        new KeyOptions { Key="ArrowDown", PreventDown = "key+none" }, // prevent scrolling page, instead decrement
                        new KeyOptions { Key="Dead", PreventDown = "key+any" }, // prevent dead keys like ^ ` ´ etc
                        new KeyOptions { Key="/^(?!"+(Pattern ?? "[0-9]").TrimEnd('*')+").$/", PreventDown = "key+none|key+shift|key+alt" }, // prevent input of all other characters except allowed, like [0-9.,-+]
                    },
                });
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        protected async Task HandleKeydown(KeyboardEventArgs obj)
        {
            if (GetDisabledState() || GetReadOnlyState())
                return;
            switch (obj.Key)
            {
                case "ArrowUp":
                    await Increment();
                    break;
                case "ArrowDown":
                    await Decrement();
                    break;
            }
            await OnKeyDown.InvokeAsync(obj);
        }

        protected Task HandleKeyUp(KeyboardEventArgs obj)
        {
            if (GetDisabledState() || GetReadOnlyState())
                return Task.CompletedTask;
            return OnKeyUp.InvokeAsync(obj);
        }

        protected async Task OnMouseWheel(WheelEventArgs obj)
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

        private bool _minHasValue = false;

        /// <summary>
        /// Reverts mouse wheel up and down events, if true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool InvertMouseWheel { get; set; } = false;

        private T _minDefault;

        private T _min;

        /// <summary>
        /// The minimum value for the input.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public T Min
        {
            get => _minHasValue ? _min : _minDefault;
            set
            {
                _minHasValue = value != null;
                _min = value;
            }
        }

        private bool _maxHasValue = false;
        private T _maxDefault;
        private T _max;

        /// <summary>
        /// The maximum value for the input.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public T Max
        {
            get => _maxHasValue ? _max : _maxDefault;
            set
            {
                _maxHasValue = value != null;
                _max = value;
            }
        }

        private bool _stepHasValue = false;
        private T _stepDefault;
        private T _step;

        /// <summary>
        /// The increment added/subtracted by the spin buttons.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public T Step
        {
            get => _stepHasValue ? _step : _stepDefault;
            set
            {
                _stepHasValue = value != null;
                _step = value;
            }
        }

        /// <summary>
        /// Hides the spin buttons, the user can still change value with keyboard arrows and manual update.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool HideSpinButtons { get; set; }

        /// <summary>
        ///  Hints at the type of data that might be entered by the user while editing the input.
        ///  Defaults to numeric
        /// </summary>
        [Parameter]
        public override InputMode InputMode { get; set; } = InputMode.numeric;

        /// <summary>
        /// <para>
        /// The pattern attribute, when specified, is a regular expression which the input's value must match in order for the value to pass constraint validation. It must be a valid JavaScript regular expression
        /// Defaults to [0-9,.\-]
        /// To get a numerical keyboard on safari, use the pattern. The default pattern should achieve numerical keyboard.
        /// </para>
        /// <para>Note: this pattern is also used to prevent all input except numbers and allowed characters. So for instance to allow only numbers, no signs and no commas you might change it to [0-9.]</para>
        /// </summary>
        [Parameter]
        public override string Pattern { get; set; } = @"[0-9,.\-]";

        private string GetCounterText() => Counter == null ? string.Empty : (Counter == 0 ? (string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") : ((string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") + $" / {Counter}"));

        private Task OnInputValueChanged(string text)
        {
            return SetTextAsync(text);
        }

        //avoids the format to use scientific notation for large or small number in floating points types, while covering all options
        //https://stackoverflow.com/questions/1546113/double-to-string-conversion-without-scientific-notation
        private const string TagFormat = "0.###################################################################################################################################################################################################################################################################################################################################################";

        private string FormatParam(T value)
        {
            if (value is IFormattable f)
                return f.ToString(TagFormat, CultureInfo.InvariantCulture.NumberFormat);
            else
                return null;
        }

        private decimal FromDecimal(T v)
            => Convert.ToDecimal((decimal?)(object)v);
        private long FromInt64(T v)
            => Convert.ToInt64((long?)(object)v);
        private ulong FromUInt64(T v)
            => Convert.ToUInt64((ulong?)(object)v);

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _keyInterceptor?.Dispose();
            }
        }
    }
}
