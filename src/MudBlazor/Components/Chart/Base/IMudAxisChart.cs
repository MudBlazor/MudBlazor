// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

#nullable enable
namespace MudBlazor.Charts;

public record struct AxisGridData(int LowestHorizontalLine, int HorizontalLineCount, double YAxisTicks, double BoundWidth, double BoundHeight);

public interface IMudAxisChart : IMudChart
{
    public AxisGridData? SharedData { get; set; }
    public IMudChart? OverlayChart { get; set; }
    public RenderFragment? OverlayContent { get; set; }
}
