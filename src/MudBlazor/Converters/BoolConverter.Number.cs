// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace MudBlazor;

internal partial class BoolConverter
{
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.Interfaces)]
    internal sealed class NumberConverter<T> : IReversibleConverter<T, bool?> where T : INumber<T>
    {
        public bool? Convert(T input) => !T.IsZero(input);

        public T ConvertBack(bool? input) => input switch
        {
            null => T.Zero,
            false => T.Zero,
            true => T.One
        };

        public static readonly NumberConverter<T> Instance = new();
    }
}
