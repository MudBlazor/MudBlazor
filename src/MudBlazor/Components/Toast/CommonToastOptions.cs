// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace MudBlazor
{
    public abstract class CommonToastOptions
    {
        /// <summary>
        /// The main toast class. Defaults to <see cref="Defaults.Classes.Toast"/>
        /// </summary>
        public string ToastClass { get; set; } = Defaults.Classes.Toast;

        /// <summary>
        /// This variable applies to the title and message. If false the rendered HTML will be shown, else the escaped HTML markup will be shown. 
        /// </summary>
        public bool EscapeHtml { get; set; } = true;

        /// <summary>
        /// The css class for the title. Defaults to <see cref="Defaults.Classes.ToastTitle"/>.
        /// </summary>
        public string ToastTitleClass { get; set; } = Defaults.Classes.ToastTitle;

        /// <summary>
        /// The css class for the message. Defaults to <see cref="Defaults.Classes.ToastMessage"/>.
        /// </summary>
        public string ToastMessageClass { get; set; } = Defaults.Classes.ToastMessage;

        /// <summary>
        /// The maximum opacity expressed as an integer percentage for a toast in the Visible state. Defaults to 80% where 0 means completely hidden and 100 means solid color.
        /// </summary>
        public int MaximumOpacity { get; set; } = 80;

        /// <summary>
        /// How long the showing transition takes to bring a toast to the MaximumOpacity and set it to the Visible state. Defaults to 1000 ms.
        /// </summary>
        public int ShowTransitionDuration { get; set; } = 1000;

        /// <summary>
        /// How long the toast remain visible without user interaction. A value less than 1 triggers the hiding immediately. Defaults to 5000 ms.
        /// </summary>
        public int VisibleStateDuration { get; set; } = 5000;

        /// <summary>
        /// How long the hiding transition takes to make a toast disappear. Defaults to 2000 ms.
        /// </summary>
        public int HideTransitionDuration { get; set; } = 2000;

        /// <summary>
        /// States if a progressbar has to be shown during the toast Visible state. Defaults to true.
        /// </summary>
        public bool ShowProgressBar { get; set; } = true;

        /// <summary>
        /// The css class for the progress bar. Defaults to <see cref="Defaults.Classes.ProgressBarClass"/>.
        /// </summary>
        public string ProgressBarClass { get; set; } = Defaults.Classes.ProgressBarClass;

        /// <summary>
        /// States if the close icon has to be used for hiding a toast. The icon presence disables the default "hide on click" behaviour. Defaults to true.
        /// </summary>
        public bool ShowCloseIcon { get; set; } = true;

        /// <summary>
        /// The css class for the close icon. Defaults to <see cref="Defaults.Classes.CloseIconClass"/>.
        /// </summary>
        public string CloseIconClass { get; set; } = Defaults.Classes.CloseIconClass;

        /// <summary>
        /// When true it disables the auto hiding and forces the user to interact with the toast for closing it. Defaults to false.
        /// </summary>
        public bool RequireInteraction { get; set; } = false;
    }
}
