// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MudBlazor.Docs.Extensions;

/// <summary>
/// Methods which are added to existing Reflection classes.
/// </summary>
public static partial class ReflectionExtensions
{
    /// <summary>
    /// Gets the user-friendly name of this type.
    /// </summary>
    /// <param name="type">The type to use.</param>
    /// <returns>The shorthand name of this type.  Generics are supported.</returns>
    public static string GetFriendlyName(this Type type)
    {
        // Replace value types
        var name = type.FullName switch
        {
            "System.Boolean" => "bool",
            "System.Boolean[]" => "bool[]",
            "System.Int32" => "int",
            "System.Int32[]" => "int[]",
            "System.Int64" => "long",
            "System.Int64[]" => "long[]",
            "System.String" => "string",
            "System.String[]" => "string[]",
            "System.Double" => "double",
            "System.Double[]" => "double[]",
            "System.Single" => "float",
            "System.Single[]" => "float[]",
            "System.Object" => "object",
            "System.Void" => "",
            _ => type.Name
        };

        // Replace generics
        if (type.IsGenericType)
        {
            // Get the parameters
            var parameters = type.GetGenericArguments();
            // Shave off the `1
            name = string.Concat(name.AsSpan(0, name.Length - 2), "<");
            // Simplify all generic parameter
            for (var index = 0; index < parameters.Length; index++)
            {
                if (index > 0)
                {
                    name += ", ";
                }

                name += GetFriendlyName(parameters[index]);
            }
            name += ">";
        }

        // Simplify Nullable<T> to T?
        foreach (var match in NullableRegEx().Matches(name).Cast<Match>())
        {
            name = name.Replace(match.Groups[0].Value, match.Groups[1].Value + "?");
        }

        return name;
    }

    /// <summary>
    /// Gets the category order for this member.
    /// </summary>
    /// <param name="member">The member to examine.</param>
    /// <returns>The order of this member via its <see cref="CategoryAttribute"/>, or <c>0</c>.</returns>
    public static int GetCategoryOrder(this MemberInfo member)
    {
        var category = member.GetCustomAttribute<CategoryAttribute>();
        return category == null ? 9999 : category.Order;
    }

    /// <summary>
    /// Gets the category order for this member.
    /// </summary>
    /// <param name="member">The member to examine.</param>
    /// <returns>The order of this member via its <see cref="CategoryAttribute"/>, or <c>0</c>.</returns>
    public static string GetCategoryName(this MemberInfo member)
    {
        var attribute = member.GetCustomAttribute<CategoryAttribute>();
        return attribute == null ? "General" : attribute.Name;
    }

    /// <summary>
    /// The regular expression for Nullable<T>
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex("Nullable<([\\S]*)>")]
    private static partial Regex NullableRegEx();
}
