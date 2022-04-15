//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
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

        public bool HideIcon { get; set; }

        public string Icon { get; set; }
        public Color IconColor { get; set; } = Color.Inherit;
        public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// Custom normal icon.
        /// </summary>
        [Parameter] public string NormalIcon { get; set; } = Icons.Material.Outlined.EventNote;

        /// <summary>
        /// Custom info icon.
        /// </summary>
        [Parameter] public string InfoIcon { get; set; } = Icons.Material.Outlined.Info;

        /// <summary>
        /// Custom success icon.
        /// </summary>
        [Parameter] public string SuccessIcon { get; set; } = Icons.Custom.Uncategorized.AlertSuccess;

        /// <summary>
        /// Custom warning icon.
        /// </summary>
        [Parameter] public string WarningIcon { get; set; } = Icons.Material.Outlined.ReportProblem;

        /// <summary>
        /// Custom error icon.
        /// </summary>
        [Parameter] public string ErrorIcon { get; set; } = Icons.Material.Filled.ErrorOutline;

        public SnackbarOptions(Severity severity, CommonSnackbarOptions options) : base(options)
        {
            Severity = severity;

            SnackbarTypeClass = $"mud-alert-{SnackbarVariant.ToDescriptionString()}-{severity.ToDescriptionString()}";

            if (SnackbarVariant != Variant.Filled)
            {
                SnackbarTypeClass += BackgroundBlurred ? " mud-snackbar-blurred" : " mud-snackbar-surface";
            }

            if (string.IsNullOrEmpty(Icon))
            {
                Icon = Severity switch
                {
                    Severity.Normal => NormalIcon,
                    Severity.Info => InfoIcon,
                    Severity.Success => SuccessIcon,
                    Severity.Warning => WarningIcon,
                    Severity.Error => ErrorIcon,
                    _ => throw new ArgumentOutOfRangeException(nameof(Severity)),
                };
            }
        }
    }
}
