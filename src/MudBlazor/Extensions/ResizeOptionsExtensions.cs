// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using MudBlazor.Services;

namespace MudBlazor.Extensions;

#nullable enable
internal static class ResizeOptionsExtensions
{
    /// <summary>
    /// Clones the <paramref name="options"/> object by creating a new instance of <see cref="ResizeOptions"/> with the same property values.
    /// </summary>
    /// <param name="options">The <see cref="ResizeOptions"/> object to clone.</param>
    /// <returns>A new instance of <see cref="ResizeOptions"/> with the same property values as the original object.</returns>
    internal static ResizeOptions Clone(this ResizeOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

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
