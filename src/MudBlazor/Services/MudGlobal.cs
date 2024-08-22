// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor;

public static class MudGlobal
{
    public static class ButtonDefaults
    {
        /// <summary>
        /// The default color for <see cref="MudButton"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to Colour.Default
        /// </remarks>
        public static Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The default size for <see cref="MudButton"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to Size.Medium
        /// </remarks>
        public static Size Size { get; set; } = Size.Medium;
    }

    public static class CardDefaults
    {
        /// <summary>
        /// The default elevation level for <see cref="MudCard"/>.
        /// </summary>
        public static int Elevation { get; set; } = 1;

        /// <summary>
        /// The default square setting for <see cref="MudCard"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, disables rounded corners.
        /// </remarks>
        public static bool Square { get; set; }

        /// <summary>
        /// The default outline setting for <see cref="MudCard"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, shows an outline around this card.
        /// </remarks>
        public static bool Outlined { get; set; }
    }

    public static class DataGridDefaults
    {
        /// <summary>
        /// The default elevation level for <see cref="MudDataGrid{T}"/>.
        /// </summary>
        public static int Elevation { set; get; } = 1;

        /// <summary>
        /// The default square setting for <see cref="MudDataGrid{T}"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, disables rounded corners.
        /// </remarks>
        public static bool Square { get; set; }

        /// <summary>
        /// The default outlined setting for <see cref="MudDataGrid{T}"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, shows an outline around this grid.
        /// </remarks>
        public static bool Outlined { get; set; }

        /// <summary>
        /// The default bordered setting for <see cref="MudDataGrid{T}"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, shows left and right borders for each column.
        /// </remarks>
        public static bool Bordered { get; set; }

        /// <summary>
        /// The default dense setting for <see cref="MudDataGrid{T}"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, uses compact padding.
        /// </remarks>
        public static bool Dense { get; set; }

        /// <summary>
        /// The default hover setting for <see cref="MudDataGrid{T}"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, highlights rows when hovering over them.
        /// </remarks>
        public static bool Hover { get; set; }

        /// <summary>
        /// The default striped setting for <see cref="MudDataGrid{T}"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, shows alternating row styles.
        /// </remarks>
        public static bool Striped { get; set; }

        /// <summary>
        /// The default fixed header setting for <see cref="MudDataGrid{T}"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, fixes the header in place even as the grid is scrolled.
        /// </remarks>
        public static bool FixedHeader { get; set; }

        /// <summary>
        /// The default fixed footer setting for <see cref="MudDataGrid{T}"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, fixes the footer in place even as the grid is scrolled.
        /// </remarks>
        public static bool FixedFooter { get; set; }

        /// <summary>
        /// The default virtualize setting for <see cref="MudDataGrid{T}"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, renders only visible items instead of all items.
        /// </remarks>
        public static bool Virtualize { get; set; }
    }

    public static class DialogDefaults
    {
        /// <summary>
        /// The default <see cref="MudDialog.DefaultFocus"/>.
        /// </summary>
        public static DefaultFocus DefaultFocus { get; set; } = DefaultFocus.Element;
    }

    public static class GridDefaults
    {
        /// <summary>
        /// The default spacing between items for <see cref="MudGrid"/>, measured in increments of <c>4px</c>.
        /// <br/>
        /// Maximum is 20.
        /// </summary>
        /// <remarks>
        /// Defaults to 6.
        /// </remarks>
        public static int Spacing { set; get; } = 6;
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

    public static class LinkDefaults
    {
        /// <summary>
        /// The default color for <see cref="MudLink"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to Color.Primary
        /// </remarks>
        public static Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// The default typography variant for <see cref="MudLink"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to Typo.body1
        /// </remarks>
        public static Typo Typo { get; set; } = Typo.body1;

        /// <summary>
        /// The default underline setting for <see cref="MudLink"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to underline on hover.
        /// </remarks>
        public static Underline Underline { get; set; } = Underline.Hover;
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

    public static class StackDefaults
    {
        /// <summary>
        /// The default justify setting for <see cref="MudStack"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, items will be placed horizontally in a row instead of vertically.
        /// </remarks>
        public static bool Row { get; set; }

        /// <summary>
        /// The default reverse setting for <see cref="MudStack"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, order of the items will be reversed.
        /// </remarks>
        public static bool Reverse { get; set; }

        /// <summary>
        /// The default gap between items for <see cref="MudStack"/>, measured in increments of <c>4px</c>..
        /// </summary>
        /// <remarks>
        /// Default is <c>3</c>.
        /// Maximum is <c>20</c>.
        /// </remarks>
        public static int Spacing { get; set; } = 3;
    }

    public static class TabDefaults
    {
        /// <summary>
        /// The default rounding setting for <see cref="MudTabs"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, the tabs will be rounded.
        /// </remarks>
        public static bool Rounded { get; set; }

        /// <summary>
        /// The default border setting for <see cref="MudTabs"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, sets a border between the content and the tab header depending on the position.
        /// </remarks>
        public static bool Border { get; set; }

        /// <summary>
        /// The default outlined setting for <see cref="MudTabs"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, the tab header will be outlined.
        /// </remarks>
        public static bool Outlined { get; set; }

        /// <summary>
        /// The default centered setting for <see cref="MudTabs"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, the tab items will be centered.
        /// </remarks>
        public static bool Centered { get; set; }

        /// <summary>
        /// The default hide slider setting for <see cref="MudTabs"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, the slider will be hidden.
        /// </remarks>
        public static bool HideSlider { get; set; }

        /// <summary>
        /// The default show scroll buttons setting for <see cref="MudTabs"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, the scroll buttons will always be shown.
        /// </remarks>
        public static bool AlwaysShowScrollButtons { get; set; }

        /// <summary>
        /// The default maximum tab height setting for <see cref="MudTabs"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to no maximum height.
        /// </remarks>
        public static int? MaxHeight { get; set; } = null;

        /// <summary>
        /// The default minimum tab width setting for <see cref="MudTabs"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to 160px.
        /// </remarks>
        public static string MinimumTabWidth { get; set; } = "160px";

        /// <summary>
        /// The default position for <see cref="MudTabs"/>.
        /// </summary>
        public static Position Position { get; set; } = Position.Top;

        /// <summary>
        /// The default colour for <see cref="MudTabs"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to Color.Default.
        /// </remarks>
        public static Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The default slider color for <see cref="MudTabs"/>.
        /// </summary>
        public static Color SliderColor { get; set; } = Color.Inherit;

        /// <summary>
        /// The default elevation setting for <see cref="MudTabs"/>.
        /// </summary>
        public static int Elevation { set; get; } = 0;

        /// <summary>
        /// The default apply effects to container setting for <see cref="MudTabs"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, the effects will be applied to the container as well.
        /// </remarks>
        public static bool ApplyEffectsToContainer { get; set; }
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
    /// </remarks>
    [Obsolete("This field is obsolete and has no function due to the new Analyzer. It will be removed in a future version.", true)]
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
