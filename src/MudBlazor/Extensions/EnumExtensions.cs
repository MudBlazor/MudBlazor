using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        /// Returns the best display name for the given enum
        /// </summary>
        /// <param name="enum">Type</param>
        /// <param name="shortName">Try to find the shorter version of the name</param>
        internal static string GetDisplayName(this Enum @enum, bool shortName = false)
        {
            return (shortName ? @enum.GetType().GetCustomAttribute<DisplayAttribute>()?.ShortName : null) ??
                   @enum.GetType().GetCustomAttribute<DisplayAttribute>()?.Name ??
                   @enum.GetType().GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ??
                   @enum.GetType().GetCustomAttribute<DescriptionAttribute>()?.Description ??
                   @enum.ToString();
        }
    }
}
