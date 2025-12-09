// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
internal partial class BoolConverter
{
    internal sealed class ObjectBoolConverter : IReversibleConverter<object?, bool?>
    {
        public bool? Convert(object? input)
        {
            return input switch
            {
                null => null,
                bool b => BoolIdentity.Instance.Convert(b),
                char c => NumberConverter<char>.Instance.Convert(c),
                byte b => NumberConverter<byte>.Instance.Convert(b),
                sbyte sb => NumberConverter<sbyte>.Instance.Convert(sb),
                short sh => NumberConverter<short>.Instance.Convert(sh),
                ushort us => NumberConverter<ushort>.Instance.Convert(us),
                int i => NumberConverter<int>.Instance.Convert(i),
                uint ui => NumberConverter<uint>.Instance.Convert(ui),
                long l => NumberConverter<long>.Instance.Convert(l),
                ulong ul => NumberConverter<ulong>.Instance.Convert(ul),
                double d => NumberConverter<double>.Instance.Convert(d),
                float f => NumberConverter<float>.Instance.Convert(f),
                decimal m => NumberConverter<decimal>.Instance.Convert(m),
                string s => StringConverter.Instance.Convert(s),
                _ => throw new InvalidOperationException($"Cannot convert type {input.GetType()} to bool?")
            };
        }

        public object? ConvertBack(bool? value)
        {
            return value;
        }

        public static readonly ObjectBoolConverter Instance = new();
    }
}
