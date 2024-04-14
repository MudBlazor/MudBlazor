// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor;

public static class MudGlobal
{
    /// <summary>
    /// The default transition delay for <see cref="MudOverlay"/> and <see cref="MudPicker{T}"/>.
    /// </summary>
    public static TimeSpan TransitionDelay { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// The default transition time for components like <see cref="MudTooltip"/>, <see cref="MudOverlay"/>, <see cref="MudPicker{T}"/>.
    /// </summary>
    public static TimeSpan TransitionDuration { get; set; } = TimeSpan.FromMilliseconds(251);

    /// <summary>
    /// The default <see cref="MudTooltip.Delay"/>.
    /// </summary>
    public static TimeSpan TooltipDelay { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Global unhandled exception handler for such exceptions which can not be bubbled up. Note: this is not a global catch-all.
    /// It just allows the user to handle such exceptions which were suppressed inside MudBlazor using Task.AndForget() in places
    /// where it is impossible to await the task. Exceptions in user code or in razor files will still crash your app if you are not carefully
    /// handling everything with <ErrorBoundary></ErrorBoundary>.
    /// </summary>
    public static Action<Exception> UnhandledExceptionHandler { get; set; } = OnDefaultExceptionHandler;

    /// <summary>
    /// Note: the user can overwrite this default handler with their own implementation. The default implementation
    /// makes sure that the unhandled exceptions don't go unnoticed
    /// </summary>
    private static void OnDefaultExceptionHandler(Exception ex)
    {
        Console.Write(ex);
    }
}
