// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor.Charts
{
#nullable enable
    /// <summary>
    /// Represents a single cell in a heat map chart.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HeatMapCell<T> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
    {
        /// <summary>
        /// The row index of the cell in the heat map.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// The column index of the cell in the heat map.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// The value associated with the cell, which determines its color intensity.
        /// </summary>
        public T? Value { get; set; }

        /// <summary>
        /// The color of the cell. If not set, the color will be determined based on the value and the chart's color palette.
        /// </summary>
        public MudColor? MudColor { get; set; }

        /// <summary>
        /// The width of the cell in pixels. If not set, a default width will be used.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// The height of the cell in pixels. If not set, a default height will be used.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// A custom RenderFragment to render inside the cell. If set, this will override the default cell rendering.
        /// </summary>
        public RenderFragment? CustomFragment { get; set; }
    }
}
