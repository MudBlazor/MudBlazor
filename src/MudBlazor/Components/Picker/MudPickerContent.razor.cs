// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// The content within a <see cref="MudPicker{T}"/>.
/// </summary>
/// <seealso cref="MudPicker{T}" />
/// <seealso cref="MudPickerToolbar" />
#nullable enable
public partial class MudPickerContent : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("mud-picker-content")
            .AddClass(Class)
            .Build();

    /// <summary>
    /// The content to display.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Picker.Behavior)]
    public RenderFragment? ChildContent { get; set; }
}
