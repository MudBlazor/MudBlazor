// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor.Internal;

#nullable enable
/// <summary>
/// An icon displayed within an input component.
/// </summary>
public partial class MudInputAdornment
{
    protected string Classname =>
        new CssBuilder("mud-input-adornment")
            .AddClass(Class)
            .Build();

    /// <summary>
    /// The CSS classes for this adornment.
    /// </summary>
    /// <remarks>
    /// Multiple classes must be separate by spaces.
    /// </remarks>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// The text for this adornment.
    /// </summary>
    [Parameter]
    public string? Text { get; set; }

    /// <summary>
    /// The icon for this adornment.
    /// </summary>
    [Parameter]
    public string? Icon { get; set; }

    /// <summary>
    /// Specifies the position of the adornment within the field.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Adornment.None"/>.
    /// </remarks>
    [Parameter]
    public Adornment Placement { get; set; }

    /// <summary>
    /// The size of the icon.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Size.Medium"/>.
    /// </remarks>
    [Parameter]
    public Size Size { get; set; } = Size.Medium;

    /// <summary>
    /// The color of the icon button.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Color.Default"/>.
    /// </remarks>
    [Parameter]
    public Color Color { get; set; } = Color.Default;

    /// <summary>
    /// The accessible name for this adornment.
    /// </summary>
    /// <remarks>
    /// More information on accessible names can be found <see href="https://developer.mozilla.org/docs/Glossary/Accessible_name">here</see>.
    /// </remarks>
    [Parameter]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Occurs when this adornment is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> AdornmentClick { get; set; }
}
