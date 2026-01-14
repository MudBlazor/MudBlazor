// Copyright (c) MudBlazor 2026
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable

/// <summary>
/// Displays a semantic footer area for content and navigation.
/// </summary>
public partial class MudFooter : MudComponentBase
{
    /// <summary>
    /// Gets the CSS class names for the component.
    /// </summary>
    protected string Classname =>
        new CssBuilder("mud-footer")
            .AddClass("mud-footer-fixed", Fixed)
            .AddClass("mud-footer-sticky", Sticky)
            .AddClass(Class)
            .Build();

    /// <summary>
    /// Fixes the footer to the bottom of the viewport.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.General.Behavior)]
    public bool Fixed { get; set; }

    /// <summary>
    /// Makes the footer stick to the bottom of its scroll container.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.General.Behavior)]
    public bool Sticky { get; set; }

    /// <summary>
    /// The content within this footer.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.General.Behavior)]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (Fixed && Sticky)
        {
            throw new InvalidOperationException("MudFooter does not support setting both Fixed and Sticky. Set only one to true.");
        }
    }
}
