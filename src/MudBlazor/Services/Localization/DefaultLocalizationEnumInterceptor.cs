// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Default implementation of the <see cref="ILocalizationEnumInterceptor"/> interface.
/// Provides localization for enumeration values using display attributes and string localizers.
/// </summary>
internal class DefaultLocalizationEnumInterceptor : ILocalizationEnumInterceptor
{
    private readonly ILocalizationInterceptor _interceptor;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultLocalizationEnumInterceptor"/> class.
    /// </summary>
    /// <param name="interceptor">The localization interceptor to use for handling translations.</param>
    public DefaultLocalizationEnumInterceptor(ILocalizationInterceptor interceptor)
    {
        ArgumentNullException.ThrowIfNull(interceptor);

        _interceptor = interceptor;
    }

    /// <inheritdoc />
    public string Handle(Enum enumeration)
    {
        var enumType = enumeration.GetType();
        var memberInfo = enumType.GetField(enumeration.ToString(), BindingFlags.Public | BindingFlags.Static);

        return GetDisplayName(memberInfo);
    }

    private string GetDisplayName(MemberInfo? member)
    {
        if (member is null)
        {
            return string.Empty;
        }

        var display = member.GetCustomAttribute<DisplayAttribute>(inherit: false);
        if (display is not null)
        {
            // Note [Display(Name = "")] is allowed but we will not attempt to localize the empty name.
            var name = display.GetName();
            if (!string.IsNullOrEmpty(name) && display.ResourceType is null)
            {
                var localization = _interceptor.Handle(name);
                if (!localization.ResourceNotFound)
                {
                    return localization.Value;
                }
            }

            return name ?? member.Name;
        }

        return member.Name;
    }
}
