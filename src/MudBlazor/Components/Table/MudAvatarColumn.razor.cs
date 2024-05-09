// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor;

#nullable enable
public partial class MudAvatarColumn<T> : MudBaseColumn
{
    [Parameter]
    public T? Value { get; set; }
}
