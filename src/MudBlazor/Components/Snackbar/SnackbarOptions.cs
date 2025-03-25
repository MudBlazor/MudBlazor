//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// The options applied to an individual snackbar.
    /// </summary>
    public class SnackbarOptions : CommonSnackbarOptions
    {
        /// <summary>
        /// Occurs when the snackbar is clicked.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        public Func<Snackbar, Task>? OnClick { get; set; }

        /// <summary>
        /// Occurs when the <c>Close</c> button is clicked.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        public Func<Snackbar, Task>? CloseButtonClickFunc { get; set; }

        /// <summary>
        /// The text for a custom button in the snackbar message.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        public string? Action { get; set; }

        /// <summary>
        /// The display variant of the action button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        public Variant? ActionVariant { get; set; }

        /// <summary>
        /// The color of the action button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        public Color ActionColor { get; set; } = Color.Default;

        /// <summary>
        /// The severity of the snackbar.
        /// </summary>
        public Severity Severity { get; }

        /// <summary>
        /// The custom CSS classes for the snackbar.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Multiple classes must be separated by spaces.
        /// </remarks>
        public string? SnackbarTypeClass { get; set; }

        /// <summary>
        /// Closes the snackbar after navigating away from the current page.
        /// </summary>
        public bool CloseAfterNavigation { get; set; }

        /// <summary>
        /// Hides the icon for the snackbar.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        public bool HideIcon { get; set; }

        /// <summary>
        /// The custom icon to display for the snackbar.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Will be set to match the <see cref="Severity"/>.
        /// </remarks>
        public string Icon { get; set; }

        /// <summary>
        /// The color of the icon to display.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Inherit" />.
        /// </remarks>
        public Color IconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// The action applied when duplicate snackbars are detected.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="SnackbarDuplicatesBehavior.GlobalDefault"/> which is set via <see cref="SnackbarConfiguration.PreventDuplicates"/>.
        /// </remarks>
        public SnackbarDuplicatesBehavior DuplicatesBehavior { get; set; } = SnackbarDuplicatesBehavior.GlobalDefault;

        /// <summary>
        /// Creates new options for a snackbar.
        /// </summary>
        /// <param name="severity">The severity of the snackbar to display.</param>
        /// <param name="options">Any other options to apply.</param>
        public SnackbarOptions(Severity severity, CommonSnackbarOptions options) : base(options)
        {
            Severity = severity;

            if (string.IsNullOrEmpty(Icon))
            {
                Icon = Severity switch
                {
                    Severity.Normal => NormalIcon,
                    Severity.Info => InfoIcon,
                    Severity.Success => SuccessIcon,
                    Severity.Warning => WarningIcon,
                    Severity.Error => ErrorIcon,
                    _ => throw new ArgumentOutOfRangeException(nameof(severity)),
                };
            }
        }
    }
}
