// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public partial class SelectColumn<T>
    {
        [Parameter] public bool ShowInHeader { get; set; } = true;
        [Parameter] public bool ShowInFooter { get; set; } = true;
        [Parameter] public Size Size { get; set; } = Size.Medium;
    }
}
