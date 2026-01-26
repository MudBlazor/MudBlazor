// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Justification.BarGroup;

#nullable enable
internal class SpaceAroundStrategy : IBarGroupPositionStrategy
{
    public double[] CalculatePositions(BarGroupContext ctx)
    {
        var positions = new double[ctx.ColumnsPerDataSet];
        var spaceAround = ctx.HorizontalSpace / (ctx.ColumnsPerDataSet * 2);
        var barWidthOffset = ctx.DataSetCount == 1 ? 0 : ctx.BarWidth / 2;
        var offset = ctx.HorizontalStartSpace + spaceAround - barWidthOffset;

        for (var i = 0; i < ctx.ColumnsPerDataSet; i++)
        {
            positions[i] = offset + i * spaceAround * 2;
        }

        return positions;
    }
}
