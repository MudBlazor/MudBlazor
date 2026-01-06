// Copyright (c) MudBlazor 2024
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor.Addons;

#nullable enable

/// <summary>
/// A dummy component for testing the MudBlazor.Addons infrastructure.
/// </summary>
public partial class MudDummyComponent : MudComponentBase
{
    /// <summary>
    /// The title to display.
    /// </summary>
    [Parameter]
    public string? Title { get; set; } = "Dummy Component";

    /// <summary>
    /// The content to display.
    /// </summary>
    [Parameter]
    public string? Content { get; set; } = "This is a dummy component for testing.";

    /// <summary>
    /// Child content to render inside the component.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    protected string Classname =>
        string.IsNullOrEmpty(Class)
            ? "mud-dummy-component"
            : $"mud-dummy-component {Class}";
}
