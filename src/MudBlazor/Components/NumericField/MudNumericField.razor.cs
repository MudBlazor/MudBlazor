using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudNumericField<T> : MudDebouncedInput<T>
    {
        public MudNumericField() : base()
        {
            //With input[type=number] the browser reads and sets the value using dot as decimal separator, while showing and parsing the text in the user Locale setting.
            //Since this is completely transparent to us, we need to use a Converter using an InvariantCulture.
            SetConverter(new DefaultConverter<T> { Culture = CultureInfo.InvariantCulture });

            _validateInstance = new Func<T, Task<bool>>(ValidateInput);

            #region parameters default depending on T
            //sbyte
            if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(sbyte?))
            {
                _minDefault = (T)(object)sbyte.MinValue;
                _maxDefault = (T)(object)sbyte.MaxValue;
                _stepDefault = (T)(object)1;
            }
            // byte
            else if (typeof(T) == typeof(byte) || typeof(T) == typeof(byte?))
            {
                _minDefault = (T)(object)byte.MinValue;
                _maxDefault = (T)(object)byte.MaxValue;
                _stepDefault = (T)(object)1;
            }
            // short
            else if (typeof(T) == typeof(short) || typeof(T) == typeof(short?))
            {
                _minDefault = (T)(object)short.MinValue;
                _maxDefault = (T)(object)short.MaxValue;
                _stepDefault = (T)(object)1;
            }
            // ushort
            else if (typeof(T) == typeof(ushort) || typeof(T) == typeof(ushort?))
            {
                _minDefault = (T)(object)ushort.MinValue;
                _maxDefault = (T)(object)ushort.MaxValue;
                _stepDefault = (T)(object)1;
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
                _stepDefault = (T)(object)1;
            }
            // long
            else if (typeof(T) == typeof(long) || typeof(T) == typeof(long?))
            {
                _minDefault = (T)(object)long.MinValue;
                _maxDefault = (T)(object)long.MaxValue;
                _stepDefault = (T)(object)1;
            }
            // ulong
            else if (typeof(T) == typeof(ulong) || typeof(T) == typeof(ulong?))
            {
                _minDefault = (T)(object)ulong.MinValue;
                _maxDefault = (T)(object)ulong.MaxValue;
                _stepDefault = (T)(object)1;
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
            #endregion
        }

        protected string Classname =>
           new CssBuilder("mud-input-input-control mud-input-number-control " + (HideSpinButtons ? "mud-input-nospin" : "mud-input-showspin"))
           .AddClass(Class)
           .Build();

        private Func<T, Task<bool>> _validateInstance;

        private MudInput<string> _elementReference;

        private static Converter<string> s_inputConverter = new DefaultConverter<string> { Culture = CultureInfo.InvariantCulture };

        public override ValueTask FocusAsync()
        {
            return _elementReference.FocusAsync();
        }

        public override ValueTask SelectAsync()
        {
            return _elementReference.SelectAsync();
        }

        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return _elementReference.SelectRangeAsync(pos1, pos2);
        }

        protected override async Task SetValueAsync(T value, bool updateText = true)
        {
            bool valueChanged;
            (value, valueChanged) = ConstrainBoundaries(value);
            await base.SetValueAsync(ConstrainBoundaries(value).value, valueChanged || updateText);
        }

        protected async Task<bool> ValidateInput(T value)
        {
            bool valueChanged;
            (value, valueChanged) = ConstrainBoundaries(value);
            if (valueChanged)
                await SetValueAsync(value, true);
            return true;//Don't show errors
        }


        #region Numeric range
        public async Task Increment()
        {
            dynamic val;
            try
            {
                checked
                {
                    val = Value switch
                    {
                        sbyte b => b + (sbyte)(object)Step,
                        byte b => b + (byte)(object)Step,
                        short i => i + (short)(object)Step,
                        ushort i => i + (ushort)(object)Step,
                        int i => i + (int)(object)Step,
                        uint i => i + (uint)(object)Step,
                        long i => i + (long)(object)Step,
                        ulong i => i + (ulong)(object)Step,
                        float f => f + (float)(object)Step,
                        double d => d + (double)(object)Step,
                        decimal d => d + (decimal)(object)Step,
                        _ => Value
                    };
                }
            }
            catch (OverflowException)
            {
                val = Max;
            }

            await SetValueAsync(ConstrainBoundaries((T)val).value);
        }

        public async Task Decrement()
        {
            dynamic val;
            try
            {
                checked
                {
                    val = Value switch
                    {
                        sbyte b => b - (sbyte)(object)Step,
                        byte b => b - (byte)(object)Step,
                        short i => i - (short)(object)Step,
                        ushort i => i - (ushort)(object)Step,
                        int i => i - (int)(object)Step,
                        uint i => i - (uint)(object)Step,
                        long i => i - (long)(object)Step,
                        ulong i => i - (ulong)(object)Step,
                        float f => f - (float)(object)Step,
                        double d => d - (double)(object)Step,
                        decimal d => d - (decimal)(object)Step,
                        _ => Value,
                    };
                }
            }
            catch (OverflowException)
            {
                val = Min;
            }

            await SetValueAsync(ConstrainBoundaries((T)val).value);
        }

        /// <summary>
        /// Checks if the value respects the boundaries set for this instance.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns>Returns a valid value and if it has been changed.</returns>
        protected (T value, bool changed) ConstrainBoundaries(T value)
        {
            //check if Max/Min has value, if not use MaxValue/MinValue for that data type
            switch (value)
            {
                case sbyte b:
                    if (b > (sbyte)(object)Max)
                        return (Max, true);
                    else if (b < (sbyte)(object)Min)
                        return (Min, true);
                    break;

                case byte b:
                    if (b > (byte)(object)Max)
                        return (Max, true);
                    else if (b < (byte)(object)Min)
                        return (Min, true);
                    break;

                case short i:
                    if (i > (short)(object)Max)
                        return (Max, true);
                    else if (i < (short)(object)Min)
                        return (Min, true);
                    break;

                case ushort i:
                    if (i > (ushort)(object)Max)
                        return (Max, true);
                    else if (i < (ushort)(object)Min)
                        return (Min, true);
                    break;

                case int i:
                    if (i > (int)(object)Max)
                        return (Max, true);
                    else if (i < (int)(object)Min)
                        return (Min, true);
                    break;

                case uint i:
                    if (i > (uint)(object)Max)
                        return (Max, true);
                    else if (i < (uint)(object)Min)
                        return (Min, true);
                    break;

                case long i:
                    if (i > (long)(object)Max)
                        return (Max, true);
                    else if (i < (long)(object)Min)
                        return (Min, true);
                    break;

                case ulong i:
                    if (i > (ulong)(object)Max)
                        return (Max, true);
                    else if (i < (ulong)(object)Min)
                        return (Min, true);
                    break;

                case float f:
                    if (f > (float)(object)Max)
                        return (Max, true);
                    else if (f < (float)(object)Min)
                        return (Min, true);
                    break;

                case double d:
                    if (d > (double)(object)Max)
                        return (Max, true);
                    else if (d < (double)(object)Min)
                        return (Min, true);
                    break;

                case decimal d:
                    if (d > (decimal)(object)Max)
                        return (Max, true);
                    else if (d < (decimal)(object)Min)
                        return (Min, true);
                    break;
            };

            return (value, false);
        }

        #endregion

        private bool _keyDownPreventDefault;
        /// <summary>
        /// Overrides KeyDown event, intercepts Arrow Up/Down and uses them to add/substract the value manually by the step value.
        /// Relying on the browser mean the steps are each integer multiple from <see cref="Min"/> up until <see cref="Max"/>.
        /// This align the behaviour with the spinner buttons.
        /// </summary>
        /// <remarks>https://try.mudblazor.com/snippet/QamlkdvmBtrsuEtb</remarks>
        protected async Task InterceptArrowKey(KeyboardEventArgs obj)
        {
            if (obj.Type == "keydown")//KeyDown or repeat, blazor never fire InvokeKeyPress
            {
                if (obj.Key == "ArrowUp")
                {
                    await Increment();
                    return;
                }
                else if (obj.Key == "ArrowDown")
                {
                    await Decrement();
                    return;
                }
            }
            _keyDownPreventDefault = KeyDownPreventDefault;
            OnKeyPress.InvokeAsync(obj).AndForget();
        }

        /// <summary>
        /// The short hint displayed in the input before the user enters a value.
        /// </summary>
        [Parameter] public string Placeholder { get; set; }

        /// <summary>
        /// If string has value the label text will be displayed in the input, and scaled down at the top if the input has value.
        /// </summary>
        [Parameter] public string Label { get; set; }


        //Tracks if Min has a value.
        private bool _minHasValue = false;
        //default value for the type
        private T _minDefault;
        private T _min;
        /// <summary>
        /// The minimum value for the input.
        /// </summary>
        [Parameter]
        public T Min
        {
            get => _minHasValue ? _min : _minDefault;
            set
            {
                _minHasValue = value != null;
                _min = value;
            }
        }

        //Tracks if Max has a value.
        private bool _maxHasValue = false;
        //default value for the type
        private T _maxDefault;
        private T _max;
        /// <summary>
        /// The maximum value for the input.
        /// </summary>
        [Parameter]
        public T Max
        {
            get => _maxHasValue ? _max : _maxDefault;
            set
            {
                _maxHasValue = value != null;
                _max = value;
            }
        }

        //Tracks if Max has a value.
        private bool _stepHasValue = false;
        //default value for the type, it's useful for decimal type to avoid constant evaluation
        private T _stepDefault;
        private T _step;

        /// <summary>
        /// The increment added/subtracted by the spin buttons.
        /// </summary>
        [Parameter]
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
        [Parameter] public bool HideSpinButtons { get; set; }
    }
}
