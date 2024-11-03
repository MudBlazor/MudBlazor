// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable

/// <summary>
/// A thin line that groups content in lists and layouts.
/// </summary>
public partial class MudDivider : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("mud-divider")
            .AddClass("mud-divider-absolute", Absolute)
            .AddClass("mud-divider-flexitem", FlexItem)
            .AddClass("mud-divider-light", Light)
            .AddClass("mud-divider-vertical", Vertical)
            .AddClass($"mud-divider-{DividerType.ToDescriptionString()}", DividerType != DividerType.FullWidth || (DividerType == DividerType.FullWidth && Vertical == false))
            .AddClass(Class)
            .Build();

    /// <summary>
    /// Uses an absolute position for this divider.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Divider.Appearance)]
    public bool Absolute { get; set; }

    /// <summary>
    /// For vertical dividers, uses the correct height within a flex container.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Divider.Appearance)]
    public bool FlexItem { get; set; }

    /// <summary>
    /// Uses a lighter color.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Divider.Appearance)]
    public bool Light { get; set; }

    /// <summary>
    /// Displays the divider vertically.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Divider.Appearance)]
    public bool Vertical { get; set; }

    /// <summary>
    /// The type of divider to display.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="DividerType.FullWidth"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Divider.Appearance)]
    public DividerType DividerType { get; set; } = DividerType.FullWidth;
}
