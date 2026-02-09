// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Justification.StackedBars;

internal class FlexStartStrategy : IStackedBarPositionStrategy
{
    public double[] CalculatePositions(StackedBarContext ctx)
    {
        var positions = new double[ctx.MaxColumns];
        var startingPoint = ctx.HorizontalStartSpace + (ctx.BarWidth / 2);

        for (var i = 0; i < ctx.MaxColumns; i++)
            positions[i] = startingPoint + i * (ctx.SpaceBetweenBars + ctx.BarWidth);

        return positions;
    }
}
