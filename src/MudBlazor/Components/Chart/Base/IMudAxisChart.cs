// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

#nullable enable
namespace MudBlazor.Charts;

public record struct PlotArea(double Width, double Height, int LowestHorizontalLine, double XAxisLines, double YAxisTicks);

public interface IMudAxisChart : IMudChart
{
    public PlotArea? PlotArea { get; set; }
    public IMudChart? OverlayChart { get; set; }
    public RenderFragment? OverlayContent { get; set; }
}
