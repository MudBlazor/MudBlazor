// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MudBlazor.Localization;

#nullable enable
/// <summary>
/// Default implementation of the <see cref="IEnumLocalizer"/> interface.
/// Provides localization for enumeration values using display attributes and string localizers.
/// </summary>
internal class DefaultEnumLocalizer : IEnumLocalizer
{
    private readonly InternalMudLocalizer _internalMudLocalizer;

    public DefaultEnumLocalizer(InternalMudLocalizer internalMudLocalizer)
    {
        _internalMudLocalizer = internalMudLocalizer;
    }

    /// <inheritdoc />
    public string Handle(Enum enumValue)
    {
        var enumType = enumValue.GetType();
        var memberInfo = enumType.GetField(enumValue.ToString(), BindingFlags.Public | BindingFlags.Static);

        return GetDisplayName(memberInfo);
    }

    private string GetDisplayName(MemberInfo? member)
    {
        var display = member?.GetCustomAttribute<DisplayAttribute>(inherit: false);
        if (display is not null)
        {
            // Note [Display(Name = "")] is allowed but we will not attempt to localize the empty name.
            var name = display.GetName();
            if (!string.IsNullOrEmpty(name) && display.ResourceType is null)
            {
                name = _internalMudLocalizer[name];
            }

            return name ?? member?.Name ?? string.Empty;
        }

        return member?.Name ?? string.Empty;
    }
}
