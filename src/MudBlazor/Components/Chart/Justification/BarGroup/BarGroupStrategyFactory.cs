// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Justification.BarGroup;

internal interface IBarGroupPositionStrategy
{
    double[] CalculatePositions(BarGroupContext ctx);
}

internal static class BarGroupStrategyFactory
{
    internal static IBarGroupPositionStrategy GetStrategy(Justify justify) => justify switch
    {
        Justify.FlexStart => new FlexStartStrategy(),
        Justify.FlexEnd => new FlexEndStrategy(),
        Justify.Center => new CenterStrategy(),
        Justify.SpaceBetween => new SpaceBetweenStrategy(),
        Justify.SpaceAround => new SpaceAroundStrategy(),
        Justify.SpaceEvenly => new SpaceEvenlyStrategy(),
        _ => throw new NotSupportedException($"Unsupported Justification: {justify}")
    };
}

internal class BarGroupContext
{
    public int ColumnsPerDataSet { get; init; }
    public int DataSetCount { get; init; }
    public int SpacesPerGroup => DataSetCount - 1;
    public int GapsPerDataSet => ColumnsPerDataSet - 1;
    public double HorizontalSpace { get; init; }
    public double BarWidth { get; init; }
    public double BarGap { get; init; }
    public double BarGroupWidth { get; init; }
    public double HorizontalStartSpace { get; init; }
    public double HorizontalEndSpace { get; init; }
    public double SeriesSpacingRatio { get; init; }
    public Func<double, int, int> CalculateSpaceWidth { get; init; }
}
