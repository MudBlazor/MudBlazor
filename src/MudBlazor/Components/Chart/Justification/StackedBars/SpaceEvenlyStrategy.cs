// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Justification.StackedBars;

internal class SpaceEvenlyStrategy : IStackedBarPositionStrategy
{
    public double[] CalculatePositions(StackedBarContext ctx)
    {
        var positions = new double[ctx.MaxColumns];
        var totalBarWidth = ctx.MaxColumns * ctx.BarWidth;
        var remainingSpace = ctx.HorizontalSpace - totalBarWidth;
        var evenSpace = remainingSpace / (ctx.MaxColumns + 1);

        positions[0] = ctx.HorizontalStartSpace + evenSpace + (ctx.BarWidth / 2);

        for (var i = 1; i < ctx.MaxColumns; i++)
            positions[i] = positions[i - 1] + ctx.BarWidth + evenSpace;

        return positions;
    }
}
