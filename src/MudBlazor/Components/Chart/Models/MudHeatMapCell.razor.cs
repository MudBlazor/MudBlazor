// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Numerics;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Represents a single cell in a <see cref="HeatMap{T}"/>. You can override the value from the <see cref="ChartSeries{T}"/> 
    /// or provide a custom graphic to be shown inside the cell. You should provide a width and height for the custom graphic you are including
    /// so the Heat Map can resize it dynamically. 
    /// </summary>
    public partial class MudHeatMapCell<T> : MudComponentBase where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
    {
        [CascadingParameter]
        internal IMudChart<T>? Parent { get; set; }

        /// <summary>
        /// The row of the cell you want to modify. Rows use a 0 based index.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public int Row { get; set; }

        /// <summary>
        /// The column of the cell you want to modify. Columns use a 0 based index.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public int Column { get; set; }

        /// <summary>
        /// If supplied this will overwrite the value in ChartSeries
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public T? Value { get; set; }

        /// <summary>
        /// Optional, Override the color of the cell
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public MudColor? MudColor { get; set; }

        /// <summary>
        /// Optional, The width of the custom svg element you want to include. Please note the custom svg elements you provide are resized according to this value if supplied.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public int? Width { get; set; }

        /// <summary>
        /// Optional, The height of the custom svg element you want to include. Please note the custom svg elements you provide are resized according to this value if supplied.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public int? Height { get; set; }

        /// <summary>
        /// Optional, setting this will set the minimum value for the heatmap range, by default the range is calculated from the data. This only needs to be set on one
        /// <see cref="MudHeatMapCell{T}"/> in the <see cref="HeatMap{T}"/>."/> Only the last value set will be used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public T? MinValue { get; set; }

        /// <summary>
        /// Optional, setting this will set the maximum value for the heatmap range, by default the range is calculated from the data. This only needs to be set on one
        /// <see cref="MudHeatMapCell{T}"/> in the <see cref="HeatMap{T}"/>."/> Only the last value set will be used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public T? MaxValue { get; set; }

        /// <summary>
        /// Optional, The custom svg element you want to include
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public RenderFragment? ChildContent { get; set; }

        protected override void OnInitialized()
        {
            if (Parent is HeatMap<T> heatMap)
            {
                heatMap.AddCell(this);
            }
            else
            {
                throw new InvalidOperationException("MudHeatMapCell must be used inside a MudChart component.");
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (Parent is HeatMap<T> heatMap)
            {
                heatMap.RebuildChart();
            }
        }
    }
}

