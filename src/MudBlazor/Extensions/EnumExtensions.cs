using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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

        /// <summary>
        /// Universal method that retrieves an array of the values of the constant in specified enumeration, works with nullable and non-nullable enums.
        /// Original <see cref="Enum.GetValues"/> works only with non-nullable enums and will throw exception.
        /// </summary>
        /// <returns>An array that contains the values of constant in type</returns>
        internal static IEnumerable<Enum> GetSafeEnumValues(Type type)
        {
            if (type.IsEnum)
                return Enum.GetValues(type).Cast<Enum>();

            if (type.IsGenericType && typeof(Nullable<>) == type.GetGenericTypeDefinition())
            {
                var actualType = type.GetGenericArguments()[0];
                return Enum.GetValues(actualType).Cast<Enum>();
            }

            return Enumerable.Empty<Enum>();
        }
    }
}
