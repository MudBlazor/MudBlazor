//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

namespace MudBlazor;

#nullable enable

/// <summary>
/// The options which control Snackbar pop-ups.
/// </summary>
public abstract class CommonSnackbarOptions
{
    /// <summary>
    /// The maximum opacity for the snackbar.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>95</c>.  The maximum value is <c>100</c>.
    /// </remarks>
    public int MaximumOpacity { get; set; } = 95;

    /// <summary>
    /// The time, in milliseconds, to animate showing the snackbar.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>1000</c> (one second).
    /// </remarks>
    public int ShowTransitionDuration { get; set; } = 1000;

    /// <summary>
    /// The time, in milliseconds, to show the snackbar.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>5000</c> (five seconds).
    /// </remarks>
    public int VisibleStateDuration { get; set; } = 5000;

    /// <summary>
    /// The time, in milliseconds, to hide the snackbar.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>2000</c> (two seconds).
    /// </remarks>
    public int HideTransitionDuration { get; set; } = 2000;

    /// <summary>
    /// Displays a close icon for the snackbar.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    public bool ShowCloseIcon { get; set; } = true;

    /// <summary>
    /// Shows the snackbar until a user manually closes it.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    public bool RequireInteraction { get; set; } = false;

    /// <summary>
    /// Blurs the background of the snackbar.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    public bool BackgroundBlurred { get; set; } = false;

    /// <summary>
    /// The default display variant for the snackbar.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Variant.Filled"/>.
    /// </remarks>
    public Variant SnackbarVariant { get; set; } = Variant.Filled;

    /// <summary>
    /// The default icon size for the snackbar.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Size.Medium"/>.
    /// </remarks>
    public Size IconSize { get; set; } = Size.Medium;

    /// <summary>
    /// The icon displayed for <c>Normal</c> severity snackbars.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Outlined.EventNote"/>.
    /// </remarks>
    public string NormalIcon { get; set; } = Icons.Material.Outlined.EventNote;

    /// <summary>
    /// The icon displayed for <c>Info</c> severity snackbars.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Outlined.Info"/>.
    /// </remarks>
    public string InfoIcon { get; set; } = Icons.Material.Outlined.Info;

    /// <summary>
    /// The icon displayed for <c>Success</c> severity snackbars.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Custom.Uncategorized.AlertSuccess"/>.
    /// </remarks>
    public string SuccessIcon { get; set; } = Icons.Custom.Uncategorized.AlertSuccess;

    /// <summary>
    /// The icon displayed for <c>Warning</c> severity snackbars.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Outlined.ReportProblem"/>.
    /// </remarks>
    public string WarningIcon { get; set; } = Icons.Material.Outlined.ReportProblem;

    /// <summary>
    /// The icon displayed for <c>Error</c> severity snackbars.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Icons.Material.Filled.ErrorOutline"/>.
    /// </remarks>
    public string ErrorIcon { get; set; } = Icons.Material.Filled.ErrorOutline;

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
        IconSize = options.IconSize;
        NormalIcon = options.NormalIcon;
        InfoIcon = options.InfoIcon;
        SuccessIcon = options.SuccessIcon;
        WarningIcon = options.WarningIcon;
        ErrorIcon = options.ErrorIcon;
    }
}
