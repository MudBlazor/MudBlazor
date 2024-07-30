// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Localization;

#nullable enable
/// <summary>
/// Provides functionality to localize enumeration values.
/// </summary>
public interface IEnumLocalizer
{
    /// <summary>
    /// Localizes the specified enumeration value.
    /// </summary>
    /// <param name="enumValue">The enumeration value to be localized.</param>
    /// <returns>The localized representation of the enumeration value.</returns>
    string Handle(Enum enumValue);
}
