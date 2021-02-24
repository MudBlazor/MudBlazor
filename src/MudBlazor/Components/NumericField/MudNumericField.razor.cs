using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
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
                minDefault = (T)(object)sbyte.MinValue;
                maxDefault = (T)(object)sbyte.MaxValue;
                stepDefault = (T)(object)1;
            }
            // byte
            else if (typeof(T) == typeof(byte) || typeof(T) == typeof(byte?))
            {
                minDefault = (T)(object)byte.MinValue;
                maxDefault = (T)(object)byte.MaxValue;
                stepDefault = (T)(object)1;
            }
            // short
            else if (typeof(T) == typeof(short) || typeof(T) == typeof(short?))
            {
                minDefault = (T)(object)short.MinValue;
                maxDefault = (T)(object)short.MaxValue;
                stepDefault = (T)(object)1;
            }
            // ushort
            else if (typeof(T) == typeof(ushort) || typeof(T) == typeof(ushort?))
            {
                minDefault = (T)(object)ushort.MinValue;
                maxDefault = (T)(object)ushort.MaxValue;
                stepDefault = (T)(object)1;
            }
            // int
            else if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
            {
                minDefault = (T)(object)int.MinValue;
                maxDefault = (T)(object)int.MaxValue;
                stepDefault = (T)(object)1;
            }
            // uint
            else if (typeof(T) == typeof(uint) || typeof(T) == typeof(uint?))
            {
                minDefault = (T)(object)uint.MinValue;
                maxDefault = (T)(object)uint.MaxValue;
                stepDefault = (T)(object)1;
            }
            // long
            else if (typeof(T) == typeof(long) || typeof(T) == typeof(long?))
            {
                minDefault = (T)(object)long.MinValue;
                maxDefault = (T)(object)long.MaxValue;
                stepDefault = (T)(object)1;
            }
            // ulong
            else if (typeof(T) == typeof(ulong) || typeof(T) == typeof(ulong?))
            {
                minDefault = (T)(object)ulong.MinValue;
                maxDefault = (T)(object)ulong.MaxValue;
                stepDefault = (T)(object)1;
            }
            // float
            else if (typeof(T) == typeof(float) || typeof(T) == typeof(float?))
            {
                minDefault = (T)(object)float.MinValue;
                maxDefault = (T)(object)float.MaxValue;
                stepDefault = (T)(object)1.0f;
            }
            // double
            else if (typeof(T) == typeof(double) || typeof(T) == typeof(double?))
            {
                minDefault = (T)(object)double.MinValue;
                maxDefault = (T)(object)double.MaxValue;
                stepDefault = (T)(object)1.0;
            }
            // decimal
            else if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?))
            {
                minDefault = (T)(object)decimal.MinValue;
                maxDefault = (T)(object)decimal.MaxValue;
                stepDefault = (T)(object)1M;
            }
            #endregion
        }

        protected string Classname =>
           new CssBuilder("mud-input-input-control mud-input-number-control" + (HideSpinButtons ? " mud-input-number-nobutton" : ""))
           .AddClass(Class)
           .Build();

        private Func<T, Task<bool>> _validateInstance;

        private MudInput<T> _elementReference;

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

        /// <summary>
        /// The short hint displayed in the input before the user enters a value.
        /// </summary>
        [Parameter] public string Placeholder { get; set; }

        /// <summary>
        /// If string has value the label text will be displayed in the input, and scaled down at the top if the input has value.
        /// </summary>
        [Parameter] public string Label { get; set; }


        //Tracks if Min has a value.
        private bool minHasValue = false;
        //default value for the type
        private T minDefault;
        private T _min;
        /// <summary>
        /// The minimum value for the input.
        /// </summary>
        [Parameter]
        public T Min
        {
            get => minHasValue ? _min : minDefault;
            set
            {
                minHasValue = value != null;
                _min = value;
            }
        }

        //Tracks if Max has a value.
        private bool maxHasValue = false;
        //default value for the type
        private T maxDefault;
        private T _max;
        /// <summary>
        /// The maximum value for the input.
        /// </summary>
        [Parameter]
        public T Max
        {
            get => maxHasValue ? _max : maxDefault;
            set
            {
                maxHasValue = value != null;
                _max = value;
            }
        }

        //Tracks if Max has a value.
        private bool stepHasValue = false;
        //default value for the type, it's useful for decimal type to avoid constant evaluation
        private T stepDefault;
        private T _step;

        /// <summary>
        /// The increment added/subtracted by the spin buttons.
        /// </summary>
        [Parameter]
        public T Step
        {
            get => stepHasValue ? _step : stepDefault;
            set
            {
                stepHasValue = value != null;
                _step = value;
            }
        }

        /// <summary>
        /// Hides the spin buttons, the user can still change value with keyboard arrows and manual update.
        /// </summary>
        [Parameter]
        public bool HideSpinButtons { get; set; }
    }
}
