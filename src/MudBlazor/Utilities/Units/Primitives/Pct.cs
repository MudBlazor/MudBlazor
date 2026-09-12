// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities;

internal sealed class Pct : LengthPercentage
{
    public Pct(double value) => Value = $"{value}%";
}
