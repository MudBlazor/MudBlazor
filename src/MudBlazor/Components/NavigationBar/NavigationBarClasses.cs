// Copyright (c) MudBlazor 2025
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor
{
#nullable enable
    public readonly record struct NavigationBarBadgeParameters(bool Dot, object? Content, bool Overlap, Color Color)
    {
        public NavigationBarBadgeParameters() : this(false, null, true, Color.Error) { }
    }
}
