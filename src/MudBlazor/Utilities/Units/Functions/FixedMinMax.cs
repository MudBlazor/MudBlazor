// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities;

internal sealed class FixedMinMax : TrackBreadth, FixedSize
{
    public FixedMinMax(LengthPercentage min, TrackBreadth max) => Value = $"minmax({min}, {max})";
}
