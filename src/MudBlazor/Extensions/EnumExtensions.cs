#nullable enable
using System;
using System.ComponentModel;
using System.Reflection;

namespace MudBlazor.Extensions;

public static class EnumExtensions
{
    public static string ToDescriptionString<T>(this T val)
        where T : Enum
    {
        var field = typeof(T).GetField(val.ToString())!;
        var attribute = field.GetCustomAttribute<DescriptionAttribute>(false);

        return attribute != null
            ? attribute.Description
            : val.ToString().ToLower();
    }
}
