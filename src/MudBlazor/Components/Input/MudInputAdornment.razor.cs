// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.Internal;

/// <summary>
/// An icon displayed within an input component.
/// </summary>
public partial class MudInputAdornment
{
    /// <summary>
    /// The CSS classes for this adornment.
    /// </summary>
    /// <remarks>
    /// Multiple classes must be separate by spaces.
    /// </remarks>
    [Parameter]
    public string Class { get; set; }

    /// <summary>
    /// The text for this adornment.
    /// </summary>
    [Parameter]
    public string Text { get; set; }

    /// <summary>
    /// The icon for this adornment.
    /// </summary>
    [Parameter]
    public string Icon { get; set; }

    /// <summary>
    /// The amount of negative margin applied to the icon.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Edge.False"/>.  Other values are <see cref="Edge.Start"/> and <see cref="Edge.End"/>.
    /// </remarks>
    [Parameter]
    public Edge Edge { get; set; }

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
    public string AriaLabel { get; set; }

    /// <summary>
    /// Occurs when this adornment is clicked.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEventArgs> AdornmentClick { get; set; }
}
