// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
public sealed class SvgPolygon : SvgPath
{
    /// <summary>
    /// Stores the coordinates of individual data points for charts like Radar Chart.
    /// </summary>
    public List<SvgPathPoint> Points { get; set; } = [];
}
