// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities;

internal sealed class Rem : LengthPercentage
{
    public Rem(double value) => Value = $"{value}rem";
}
