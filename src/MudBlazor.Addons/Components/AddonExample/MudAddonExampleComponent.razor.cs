// Copyright (c) MudBlazor 2024
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor.Addons;

#nullable enable

/// <summary>
/// An example component for demonstrating the MudBlazor.Addons infrastructure.
/// </summary>
public partial class MudAddonExampleComponent : MudComponentBase
{
    /// <summary>
    /// The title to display.
    /// </summary>
    [Parameter]
    public string? Title { get; set; } = "Addon Example";

    /// <summary>
    /// The content to display.
    /// </summary>
    [Parameter]
    public string? Content { get; set; } = "This is an example addon component.";

    /// <summary>
    /// Child content to render inside the component.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected string Classname =>
        string.IsNullOrEmpty(Class)
            ? "mud-addon-example"
            : $"mud-addon-example {Class}";
}
