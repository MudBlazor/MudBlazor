// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;


/// <summary>
/// A thin line that groups content in lists and layouts. Only use dividers if items can't be grouped with open space. Use dividers to group things, not separate individual items.
/// </summary>
public partial class MudDivider : MudComponentBase
{
    private bool HasContent => ChildContent is not null;

// Même règle qu'avant : pas de classe de type pour un divider vertical FullWidth
    private bool ApplyDividerType => DividerType != DividerType.FullWidth || !Vertical;

    private string DividerTypeClass => $"mud-divider-{DividerType.ToStringFast(true)}";

    protected string Classname =>
        new CssBuilder("mud-divider")
            .AddClass("mud-divider-light", Light)
            .AddClass("mud-divider-vertical", Vertical)
            .AddClass("mud-divider-with-content-line", HasContent)
            .AddClass("mud-divider-absolute", Absolute && !HasContent)
            .AddClass("mud-divider-flexitem", FlexItem && !HasContent)
            .AddClass(DividerTypeClass, !HasContent && ApplyDividerType)
            .AddClass(Class, !HasContent)
            .Build();

    protected string WrapperClassname =>
        new CssBuilder("mud-divider-with-content")
            .AddClass("mud-divider-with-content-vertical", Vertical)
            .AddClass("mud-divider-absolute", Absolute)
            .AddClass("mud-divider-flexitem", FlexItem)
            .AddClass(DividerTypeClass, ApplyDividerType)
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
    
    /// <summary>
    /// The content within this component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Divider.Behavior)]
    public RenderFragment? ChildContent { get; set; }
}
