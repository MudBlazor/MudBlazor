// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Justification.StackedBars;

internal class SpaceAroundStrategy : IStackedBarPositionStrategy
{
    public double[] CalculatePositions(StackedBarContext ctx)
    {
        var positions = new double[ctx.MaxColumns];
        var spaceAround = ctx.HorizontalSpace / (ctx.MaxColumns * 2);

        for (var i = 0; i < ctx.MaxColumns; i++)
            positions[i] = ctx.HorizontalStartSpace + spaceAround + i * (spaceAround * 2);

        return positions;
    }
}
