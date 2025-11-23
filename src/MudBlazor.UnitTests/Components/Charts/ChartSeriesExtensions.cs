using System.Numerics;

namespace MudBlazor.UnitTests.Components
{
    internal static class ChartSeriesExtensions
    {
        internal static bool TryGetIndexOfDataValue<T>(this ChartSeries<T> chartSeries, int seriesIndex, T value, out int dataIndex) where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
        {
            dataIndex = -1;

            for (var i = 0; i < chartSeries.Data.Values.Count; i++)
            {
                if (chartSeries.Data[i].Y == value)
                {
                    dataIndex = i;
                    return true;
                }
            }

            return false;
        }

        internal static bool TryGetIndexOfDataValue<T>(this IEnumerable<ChartSeries<T>> chartSeries, int seriesIndex, T value, out int dataIndex) where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
            => TryGetIndexOfDataValue(chartSeries.ElementAt(seriesIndex), seriesIndex, value, out dataIndex);
    }
}
