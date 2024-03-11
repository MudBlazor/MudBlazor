//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

namespace MudBlazor
{
    public abstract class CommonSnackbarOptions
    {
        public int MaximumOpacity { get; set; } = 95;

        public int ShowTransitionDuration { get; set; } = 1000;

        public int VisibleStateDuration { get; set; } = 5000;

        public int HideTransitionDuration { get; set; } = 2000;

        public bool ShowCloseIcon { get; set; } = true;

        public bool RequireInteraction { get; set; } = false;

        public bool BackgroundBlurred { get; set; } = false;

        public Variant SnackbarVariant { get; set; } = Variant.Filled;

        protected CommonSnackbarOptions() { }

        protected CommonSnackbarOptions(CommonSnackbarOptions options)
        {
            MaximumOpacity = options.MaximumOpacity;
            ShowTransitionDuration = options.ShowTransitionDuration;
            VisibleStateDuration = options.VisibleStateDuration;
            HideTransitionDuration = options.HideTransitionDuration;
            ShowCloseIcon = options.ShowCloseIcon;
            RequireInteraction = options.RequireInteraction;
            BackgroundBlurred = options.BackgroundBlurred;
            SnackbarVariant = options.SnackbarVariant;
        }
    }
}
