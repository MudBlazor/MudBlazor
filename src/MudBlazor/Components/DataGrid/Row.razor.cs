// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// Represents a row within a <see cref="MudDataGrid{T}"/>.
/// </summary>
public partial class Row : MudComponentBase
{
    // This class is only referenced by tests.  Can it be removed?

    [Parameter] public RenderFragment ChildContent { get; set; }

    protected string Classname => new CssBuilder("mud-table-row")
        .AddClass(Class).Build();
}
