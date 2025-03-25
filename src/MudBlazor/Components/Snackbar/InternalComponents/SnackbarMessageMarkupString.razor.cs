// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor.Components.Snackbar.InternalComponents;

#nullable enable
public partial class SnackbarMessageMarkupString : ComponentBase
{
    /// <summary>
    /// Sets the message to be displayed as HTML content.
    /// </summary>
    /// <remarks>
    /// This property allows you to pass an HTML-formatted message using <see cref="MarkupString"/>.
    /// </remarks>
    [Parameter]
    public MarkupString Message { get; set; }
}
