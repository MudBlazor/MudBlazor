// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor;

/// <summary>
/// Indicates how string values are compared for a <see cref="MudDataGrid{T}"/> filter.
/// </summary>
public enum DataGridFilterCaseSensitivity
{
    /// <summary>
    /// Text is compared using <see cref="StringComparison.Ordinal"/>.
    /// </summary>
    /// <remarks>
    /// When using this value, casing of text matters.
    /// </remarks>
    Default,

    /// <summary>
    /// Text is compared using <see cref="StringComparison.OrdinalIgnoreCase"/>.
    /// </summary>
    /// <remarks>
    /// When using this value, casing of text does not matter.
    /// </remarks>
    CaseInsensitive,

    /// <summary>
    /// Excludes any <see cref="StringComparison" /> value for text comparisons.
    /// </summary>
    /// <remarks>
    /// This is typically used for Entity Framework expressions, which do not support string comparisons.
    /// </remarks>
    Ignore
}
