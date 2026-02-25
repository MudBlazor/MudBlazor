// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// A container that stacks multiple <see cref="MudFab"/> buttons vertically in a fixed position.
/// </summary>
/// <seealso cref="MudFab"/>
public partial class MudFabGroup : MudComponentBase
{
    protected string Classname => new CssBuilder("mud-fab-group")
        .AddClass(Class)
        .AddClass($"gap-{Spacing}", Spacing >= 0)
        .Build();

    protected string Stylename => new StyleBuilder()
        .AddStyle("bottom", Bottom)
        .AddStyle("right", Right)
        .AddStyle(Style)
        .Build();

    /// <summary>
    /// The distance from the bottom of the viewport.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>16px</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Button.Appearance)]
    public string Bottom { get; set; } = "16px";

    /// <summary>
    /// The distance from the right edge of the viewport.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>16px</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Button.Appearance)]
    public string Right { get; set; } = "16px";

    /// <summary>
    /// The space between buttons.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>3</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Button.Appearance)]
    public int Spacing { get; set; } = 3;

    /// <summary>
    /// The <see cref="MudFab"/> components within this group.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public RenderFragment? ChildContent { get; set; }
}
