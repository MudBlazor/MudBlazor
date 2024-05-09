// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor;

#nullable enable
public partial class MudTemplateColumn<T> : MudBaseColumn
{
    [Parameter]
    public T? DataContext { get; set; }

    [Parameter]
    public RenderFragment<T>? Header { get; set; }

    [Parameter]
    public RenderFragment<T>? Row { get; set; }

    [Parameter]
    public RenderFragment<T>? Edit { get; set; }

    [Parameter]
    public RenderFragment<T>? Footer { get; set; }
}
