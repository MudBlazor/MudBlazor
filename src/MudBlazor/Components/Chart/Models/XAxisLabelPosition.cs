// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

namespace MudBlazor
{
    /// <summary>
    /// Indicates the position of the X axis labels as either top or bottom in a <see cref="ChartType.HeatMap"/>
    /// </summary>
    public enum XAxisLabelPosition
    {
        /// <summary>
        /// The X axis labels are displayed at the top of the chart centered horizontally.
        /// </summary>
        Top,
        /// <summary>
        /// The X axis labels are displayed at the bottom of the chart centered horizontally.
        /// </summary>
        Bottom,
        /// <summary>
        /// Do not include X axis labels
        /// </summary>
        None,
    }
}
