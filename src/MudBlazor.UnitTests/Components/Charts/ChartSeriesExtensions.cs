using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.UnitTests.Components
{
    internal static class ChartSeriesExtensions
    {
        internal static bool TryGetIndexOfDataValue(this ChartSeries chartSeries, int seriesIndex, double value, out int dataIndex)
        {
            dataIndex = -1;

            for (var i = 0; i < chartSeries.Data.Length; i++)
            {
                if (chartSeries.Data[i] == value)
                {
                    dataIndex = i;
                    return true;
                }
            }

            return false;
        }

        internal static bool TryGetIndexOfDataValue(this IEnumerable<ChartSeries> chartSeries, int seriesIndex, double value, out int dataIndex) => TryGetIndexOfDataValue(chartSeries.ElementAt(seriesIndex), seriesIndex, value, out dataIndex);
    }
}
