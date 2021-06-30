using System;
using System.Collections.Generic;
using MudBlazor.EnhanceChart.Internal;

namespace MudBlazor.EnhanceChart
{
    /// <summary>
    /// Representing a single point in a data series
    /// </summary>
    public interface IDataPoint : IChartDataElement
    {
        /// <summary>
        /// The value of the data point
        /// </summary>
        Double Value { get; }

        /// <summary>
        /// A label (name) for this point
        /// </summary>
        String Label { get;  }
    }
}
