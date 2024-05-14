// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor;

public static class MudGlobal
{
    public static class DialogDefaults
    {
        /// <summary>
        /// The default <see cref="MudDialog.DefaultFocus"/>.
        /// </summary>
        public static DefaultFocus DefaultFocus { get; set; } = DefaultFocus.Element;
    }

    public static class InputDefaults
    {
        /// <summary>
        /// Shows the label inside the input if no <see cref="MudBaseInput{T}.Value"/> is specified.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, the label will not move into the input when the input is empty.
        /// </remarks>
        public static bool ShrinkLabel { get; set; }
    }

    public static class OverlayDefaults
    {
        /// <summary>
        /// The default transition delay for <see cref="MudOverlay"/> and <see cref="MudPicker{T}"/>.
        /// </summary>
        public static TimeSpan Delay { get; set; } = TransitionDefaults.Delay;

        /// <summary>
        /// The default transition time for components like <see cref="MudTooltip"/>, <see cref="MudOverlay"/>, <see cref="MudPicker{T}"/>.
        /// </summary>
        public static TimeSpan Duration { get; set; } = TransitionDefaults.Duration;
    }

    public static class PickerDefaults
    {
        /// <summary>
        /// The default transition delay for <see cref="MudOverlay"/> and <see cref="MudPicker{T}"/>.
        /// </summary>
        public static TimeSpan Delay { get; set; } = TransitionDefaults.Delay;

        /// <summary>
        /// The default transition time for components like <see cref="MudTooltip"/>, <see cref="MudOverlay"/>, <see cref="MudPicker{T}"/>.
        /// </summary>
        public static TimeSpan Duration { get; set; } = TransitionDefaults.Duration;
    }

    public static class PopoverDefaults
    {
        /// <summary>
        /// The default elevation level for <see cref="MudPopover"/>.
        /// </summary>
        public static int Elevation { get; set; } = 8;
    }

    public static class TooltipDefaults
    {
        /// <summary>
        /// The default transition delay for <see cref="MudOverlay"/> and <see cref="MudPicker{T}"/>.
        /// </summary>
        public static TimeSpan Delay { get; set; } = TransitionDefaults.Delay;

        /// <summary>
        /// The default transition time for components like <see cref="MudTooltip"/>, <see cref="MudOverlay"/>, <see cref="MudPicker{T}"/>.
        /// </summary>
        public static TimeSpan Duration { get; set; } = TransitionDefaults.Duration;
    }

    public static class TransitionDefaults
    {
        /// <summary>
        /// The default transition delay for <see cref="MudOverlay"/> and <see cref="MudPicker{T}"/>.
        /// </summary>
        public static TimeSpan Delay { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// The default transition time for components like <see cref="MudTooltip"/>, <see cref="MudOverlay"/>, <see cref="MudPicker{T}"/>.
        /// </summary>
        public static TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(251);
    }

    /// <summary>
    /// Global unhandled exception handler for such exceptions which can not be bubbled up. Note: this is not a global catch-all.
    /// It just allows the user to handle such exceptions which were suppressed inside MudBlazor using Task.AndForget() in places
    /// where it is impossible to await the task. Exceptions in user code or in razor files will still crash your app if you are not carefully
    /// handling everything with <ErrorBoundary></ErrorBoundary>.
    /// </summary>
    public static Action<Exception> UnhandledExceptionHandler { get; set; } = OnDefaultExceptionHandler;

    /// <summary>
    /// Gets or sets whether old parameters that were renamed in v7.0.0 should cause a runtime exception.
    /// </summary>
    /// <remarks>
    /// Razor silently ignores parameters which don't exist. Since v7.0.0 renamed so many parameters we want
    /// to help our users find old parameters they missed by throwing a runtime exception.
    ///
    /// TODO: Remove this later. At the moment, we don't know yet when will be the best time to remove it.
    /// Sometime when the v7 version has stabilized.
    /// </remarks>
    public static bool EnableIllegalRazorParameterDetection = true;

    /// <summary>
    /// Note: the user can overwrite this default handler with their own implementation. The default implementation
    /// makes sure that the unhandled exceptions don't go unnoticed
    /// </summary>
    private static void OnDefaultExceptionHandler(Exception ex)
    {
        Console.Write(ex);
    }
}
