// Copyright (c) 2019 Blazored
// License: MIT
// See https://github.com/Blazored
// Copyright (c) 2020 Adapted by MudBlazor

#nullable enable
namespace MudBlazor
{
    /// <summary>
    /// The customization options for a <see cref="MudDialog"/>.
    /// </summary>
    /// <seealso cref="MudDialogContainer"/>
    /// <seealso cref="MudDialogProvider"/>
    /// <seealso cref="MudDialog"/>
    /// <seealso cref="DialogParameters{T}"/>
    /// <seealso cref="DialogReference"/>
    /// <seealso cref="DialogService"/>
    public record DialogOptions
    {
        /// <summary>
        /// The default dialog options.
        /// This field is only intended for options that do not differ from their default values.
        /// </summary>
        internal static readonly DialogOptions Default = new();

        /// <summary>
        /// The location of the dialog.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        public DialogPosition? Position { get; init; }

        /// <summary>
        /// The maximum allowed width of the dialog.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        public MaxWidth? MaxWidth { get; init; }

        /// <summary>
        /// Allows closing the dialog by clicking outside of the dialog.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        public bool? BackdropClick { get; init; }

        /// <summary>
        /// Allows closing the dialog by pressing the Escape key.
        /// </summary>
        public bool? CloseOnEscapeKey { get; init; }

        /// <summary>
        /// Hides the dialog header.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        public bool? NoHeader { get; init; }

        /// <summary>
        /// Shows a close button in the top-right corner of the dialog.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        public bool? CloseButton { get; init; }

        /// <summary>
        /// Sets the size of the dialog to the entire screen.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        public bool? FullScreen { get; init; }

        /// <summary>
        /// Sets the width of the dialog to the width of the screen.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        public bool? FullWidth { get; init; }

        /// <summary>
        /// The custom CSS classes to apply to the dialog background.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        public string? BackgroundClass { get; init; }
    }
}
