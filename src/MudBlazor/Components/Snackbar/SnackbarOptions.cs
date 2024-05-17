//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

using System;
using System.Threading.Tasks;

namespace MudBlazor
{
    public class SnackbarOptions : CommonSnackbarOptions
    {
        /// <summary>
        /// The asynchronous delegate that is invoked when the Snackbar is clicked.
        /// </summary>
        public Func<Snackbar, Task> OnClick { get; set; }

        /// <summary>
        /// The asynchronous delegate that is invoked when the close button of the Snackbar is clicked.
        /// </summary>
        public Func<Snackbar, Task> OnCloseButtonClick { get; set; }

        /// <summary>
        /// The Label for the action button displayed on the <see cref="Snackbar"/>.
        /// </summary>
        /// <remarks>
        /// Action button invokes <see cref="OnClick"/> task.
        /// </remarks>
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

        public SnackbarDuplicatesBehavior DuplicatesBehavior { get; set; } = SnackbarDuplicatesBehavior.GlobalDefault;

        /// <summary>
        /// Custom normal icon.
        /// </summary>
        public string NormalIcon { get; set; } = Icons.Material.Outlined.EventNote;

        /// <summary>
        /// Custom info icon.
        /// </summary>
        public string InfoIcon { get; set; } = Icons.Material.Outlined.Info;

        /// <summary>
        /// Custom success icon.
        /// </summary>
        public string SuccessIcon { get; set; } = Icons.Custom.Uncategorized.AlertSuccess;

        /// <summary>
        /// Custom warning icon.
        /// </summary>
        public string WarningIcon { get; set; } = Icons.Material.Outlined.ReportProblem;

        /// <summary>
        /// Custom error icon.
        /// </summary>
        public string ErrorIcon { get; set; } = Icons.Material.Filled.ErrorOutline;

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
                    _ => throw new ArgumentOutOfRangeException(nameof(Severity)),
                };
            }
        }
    }
}
