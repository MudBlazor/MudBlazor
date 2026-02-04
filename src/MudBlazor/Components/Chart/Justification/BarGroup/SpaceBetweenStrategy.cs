// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Justification.BarGroup;

internal class SpaceBetweenStrategy : IBarGroupPositionStrategy
{
    public double[] CalculatePositions(BarGroupContext ctx)
    {
        var positions = new double[ctx.ColumnsPerDataSet];

        if (ctx.ColumnsPerDataSet == 1)
        {
            positions[0] = ctx.HorizontalStartSpace + (ctx.BarGroupWidth / 2) +
                           (ctx.HorizontalSpace - (ctx.BarWidth * ctx.DataSetCount) - (ctx.BarGap * ctx.SpacesPerGroup)) / 2;
            return positions;
        }

        var availableSpace = ctx.HorizontalSpace - ((ctx.BarWidth * ctx.DataSetCount * ctx.ColumnsPerDataSet) +
                                                       (ctx.BarGap * ctx.SpacesPerGroup * ctx.ColumnsPerDataSet));

        var spaceBetween = availableSpace / Math.Max(1, ctx.GapsPerDataSet);
        var start = ctx.HorizontalStartSpace + (ctx.BarGroupWidth / 2);

        for (var i = 0; i < ctx.ColumnsPerDataSet; i++)
        {
            positions[i] = start + i * ((ctx.BarWidth * ctx.DataSetCount) + (ctx.BarGap * ctx.SpacesPerGroup) + spaceBetween);
        }

        return positions;
    }
}

