using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MudBlazor.Extensions
{
#nullable enable
    public static class EnumExtensions
    {
        /// <summary>
        /// Universal method that retrieves an array of the values of the constant in specified enumeration, works with nullable and non-nullable enums.
        /// Original <see cref="Enum.GetValues"/> works only with non-nullable enums and will throw exception.
        /// </summary>
        /// <returns>An array that contains the values of constant in type</returns>
        internal static IEnumerable<Enum> GetSafeEnumValues(Type? type)
        {
            if (type is null)
            {
                return Enumerable.Empty<Enum>();
            }

            if (type.IsEnum)
            {
                return Enum.GetValues(type).Cast<Enum>();
            }

            if (type.IsGenericType && typeof(Nullable<>) == type.GetGenericTypeDefinition())
            {
                var actualType = type.GetGenericArguments()[0];
                return Enum.GetValues(actualType).Cast<Enum>();
            }

            return Enumerable.Empty<Enum>();
        }

        /// <summary>
        /// Returns the name of the enumeration if attributes are set DisplayName in enum
        /// </summary>
        /// <param name="currentEnum">Current enum</param>
        /// <returns>Get value Name of Display attribute or default value if non exist</returns>
        public static string GetEnumDisplayName(this Enum currentEnum)
        {
            return currentEnum.GetType().GetMember(currentEnum.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                ?.Name ?? currentEnum.ToString();
        }
    }
}
