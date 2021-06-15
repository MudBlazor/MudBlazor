//Copyright(c) Alessandro Ghidini.All rights reserved.
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

        public bool HideIcon { get; set; }

        public string Icon { get; set; }

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
                    Severity.Normal => Icons.Material.Outlined.EventNote,
                    Severity.Info => Icons.Material.Outlined.Info,
                    Severity.Success => Icons.Custom.Uncategorized.AlertSuccess,
                    Severity.Warning => Icons.Material.Outlined.ReportProblem,
                    Severity.Error => Icons.Material.Filled.ErrorOutline,
                    _ => throw new ArgumentOutOfRangeException(nameof(Severity)),
                };
            }
        }
    }
}
