﻿//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

using System;
using System.Threading.Tasks;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public class SnackbarOptions : CommonSnackbarOptions
    {
        public Func<Snackbar, Task> Onclick { get; set; }

        public string Action { get; set; }

        public Variant? ActionVariant { get; set; }

        public Color ActionColor { get; set; } = Color.Default;

        public Severity Severity { get; }

        public string SnackbarTypeClass { get; set; }

        public bool CloseAfterNavigation { get; set; }

        public SnackbarOptions(Severity severity, CommonSnackbarOptions options) : base(options)
        {
            Severity = severity;

            SnackbarTypeClass = $"mud-alert-{SnackbarVariant.ToDescriptionString()}-{severity.ToDescriptionString()}";

            if (SnackbarVariant != Variant.Filled)
            {
                SnackbarTypeClass += BackgroundBlurred ? " mud-snackbar-blurred" : " mud-snackbar-surface";
            }
        }
    }
}
