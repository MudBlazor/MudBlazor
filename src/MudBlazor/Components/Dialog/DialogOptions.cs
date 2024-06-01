﻿// Copyright (c) 2020 Jonny Larsson
// License: MIT
// See https://github.com/MudBlazor/MudBlazor
// Modified version of Blazored Modal
// Copyright (c) 2019 Blazored
// License: MIT
// See https://github.com/Blazored

namespace MudBlazor;

#nullable enable

/// <summary>
/// Represents the customization options for a <see cref="MudDialog"/>.
/// </summary>
/// <seealso cref="MudDialogInstance"/>
/// <seealso cref="MudDialogProvider"/>
/// <seealso cref="MudDialog"/>
/// <seealso cref="DialogParameters{T}"/>
/// <seealso cref="DialogReference"/>
public class DialogOptions
{
    /// <summary>
    /// The location of the dialog.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    public DialogPosition? Position { get; set; }

    /// <summary>
    /// The maximum allowed with of the dialog.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    public MaxWidth? MaxWidth { get; set; }

    /// <summary>
    /// Allows closing the dialog by clicking outside of the dialog.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    public bool? BackdropClick { get; set; }

    /// <summary>
    /// Allows closing the dialog by pressing the <c>Escape</c> key.
    /// </summary>
    public bool? CloseOnEscapeKey { get; set; }

    /// <summary>
    /// Hides the dialog header.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    public bool? NoHeader { get; set; }

    /// <summary>
    /// Shows a close button in the top-right corner of the dialog.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    public bool? CloseButton { get; set; }

    /// <summary>
    /// Sets the size of the dialog to the entire screen.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    public bool? FullScreen { get; set; }

    /// <summary>
    /// Sets the width of the dialog to the width of the screen.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    public bool? FullWidth { get; set; }

    /// <summary>
    /// The custom CSS classes to apply to the dialog background.
    /// </summary>
    /// <remarks>
    /// Multiple classes must be separated by spaces.
    /// </remarks>
    public string? BackgroundClass { get; set; }
}
