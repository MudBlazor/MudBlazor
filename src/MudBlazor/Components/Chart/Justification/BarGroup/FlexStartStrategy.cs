// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Justification.BarGroup;

internal class FlexStartStrategy : IBarGroupPositionStrategy
{
    public double[] CalculatePositions(BarGroupContext ctx)
    {
        var positions = new double[ctx.ColumnsPerDataSet];
        var barGapOffset = ctx.BarGap / 2;
        var spacingRatioOffset = ctx.SeriesSpacingRatio / 2;
        var spaceBetweenGroups = Math.Max(ctx.DataSetCount == 1 ? 0 : ctx.BarGroupWidth,
                                          ctx.CalculateSpaceWidth(ctx.HorizontalSpace, ctx.ColumnsPerDataSet));

        var start = ctx.HorizontalStartSpace + (ctx.BarGroupWidth / 2);

        for (var i = 0; i < ctx.ColumnsPerDataSet; i++)
        {
            positions[i] = start + (i * (spaceBetweenGroups + ctx.BarWidth + (barGapOffset * spacingRatioOffset)));
        }

        return positions;
    }
}
