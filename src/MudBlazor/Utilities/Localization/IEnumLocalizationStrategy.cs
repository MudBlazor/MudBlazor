// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Extensions.Localization;

namespace MudBlazor.Utilities.Localization;

#nullable enable
/// <summary>
/// Defines a strategy for localizing enumeration values.
/// </summary>
public interface IEnumLocalizationStrategy
{
    /// <summary>
    /// Handles the localization of the specified enumeration value using the provided string localizer.
    /// </summary>
    /// <param name="enumValue">The enumeration value to be localized.</param>
    /// <param name="localizer">The string localizer to use for localization, or null if no localizer is available.</param>
    /// <returns>The localized string representation of the enumeration value.</returns>
    string Handle(Enum enumValue, IStringLocalizer? localizer);
}
