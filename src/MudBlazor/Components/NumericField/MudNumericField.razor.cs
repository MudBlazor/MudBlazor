using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudNumericField<T> : MudDebouncedInput<T>
    {
        protected string Classname =>
           new CssBuilder("mud-input-input-control")
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
                sbyte b => b + Step.ToSByte(1),
                byte b => b + Step.ToByte(1),
                short i => i + Step.ToInt16(1),
                ushort i => i + Step.ToUInt16(1),
                int i => i + Step.ToInt32(1),
                uint i => i + Step.ToUInt32(1),
                long i => i + Step.ToInt64(1),
                ulong i => i + Step.ToUInt64(1),
                float f => f + Step.ToSingle(1),
                double d => d + Step.ToDouble(1),
                decimal d => d + Step.ToDecimal(1),
                _ => Value,
            };
            await SetValueAsync(val);
        }

        public async Task Decrement()
        {
            dynamic val = Value switch
            {
                sbyte b => b - Step.ToSByte(1),
                byte b => b - Step.ToByte(1),
                short i => i - Step.ToInt16(1),
                ushort i => i - Step.ToUInt16(1),
                int i => i - Step.ToInt32(1),
                uint i => i - Step.ToUInt32(1),
                long i => i - Step.ToInt64(1),
                ulong i => i - Step.ToUInt64(1),
                float f => f - Step.ToSingle(1),
                double d => d - Step.ToDouble(1),
                decimal d => d - Step.ToDecimal(1),
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
                    if (b > Max.ToSByte(sbyte.MaxValue))
                        return (Max.Value, true);
                    else if (b < Min.ToSByte(sbyte.MinValue))
                        return (Min.Value, true);
                    break;

                case byte b:
                    if (b > Max.ToByte(byte.MaxValue))
                        return (Max.Value, true);
                    else if (b < Min.ToByte(byte.MinValue))
                        return (Min.Value, true);
                    break;

                case short i:
                    if (i > Max.ToInt16(short.MaxValue))
                        return (Max.Value, true);
                    else if (i < Min.ToInt16(short.MinValue))
                        return (Min.Value, true);
                    break;

                case ushort i:
                    if (i > Max.ToUInt16(ushort.MaxValue))
                        return (Max.Value, true);
                    else if (i < Min.ToUInt16(ushort.MinValue))
                        return (Min.Value, true);
                    break;

                case int i:
                    if (i > Max.ToInt32(int.MaxValue))
                        return (Max.Value, true);
                    else if (i < Min.ToInt32(int.MinValue))
                        return (Min.Value, true);
                    break;

                case uint i:
                    if (i > Max.ToUInt32(uint.MaxValue))
                        return (Max.Value, true);
                    else if (i < Min.ToUInt32(uint.MinValue))
                        return (Min.Value, true);
                    break;

                case long i:
                    if (i > Max.ToInt64(long.MaxValue))
                        return (Max.Value, true);
                    else if (i < Min.ToInt64(long.MinValue))
                        return (Min.Value, true);
                    break;

                case ulong i:
                    if (i > Max.ToUInt64(ulong.MaxValue))
                        return (Max.Value, true);
                    else if (i < Min.ToUInt64(ulong.MinValue))
                        return (Min.Value, true);
                    break;

                case float f:
                    if (f > Max.ToSingle(float.MaxValue))
                        return (Max.Value, true);
                    else if (f < Min.ToSingle(float.MinValue))
                        return (Min.Value, true);
                    break;

                case double d:
                    if (d > Max.ToDouble(double.MaxValue))
                        return (Max.Value, true);
                    else if (d < Min.ToDouble(double.MinValue))
                        return (Min.Value, true);
                    break;

                case decimal d:
                    if (d > Max.ToDecimal(decimal.MaxValue))
                        return (Max.Value, true);
                    else if (d < Min.ToDecimal(decimal.MinValue))
                        return (Min.Value, true);
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
        [Parameter] public GenericNumber<T> Min { get; set; }

        /// <summary>
        /// The maximum value for the input.
        /// </summary>
        [Parameter] public GenericNumber<T> Max { get; set; }

        /// <summary>
        /// The increment added/subtracted by the spin buttons.
        /// </summary>
        [Parameter] public GenericNumber<T> Step { get; set; }
    }
}
