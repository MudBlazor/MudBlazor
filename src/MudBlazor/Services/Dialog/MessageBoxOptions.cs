// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace MudBlazor;

/// <summary>
/// Options for displaying a simple message box dialog via <see cref="IDialogService"/>.
/// </summary>
/// <remarks>
/// Use this when you want a quick yes/no/cancel prompt without building a custom dialog component.
/// </remarks>
[ExcludeFromCodeCoverage]
public class MessageBoxOptions
{
    /// <summary>
    /// The text at the top of the message box.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The main content of the message box.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// The main HTML content of the message box.
    /// </summary>
    public MarkupString MarkupMessage { get; set; }

    /// <summary>
    /// The default label of the Yes button.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>OK</c>.  When <c>null</c>, this button will be hidden.
    /// </remarks>
    public string YesText { get; set; } = "OK";

    /// <summary>
    /// The default label of the No button.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  When <c>null</c>, this button will be hidden.
    /// </remarks>
    public string? NoText { get; set; }

    /// <summary>
    /// The default label of the cancel button.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  When <c>null</c>, this button will be hidden.
    /// </remarks>
    public string? CancelText { get; set; }

    /// <summary>
    /// Reverses the order of the action buttons.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// When <c>false</c>, the standard button order is used for this message box.
    /// When <c>null</c>, the value is inherited from <see cref="MudDialogProvider.ReverseMessageBoxButtonOrder" />.
    /// </remarks>
    public bool? ReverseButtonOrder { get; set; }
}
