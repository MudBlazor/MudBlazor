//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

namespace MudBlazor;

#nullable enable
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

    public Size IconSize { get; set; } = Size.Medium;

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
