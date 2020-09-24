// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace MudBlazor
{
    public abstract class CommonSnackbarOptions
    {
        public int MaximumOpacity { get; set; } = 100;
        public int ShowTransitionDuration { get; set; } = 1000;

        public int VisibleStateDuration { get; set; } = 5000;

        public int HideTransitionDuration { get; set; } = 2000;

        public bool ShowCloseIcon { get; set; } = true;

        public bool RequireInteraction { get; set; } = false;
    }
}
