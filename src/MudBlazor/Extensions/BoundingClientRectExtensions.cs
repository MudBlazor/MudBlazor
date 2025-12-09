// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Interop;

namespace MudBlazor.Extensions;

#nullable enable
public static class BoundingClientRectExtensions
{
    public static bool IsEqualTo(this BoundingClientRect? sourceRect, BoundingClientRect? targetRect, double tolerance = 0.00001)
    {
        if (sourceRect is null || targetRect is null) return false;
        return Math.Abs(sourceRect.Top - targetRect.Top) < tolerance
               && Math.Abs(sourceRect.Left - targetRect.Left) < tolerance
               && Math.Abs(sourceRect.Width - targetRect.Width) < tolerance
               && Math.Abs(sourceRect.Height - targetRect.Height) < tolerance
               && Math.Abs(sourceRect.WindowHeight - targetRect.WindowHeight) < tolerance
               && Math.Abs(sourceRect.WindowWidth - targetRect.WindowWidth) < tolerance
               && Math.Abs(sourceRect.ScrollX - targetRect.ScrollX) < tolerance
               && Math.Abs(sourceRect.ScrollY - targetRect.ScrollY) < tolerance;
    }
}
