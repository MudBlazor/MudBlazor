// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using MudBlazor.Services;

namespace MudBlazor.Extensions;

#nullable enable
internal static class ResizeOptionsExtensions
{
    internal static ResizeOptions Clone(this ResizeOptions options)
    {
        return new ResizeOptions
        {
            BreakpointDefinitions = (options.BreakpointDefinitions ?? new Dictionary<Breakpoint, int>()).ToDictionary(entry => entry.Key, entry => entry.Value),
            EnableLogging = options.EnableLogging,
            NotifyOnBreakpointOnly = options.NotifyOnBreakpointOnly,
            ReportRate = options.ReportRate,
            SuppressInitEvent = options.SuppressInitEvent
        };
    }
}
