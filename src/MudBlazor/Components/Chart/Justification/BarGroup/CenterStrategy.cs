// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Justification.BarGroup;

internal class CenterStrategy : IBarGroupPositionStrategy
{
    public double[] CalculatePositions(BarGroupContext ctx)
    {
        var positions = new double[ctx.ColumnsPerDataSet];
        var barWidthOffset = ctx.BarWidth / 2;
        var barGapOffset = ctx.BarGap / 2;
        var spaceBetweenGroups = Math.Max(ctx.DataSetCount == 1 ? 0 : ctx.BarGroupWidth,
                                             ctx.CalculateSpaceWidth(ctx.HorizontalSpace, ctx.ColumnsPerDataSet));

        var start = ctx.HorizontalStartSpace + ((ctx.HorizontalSpace - (ctx.DataSetCount * ctx.BarGroupWidth)) / 2);

        var centerOffset = ctx.DataSetCount switch
        {
            <= 2 => ctx.BarGroupWidth / 2,
            3 => ctx.BarGroupWidth + barWidthOffset,
            _ => ((ctx.BarGroupWidth + ctx.BarWidth) * (ctx.SpacesPerGroup / 2.0)) - barWidthOffset
        };

        var spacingOffset = spaceBetweenGroups / 2;
        var leftShift = ctx.GapsPerDataSet * (barWidthOffset + spacingOffset);

        for (var i = 0; i < ctx.ColumnsPerDataSet; i++)
        {
            positions[i] = start + centerOffset - leftShift + (ctx.SpacesPerGroup * barGapOffset) +
                           (i * (spaceBetweenGroups + ctx.BarWidth + (barGapOffset * ctx.SeriesSpacingRatio / 2)));
        }

        return positions;
    }
}
