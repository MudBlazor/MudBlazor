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
        /// Get string from [Display(Name = "Value")] atrribute from enum
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns>String value from [Display]</returns>
        public static string GetEnumDisplayName(Enum enumType)
        {
            return enumType.GetType().GetMember(enumType.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                ?.Name ?? enumType.ToString();
        }
        
    }
}
