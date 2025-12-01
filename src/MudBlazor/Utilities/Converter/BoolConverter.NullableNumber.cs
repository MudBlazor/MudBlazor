// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;

namespace MudBlazor.Utilities.Converter;

#nullable enable
internal partial class BoolConverter
{
    internal sealed class NullableNumberConverter<T> : IReversibleConverter<T?, bool?> where T : struct, INumber<T>
    {
        public bool? Convert(T? input) => input switch
        {
            null => null,
            > 0 => true,
            _ => false
        };

        public T? ConvertBack(bool? input) => input switch
        {
            true => T.One,
            false => T.Zero,
            _ => null
        };

        public static readonly NullableNumberConverter<T> Instance = new();
    }
}
