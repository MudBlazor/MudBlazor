using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MudBlazor.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDescriptionString(this Enum val)
        {
            var attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0
                ? attributes[0].Description
                : val.ToString().ToLower();
        }

        internal static IEnumerable<Enum> GetSafeEnumValues(Type type)
        {
            if (type.IsEnum)
                return Enum.GetValues(type).Cast<Enum>();

            if (typeof(Nullable<>) == type.GetGenericTypeDefinition())
            {
                var actualType = type.GetGenericArguments()[0];
                return Enum.GetValues(actualType).Cast<Enum>();
            }

            return Enumerable.Empty<Enum>();
        }
    }
}
