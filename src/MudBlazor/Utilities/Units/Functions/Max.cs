// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities;

internal sealed class Max : LengthPercentage
{
    public Max(LengthPercentage a, LengthPercentage b) => Value = $"max({a}, {b})";
}
