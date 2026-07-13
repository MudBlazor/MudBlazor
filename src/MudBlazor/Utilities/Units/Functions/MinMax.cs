// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities;

internal sealed class MinMax : CssStringBuilder, ITrackBreadth
{
    public MinMax(IInflexibleBreadth min, ITrackBreadth max) => Value = $"minmax({min}, {max})";
}
