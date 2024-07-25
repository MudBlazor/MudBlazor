// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Localization;

namespace MudBlazor.Utilities.Localization;

#nullable enable
/// <summary>
/// Default implementation of the <see cref="IEnumLocalizationStrategy"/> interface.
/// Provides localization for enumeration values using display attributes and string localizers.
/// </summary>
internal class DefaultEnumLocalizationStrategy : IEnumLocalizationStrategy
{
    /// <summary>
    /// Singleton instance of the <see cref="DefaultEnumLocalizationStrategy"/> class.
    /// </summary>
    public static readonly DefaultEnumLocalizationStrategy Instance = new();

    /// <inheritdoc />
    public string Handle(Enum enumValue, IStringLocalizer? localizer)
    {
        var enumType = enumValue.GetType();
        var memberInfo = enumType.GetMember(enumValue.ToString()).FirstOrDefault();

        return GetDisplayName(memberInfo, localizer);
    }

    private static string GetDisplayName(MemberInfo? member, IStringLocalizer? localizer)
    {
        var display = member?.GetCustomAttribute<DisplayAttribute>(inherit: false);
        if (display is not null)
        {
            // Note [Display(Name = "")] is allowed but we will not attempt to localize the empty name.
            var name = display.GetName();
            if (localizer is not null && !string.IsNullOrEmpty(name) && display.ResourceType is null)
            {
                name = localizer[name];
            }

            return name ?? member?.Name ?? string.Empty;
        }

        return member?.Name ?? string.Empty;
    }
}
