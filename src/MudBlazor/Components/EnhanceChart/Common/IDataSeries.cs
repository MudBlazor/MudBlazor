using System;
using System.Collections.Generic;

namespace MudBlazor.EnhanceChart
{
    /// <summary>
    /// Representing a dataseries
    /// </summary>
    public interface IDataSeries : IChartDataElement
    {
        /// <summary>
        /// The values of this series
        /// </summary>
        IList<Double> Points { get; }
    }
}
