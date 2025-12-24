// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Justification.BarGroup;

#nullable enable
internal class SpaceEvenlyStrategy : IBarGroupPositionStrategy
{
    public double[] CalculatePositions(BarGroupContext ctx)
    {
        var positions = new double[ctx.ColumnsPerDataSet];

        var availableSpace = ctx.HorizontalSpace - ((ctx.BarWidth * ctx.DataSetCount * ctx.ColumnsPerDataSet) +
                                                    (ctx.BarGap * ctx.SpacesPerGroup * ctx.ColumnsPerDataSet));

        var evenSpace = availableSpace / (ctx.ColumnsPerDataSet + 1);

        positions[0] = ctx.HorizontalStartSpace + evenSpace + (ctx.BarGroupWidth / 2);

        for (var i = 1; i < ctx.ColumnsPerDataSet; i++)
        {
            positions[i] = positions[i - 1] + evenSpace + (ctx.BarWidth * ctx.DataSetCount) + (ctx.BarGap * ctx.SpacesPerGroup);
        }

        return positions;
    }
}
