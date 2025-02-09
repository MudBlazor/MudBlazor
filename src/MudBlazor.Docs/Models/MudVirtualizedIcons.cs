// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Docs.Models;

#nullable enable
public class MudVirtualizedIcons
{
    public MudIcons[] RowIcons { get; }

    public MudVirtualizedIcons(MudIcons[] rowIcons)
    {
        RowIcons = rowIcons;
    }
}
