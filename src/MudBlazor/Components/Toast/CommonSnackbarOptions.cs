// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace MudBlazor
{
    public abstract class CommonSnackbarOptions
    {
        public string SnackbarClass { get; set; } = Defaults.Classes.Snackbar;

        public bool EscapeHtml { get; set; } = true;

        public string SnackbarTitleClass { get; set; } = Defaults.Classes.SnackbarTitle;

        public string SnackbarMessageClass { get; set; } = Defaults.Classes.SnackbarMessage;

        public int MaximumOpacity { get; set; } = 80;

        public int ShowTransitionDuration { get; set; } = 1000;

        public int VisibleStateDuration { get; set; } = 5000;

        public int HideTransitionDuration { get; set; } = 2000;

        public bool ShowProgressBar { get; set; } = true;

        public string ProgressBarClass { get; set; } = Defaults.Classes.ProgressBarClass;

        public bool ShowCloseIcon { get; set; } = true;

        public string CloseIconClass { get; set; } = Defaults.Classes.CloseIconClass;

        public bool RequireInteraction { get; set; } = false;
    }
}
