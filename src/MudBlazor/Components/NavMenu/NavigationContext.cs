// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;

namespace MudBlazor;

#nullable enable
public record NavigationContext(bool Disabled, bool Expanded)
{
    public string MenuId { get; } = $"mudnavmenu-{Guid.NewGuid()}";
}
