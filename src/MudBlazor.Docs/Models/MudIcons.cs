// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Docs.Models;

#nullable enable
public class MudIcons
{
    public string Name { get; }

    public string Code { get; }

    public string Category { get; }

    public MudIcons(string name, string code, string category)
    {
        Name = name;
        Code = code;
        Category = category;
    }

    public static readonly MudIcons Empty = new(string.Empty, string.Empty, string.Empty);
}
