﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts.SVG.Models;

namespace MudBlazor.Charts
{
    /// <summary>
    /// Represents a set of text labels which describe data values in a <see cref="MudChart"/>.
    /// </summary>
    public partial class Legend : MudChartBase
    {
        /// <summary>
        /// Gets or sets the chart, if any, containing this component.
        /// </summary>
        [CascadingParameter]
        public MudChart MudChartParent { get; set; }

        /// <summary>
        /// Gets or sets the data labels for this legend.
        /// </summary>
        [Parameter]
        public List<SvgLegend> Data { get; set; } = new List<SvgLegend>();

        private string GetCheckBoxStyle(int index)
        {
            var color = MudChartParent.ChartOptions.ChartPalette.GetValue(index % ChartOptions.ChartPalette.Length);
            return $"--checkbox-color: {color};";
        }
    }
}
