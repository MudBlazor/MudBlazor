// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor.Components.Snackbar.InternalComponents;

#nullable enable
public partial class SnackbarMessageRenderFragment : ComponentBase
{
    /// <summary>
    /// Sets the message to be rendered as a custom fragment of UI.
    /// </summary>
    /// <remarks>
    /// This property allows you to define a custom UI using <see cref="RenderFragment"/>. 
    /// It can be used to pass in components, markup, or other content.
    /// </remarks>
    [Parameter]
    public RenderFragment? Message { get; set; }
}
