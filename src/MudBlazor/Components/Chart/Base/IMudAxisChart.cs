// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Microsoft.AspNetCore.Components;

#nullable enable
namespace MudBlazor.Charts;

public record struct AxisGridData<T>(int LowestHorizontalLine, int HorizontalLineCount, T YAxisTicks, double BoundWidth, double BoundHeight)
    where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable;

public interface IMudAxisChart<T> : IMudChart<T> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
{
    public AxisGridData<T>? SharedData { get; set; }
    public IMudChart<T>? OverlayChart { get; set; }
    public RenderFragment? OverlayContent { get; set; }
}
