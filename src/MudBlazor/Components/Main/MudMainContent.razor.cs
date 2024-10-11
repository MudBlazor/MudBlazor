// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents the main content area of the <see cref="MudLayout"/>.
/// </summary>
public partial class MudMainContent : MudComponentBase
{
    /// <summary>
    /// Gets the CSS class names for the component.
    /// </summary>
    protected string Classname =>
        new CssBuilder("mud-main-content")
            .AddClass(Class)
            .Build();

    /// <summary>
    /// Sets the content to be rendered inside the main content area.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.MainContent.Behavior)]
    public RenderFragment? ChildContent { get; set; }
}
