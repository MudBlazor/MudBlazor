// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable

/// <summary>
/// A simple component that displays an image.
/// </summary>
/// <remarks>
/// This component is equivalent to the <c>img</c> HTML tag.
/// </remarks>
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
    /// Scales this image to the parent container.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Image.Behavior)]
    public bool Fluid { get; set; }

    /// <summary>
    /// The path to the image.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Image.Behavior)]
    public string? Src { get; set; }

    /// <summary>
    /// The alternate text for this image.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Image.Behavior)]
    public string? Alt { get; set; }

    /// <summary>
    /// The height of this image, in pixels.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Image.Appearance)]
    public int? Height { get; set; }

    /// <summary>
    /// The width of this image, in pixels.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Image.Appearance)]
    public int? Width { get; set; }

    /// <summary>
    /// The size of the drop shadow for this image.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>0</c>.  
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Image.Appearance)]
    public int Elevation { set; get; }

    /// <summary>
    /// Controls how this image is resized.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="ObjectFit.Fill"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Image.Appearance)]
    public ObjectFit ObjectFit { set; get; } = ObjectFit.Fill;

    /// <summary>
    /// Controls how this image is positioned within its container.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="ObjectPosition.Center"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Image.Appearance)]
    public ObjectPosition ObjectPosition { set; get; } = ObjectPosition.Center;
}
