// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Tracks the dismissible overlays that are currently visible and lets host code close the most recently opened one.
/// </summary>
/// <remarks>
/// A dismissible overlay is any visible <see cref="MudOverlay"/> with <see cref="MudOverlay.AutoClose"/> enabled, which
/// is the backdrop used by menus, selects, autocompletes, pickers, and dialogs, as well as by user-created overlays.
/// <para>
/// This exists for platforms that own a global "back"/dismiss gesture outside the DOM, where a synthetic key event is
/// not reliable, for example the Android hardware Back button in a Blazor Hybrid (MAUI) app (a WebView does not forward a
/// hardware Escape to the DOM) or browser Back / <c>popstate</c> in a PWA. Such a handler can dismiss the open overlay
/// before navigating:
/// <code>
/// if (OverlayService.HasVisibleOverlay)
///     await OverlayService.CloseLastOverlayAsync();
/// else
///     Navigate();
/// </code>
/// </para>
/// </remarks>
public interface IOverlayService
{
    /// <summary>
    /// Gets a value indicating whether any dismissible (auto-close) overlay is currently visible.
    /// </summary>
    bool HasVisibleOverlay { get; }

    /// <summary>
    /// Closes the most recently opened dismissible overlay, running the same close path as clicking outside it.
    /// </summary>
    /// <returns><c>true</c> if an overlay was closed; otherwise <c>false</c> when none was visible.</returns>
    /// <remarks>
    /// Closing mutates component state, so call this from within <see cref="Microsoft.AspNetCore.Components.ComponentBase.InvokeAsync(System.Func{System.Threading.Tasks.Task})"/>
    /// (or otherwise on the renderer's synchronization context) when invoking it from outside the Blazor render loop.
    /// </remarks>
    Task<bool> CloseLastOverlayAsync();

    /// <summary>
    /// Registers a dismissible overlay's close callback and returns a token that unregisters it when disposed.
    /// </summary>
    /// <param name="closeAsync">The callback that closes the overlay, invoked by <see cref="CloseLastOverlayAsync"/>.</param>
    /// <returns>A token that unregisters the overlay when disposed (for example when it is hidden or disposed).</returns>
    /// <remarks>This is called internally by <see cref="MudOverlay"/> while it is visible with auto-close enabled.</remarks>
    IDisposable RegisterOverlay(Func<Task> closeAsync);
}
