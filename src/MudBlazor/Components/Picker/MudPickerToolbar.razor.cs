// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// The toolbar content of a <see cref="MudPicker{T}"/>.
/// </summary>
/// <seealso cref="MudPicker{T}" />
/// <seealso cref="MudPickerContent" />
#nullable enable
public partial class MudPickerToolbar : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("mud-picker-toolbar")
            .AddClass($"mud-theme-{Color.ToDescriptionString()}")
            .AddClass("mud-picker-toolbar-landscape",
                Orientation == Orientation.Landscape && PickerVariant == PickerVariant.Static)
            .AddClass(Class)
            .Build();

    /// <summary>
    /// Shows the toolbar.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Picker.Behavior)]
    public bool ShowToolbar { get; set; } = true;

    /// <summary>
    /// The display orientation of this toolbar.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Picker.Appearance)]
    public Orientation Orientation { get; set; }

    /// <summary>
    /// The display variant for this toolbar.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Picker.Appearance)]
    public PickerVariant PickerVariant { get; set; }

    /// <summary>
    /// The color of the toolbar, selected, and active values.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Picker.Appearance)]
    public Color Color { get; set; }

    /// <summary>
    /// The content within this toolbar.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Picker.Behavior)]
    public RenderFragment? ChildContent { get; set; }
}
