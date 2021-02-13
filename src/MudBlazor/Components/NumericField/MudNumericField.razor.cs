using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudNumericField<T> : MudDebouncedInput<T> where T:struct
    {
        public MudNumericField() : base()
        {
            //With input[type=number] the browser reads and sets the value using dot as decimal separator, while showing and parsing the text in the user Locale setting.
            //Since this is completely transparent to us, we need to use a Converter using an InvariantCulture.
            SetConverter(new DefaultConverter<T> { Culture = CultureInfo.InvariantCulture });

            //sbyte
            if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(sbyte?))
            {
                Min = (T)(object)sbyte.MinValue;
                Max = (T)(object)sbyte.MaxValue;
                Step = (T)(object)1;
            }
            // byte
            else if (typeof(T) == typeof(byte) || typeof(T) == typeof(byte?))
            {
                Min = (T)(object)byte.MinValue;
                Max = (T)(object)byte.MaxValue;
                Step = (T)(object)1;
            }
            // short
            else if (typeof(T) == typeof(short) || typeof(T) == typeof(short?))
            {
                Min = (T)(object)short.MinValue;
                Max = (T)(object)short.MaxValue;
                Step = (T)(object)1;
            }
            // ushort
            else if (typeof(T) == typeof(ushort) || typeof(T) == typeof(ushort?))
            {
                Min = (T)(object)ushort.MinValue;
                Max = (T)(object)ushort.MaxValue;
                Step = (T)(object)1;
            }
            // int
            else if (typeof(T) == typeof(int) || typeof(T) == typeof(int?))
            {
                Min = (T)(object)int.MinValue;
                Max = (T)(object)int.MaxValue;
                Step = (T)(object)1;
            }
            // uint
            else if (typeof(T) == typeof(uint) || typeof(T) == typeof(uint?))
            {
                Min = (T)(object)uint.MinValue;
                Max = (T)(object)uint.MaxValue;
                Step = (T)(object)1;
            }
            // long
            else if (typeof(T) == typeof(long) || typeof(T) == typeof(long?))
            {
                Min = (T)(object)long.MinValue;
                Max = (T)(object)long.MaxValue;
                Step = (T)(object)1;
            }
            // ulong
            else if (typeof(T) == typeof(ulong) || typeof(T) == typeof(ulong?))
            {
                Min = (T)(object)ulong.MinValue;
                Max = (T)(object)ulong.MaxValue;
                Step = (T)(object)1;
            }
            // float
            else if (typeof(T) == typeof(float) || typeof(T) == typeof(float?))
            {
                Min = (T)(object)float.MinValue;
                Max = (T)(object)float.MaxValue;
                Step = (T)(object)1.0f;
            }
            // double
            else if (typeof(T) == typeof(double) || typeof(T) == typeof(double?))
            {
                Min = (T)(object)double.MinValue;
                Max = (T)(object)double.MaxValue;
                Step = (T)(object)1.0;
            }
            // decimal
            else if (typeof(T) == typeof(decimal) || typeof(T) == typeof(decimal?))
            {
                Min = (T)(object)decimal.MinValue;
                Max = (T)(object)decimal.MaxValue;
                Step = (T)(object)1M;
            }
        }

        protected string Classname =>
           new CssBuilder("mud-input-input-control mud-input-number-control")
           .AddClass(Class)
           .Build();

        private MudInput<string> _elementReference;

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
            await base.SetValueAsync(value, updateText || valueChanged);
        }

        #region Numeric range
        public async Task Increment()
        {
            dynamic val = Value switch
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
                _ => Value,
            };
            await SetValueAsync(val);
        }

        public async Task Decrement()
        {
            dynamic val = Value switch
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
            await SetValueAsync(val);
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

        /// <summary>
        /// The minimum value for the input.
        /// </summary>
        [Parameter] public T Min { get; set; }

        /// <summary>
        /// The maximum value for the input.
        /// </summary>
        [Parameter] public T Max { get; set; }

        /// <summary>
        /// The increment added/subtracted by the spin buttons.
        /// </summary>
        [Parameter] public T Step { get; set; }

        private string InputNumericParameter(T value)
        {
            var culture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            string ret = value.ToString();
            CultureInfo.CurrentCulture = culture;
            return ret;
        }
    }
}
