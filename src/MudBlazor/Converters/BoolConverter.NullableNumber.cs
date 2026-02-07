// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace MudBlazor;

internal partial class BoolConverter
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.Interfaces)]
    internal sealed class NullableNumberConverter<T> : IReversibleConverter<T?, bool?> where T : struct, INumber<T>
    {
        public bool? Convert(T? input) => input switch
        {
            null => null,
            _ => !T.IsZero(input.Value)
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
