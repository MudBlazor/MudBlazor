//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

using System;
using System.Threading.Tasks;

namespace MudBlazor
{
    public class SnackbarOptions : CommonSnackbarOptions
    {
        public Func<Snackbar, Task> Onclick { get; set; }

        public Severity Severity { get; }

        public string SnackbarTypeClass { get; set; }

        public SnackbarOptions(Severity severity, SnackbarConfiguration configuration)
        {
            Severity = severity;
            SnackbarTypeClass = configuration.SnackbarTypeClass(severity, configuration.SnackbarVariant, configuration.BackgroundBlurred);

            MaximumOpacity = configuration.MaximumOpacity;

            ShowTransitionDuration = configuration.ShowTransitionDuration;

            VisibleStateDuration = configuration.VisibleStateDuration;

            HideTransitionDuration = configuration.HideTransitionDuration;

            RequireInteraction = configuration.RequireInteraction;
        }
    }
}