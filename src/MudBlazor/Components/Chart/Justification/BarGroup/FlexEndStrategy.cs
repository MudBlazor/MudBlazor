// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Justification.BarGroup;

internal class FlexEndStrategy : IBarGroupPositionStrategy
{
    public double[] CalculatePositions(BarGroupContext ctx)
    {
        var positions = new double[ctx.ColumnsPerDataSet];
        var barGapOffset = ctx.BarGap / 2;
        var spaceBetweenGroups = Math.Max(ctx.DataSetCount == 1 ? 0 : ctx.BarGroupWidth,
                                          ctx.CalculateSpaceWidth(ctx.HorizontalSpace, ctx.ColumnsPerDataSet));

        var start = ctx.HorizontalSpace + ctx.HorizontalEndSpace -
                       (ctx.DataSetCount * ctx.BarGroupWidth + ctx.GapsPerDataSet * (spaceBetweenGroups + ctx.BarWidth)) +
                       ctx.SpacesPerGroup * barGapOffset;

        var centerOffset = ctx.DataSetCount switch
        {
            <= 2 => (ctx.BarGroupWidth / 2) + ctx.SpacesPerGroup * barGapOffset,
            3 => ctx.BarGroupWidth * ctx.SpacesPerGroup,
            _ => (ctx.BarGroupWidth * ctx.SpacesPerGroup) + (ctx.BarWidth * (ctx.SpacesPerGroup - 2) * 0.5)
        };

        var step = spaceBetweenGroups + ctx.BarWidth - ((ctx.BarGap - ctx.BarWidth) / 64);

        for (var i = 0; i < ctx.ColumnsPerDataSet; i++)
        {
            positions[i] = start + centerOffset + i * step;
        }

        return positions;
    }
}

