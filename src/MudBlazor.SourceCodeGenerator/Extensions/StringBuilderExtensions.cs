// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.SourceCodeGenerator.Extensions;

internal static class StringBuilderExtensions
{
    public static StringBuilder AppendCode(this StringBuilder builder, string value, ushort indent = 0)
    {
        for (var i = 0; i < indent; i++)
        {
            builder.Append("    ");
        }

        return builder.AppendLine(value);
    }
}
