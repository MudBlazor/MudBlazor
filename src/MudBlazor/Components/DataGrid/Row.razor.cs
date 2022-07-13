// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class Row : MudComponentBase
    {
        [Parameter] public RenderFragment ChildContent { get; set; }

        protected string Classname => new CssBuilder("mud-table-row")
            .AddClass(Class).Build();
    }
}
