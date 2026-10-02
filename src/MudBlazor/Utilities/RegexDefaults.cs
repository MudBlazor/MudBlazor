// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities;

/// <summary>
/// Shared defaults applied to every regular expression in MudBlazor.
/// </summary>
internal static class RegexDefaults
{
    /// <summary>
    /// The match timeout, in milliseconds.
    /// </summary>
    /// <remarks>
    /// Guards against catastrophic backtracking in user-supplied patterns such as masks.
    /// Deliberately generous so a legitimate pattern never hits it, even on a loaded server.
    /// This is a constant because <see cref="System.Text.RegularExpressions.GeneratedRegexAttribute"/> requires a compile-time value.
    /// </remarks>
    public const int MatchTimeoutMilliseconds = 1000;

    /// <summary>
    /// <see cref="MatchTimeoutMilliseconds"/> for APIs which take a <see cref="TimeSpan"/>.
    /// </summary>
    public static readonly TimeSpan MatchTimeout = TimeSpan.FromMilliseconds(MatchTimeoutMilliseconds);
}
