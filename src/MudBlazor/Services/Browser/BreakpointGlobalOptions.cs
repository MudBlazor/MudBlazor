// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Services;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Breakpoint definitions for <see cref="BrowserViewportService"/>.
/// </summary>
internal class BreakpointGlobalOptions
{
    /// <summary>
    /// Default  breakpoint definitions
    /// </summary>
    internal static Dictionary<Breakpoint, int> DefaultBreakpointDefinitions { get; set; } = new()
    {
        [Breakpoint.Xxl] = 2560,
        [Breakpoint.Xl] = 1920,
        [Breakpoint.Lg] = 1280,
        [Breakpoint.Md] = 960,
        [Breakpoint.Sm] = 600,
        [Breakpoint.Xs] = 0,
    };

    /// <summary>
    /// Retrieves the default or user-defined breakpoint definitions based on the provided <paramref name="options"/>.
    /// If user-defined breakpoint definitions are available in the <paramref name="options"/>, a copy is returned to prevent unintended modifications.
    /// Otherwise, the default <see cref="DefaultBreakpointDefinitions"/> breakpoint definitions are returned.
    /// </summary>
    /// <param name="options">The resize options containing breakpoint definitions, if any.</param>
    /// <param name="globalOptions">The global resize options containing breakpoint definitions, if any.</param>
    /// <returns>A dictionary containing the breakpoint definitions.</returns>
    internal static Dictionary<Breakpoint, int> GetDefaultOrUserDefinedBreakpointDefinition(ResizeOptions options, ResizeOptions? globalOptions = null)
    {
        // Priority of breakpoint definitions:
        // 1. Component-defined breakpoints
        // 2. Global breakpoints (if component breakpoints are null)
        // 3. Default breakpoints (if both component and global breakpoints are null)

        if (options.BreakpointDefinitions is not null && options.BreakpointDefinitions.Count != 0)
        {
            // Copy as we don't want any unexpected modification
            return CopyBreakpoints(options.BreakpointDefinitions);
        }

        if (globalOptions?.BreakpointDefinitions is not null && globalOptions.BreakpointDefinitions.Count != 0)
        {
            return CopyBreakpoints(globalOptions.BreakpointDefinitions);
        }

        return CopyBreakpoints(DefaultBreakpointDefinitions);
    }

    private static Dictionary<Breakpoint, int> CopyBreakpoints(Dictionary<Breakpoint, int> originalDictionary) => originalDictionary
        .ToDictionary(entry => entry.Key, entry => entry.Value);
}
