// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace MudBlazor
{
    public class SnackbarOptions : CommonSnackbarOptions
    {
        public Func<Snackbar, Task> Onclick { get; set; }

        public SnackbarType Type { get; }

        public string SnackbarTypeClass { get; set; }

        public SnackbarOptions(SnackbarType type, SnackbarConfiguration configuration)
        {
            Type = type;
            SnackbarTypeClass = configuration.SnackbarTypeClass(type);

            MaximumOpacity = configuration.MaximumOpacity;

            ShowTransitionDuration = configuration.ShowTransitionDuration;

            VisibleStateDuration = configuration.VisibleStateDuration;

            HideTransitionDuration = configuration.HideTransitionDuration;

            RequireInteraction = configuration.RequireInteraction;
        }
    }
}