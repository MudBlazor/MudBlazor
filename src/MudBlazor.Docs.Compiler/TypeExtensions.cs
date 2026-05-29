// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;

namespace MudBlazor.Docs.Compiler;

/// <summary>
/// Methods added to the <see cref="Type"/> class.
/// </summary>
public static partial class TypeExtensions
{
    /// <summary>
    /// Gets the shorthand name for this type.
    /// </summary>
    public static string GetFriendlyName(this Type type)
    {
        // Replace value types
        var name = type.FullName switch
        {
            "System.Boolean" => "bool",
            "System.Boolean[]" => "bool[]",
            "System.Int32" => "int",
            "System.Int32&" => "ref int",
            "System.Int32[]" => "int[]",
            "System.Int64" => "long",
            "System.Int64&" => "ref long",
            "System.Int64[]" => "long[]",
            "System.String" => "string",
            "System.String&" => "ref string",
            "System.String[]" => "string[]",
            "System.Double" => "double",
            "System.Double&" => "ref double",
            "System.Double[]" => "double[]",
            "System.Single" => "float",
            "System.Single&" => "ref float",
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
    /// The regular expression for <see cref="Nullable{T}"/>
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex("Nullable<([\\S]*)>")]
    private static partial Regex NullableRegEx();
}
