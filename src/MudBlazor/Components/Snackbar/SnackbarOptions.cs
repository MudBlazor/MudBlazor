//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

namespace MudBlazor
{
#nullable enable
    public class SnackbarOptions : CommonSnackbarOptions
    {
        public Func<Snackbar, Task>? Onclick { get; set; }

        public string? Action { get; set; }

        public Variant? ActionVariant { get; set; }

        public Color ActionColor { get; set; } = Color.Default;

        public Severity Severity { get; }

        public string? SnackbarTypeClass { get; set; }

        public bool CloseAfterNavigation { get; set; }

        public bool HideIcon { get; set; }

        public string Icon { get; set; }

        public Color IconColor { get; set; } = Color.Inherit;

        public SnackbarDuplicatesBehavior DuplicatesBehavior { get; set; } = SnackbarDuplicatesBehavior.GlobalDefault;

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
