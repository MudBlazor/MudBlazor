using System;
using System.ComponentModel;

namespace MudBlazor.Extensions
{
#nullable enable
    public static class EnumExtensions
    {
        [Obsolete("Please use the auto-generated ToDescriptionString method instead or implement your own extension method.")]
        public static string ToDescriptionString(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            if (field is null)
            {
                return value.ToString().ToLower();
            }

            var attributes = Attribute.GetCustomAttributes(field, typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            return attributes is { Length: > 0 }
                ? attributes[0].Description
                : value.ToString().ToLower();
        }
    }
}
