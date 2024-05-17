// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Indicates how string values are compared for a <see cref="MudDataGrid{T}"/> filter.
/// </summary>
public enum DataGridFilterCaseSensitivity
{
    /// <summary>
    /// The casing of text matters for comparisons.
    /// </summary>
    Default,

    /// <summary>
    /// The casing of text does not matter for comparisons.
    /// </summary>
    CaseInsensitive,

    /// <summary>
    /// The casing of text is ignored for comparisons.
    /// </summary>
    Ignore
}
