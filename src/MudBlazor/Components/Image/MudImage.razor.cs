// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable
public partial class MudImage : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("mud-image")
            .AddClass("fluid", Fluid)
            .AddClass($"object-{ObjectFit.ToDescriptionString()}")
            .AddClass($"object-{ObjectPosition.ToDescriptionString()}")
            .AddClass($"mud-elevation-{Elevation}", Elevation > 0)
            .AddClass(Class)
            .Build();

    /// <summary>
    /// Applies the fluid class so the image scales with the parent width.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Image.Behavior)]
    public bool Fluid { get; set; }

    /// <summary>
    /// Specifies the path to the image.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Image.Behavior)]
    public string? Src { get; set; }

    /// <summary>
    /// Specifies an alternate text for the image.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Image.Behavior)]
    public string? Alt { get; set; }

    /// <summary>
    /// Specifies the height of the image in px.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Image.Appearance)]
    public int? Height { get; set; }

    /// <summary>
    /// Specifies the width of the image in px.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Image.Appearance)]
    public int? Width { get; set; }

    /// <summary>
    /// The higher the number, the heavier the drop-shadow.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Image.Appearance)]
    public int Elevation { set; get; }

    /// <summary>
    /// Controls how the image should be resized.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Image.Appearance)]
    public ObjectFit ObjectFit { set; get; } = ObjectFit.Fill;

    /// <summary>
    /// Controls how the image should positioned within its container.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Image.Appearance)]
    public ObjectPosition ObjectPosition { set; get; } = ObjectPosition.Center;
}
