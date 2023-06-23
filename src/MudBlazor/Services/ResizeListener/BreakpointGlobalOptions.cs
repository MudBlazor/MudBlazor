// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using MudBlazor.Services;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Shares breakpoint definition between <see cref="BreakpointService"/> and <see cref="ResizeListenerService"/>.
/// </summary>
/// <remarks>
/// This class is not really needed when <see cref="ResizeListenerService"/> will be removed, for now it's for consistency.
/// </remarks>
internal class BreakpointGlobalOptions
{
    public static Dictionary<Breakpoint, int> DefaultBreakpointDefinitions { get; set; } = new()
    {
        [Breakpoint.Xxl] = 2560,
        [Breakpoint.Xl] = 1920,
        [Breakpoint.Lg] = 1280,
        [Breakpoint.Md] = 960,
        [Breakpoint.Sm] = 600,
        [Breakpoint.Xs] = 0,
    };

    public static Dictionary<Breakpoint, int> GetDefaultOrUserDefinedBreakpointDefinition(ResizeOptions options)
    {
        if (options.BreakpointDefinitions is not null && options.BreakpointDefinitions.Count != 0)
        {
            //Copy as we don't want any unexpected modification
            return options.BreakpointDefinitions.ToDictionary(entry => entry.Key, entry => entry.Value);
        }

        return DefaultBreakpointDefinitions.ToDictionary(entry => entry.Key, entry => entry.Value);
    }
}
