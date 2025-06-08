// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Justification.StackedBars;

internal class FlexEndStrategy : IStackedBarPositionStrategy
{
    public double[] CalculatePositions(StackedBarContext ctx)
    {
        var positions = new double[ctx.MaxColumns];
        var totalWidth = (ctx.MaxColumns * ctx.BarWidth) + (ctx.SpaceBetweenBars * (ctx.MaxColumns - 1));
        var start = ctx.HorizontalSpace + ctx.HorizontalEndSpace - totalWidth + (ctx.BarWidth / 2);

        for (var i = 0; i < ctx.MaxColumns; i++)
            positions[i] = start + i * (ctx.SpaceBetweenBars + ctx.BarWidth);

        return positions;
    }
}
