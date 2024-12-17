// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Settings which control the default behavior and appearance of MudBlazor components.
/// </summary>
public static class MudGlobal
{
    /// <summary>
    /// Defaults for the <see cref="MudButton"/> component.
    /// </summary>
    public static class ButtonDefaults
    {
        /// <summary>
        /// The default color for <see cref="MudButton"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        public static Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The default size for <see cref="MudButton"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.
        /// </remarks>
        public static Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The default variant for <see cref="MudButton"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Text"/>.
        /// </remarks>
        public static Variant Variant { get; set; } = Variant.Text;
    }

    /// <summary>
    /// Defaults for the <see cref="MudCard"/> component.
    /// </summary>
    public static class CardDefaults
    {
        /// <summary>
        /// The default elevation level for <see cref="MudCard"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>1</c>.
        /// </remarks>
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

    /// <summary>
    /// Defaults for the <see cref="MudDataGrid{T}"/> component.
    /// </summary>
    public static class DataGridDefaults
    {
        /// <summary>
        /// The default elevation level for <see cref="MudDataGrid{T}"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>1</c>.
        /// </remarks>
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

    /// <summary>
    /// Defaults for the <see cref="MudDialog"/> component.
    /// </summary>
    public static class DialogDefaults
    {
        /// <summary>
        /// The default focus for <see cref="MudDialog"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="DefaultFocus.Element"/>.
        /// </remarks>
        public static DefaultFocus DefaultFocus { get; set; } = DefaultFocus.Element;
    }

    /// <summary>
    /// Defaults for the <see cref="MudGrid"/> component.
    /// </summary>
    public static class GridDefaults
    {
        /// <summary>
        /// The default spacing between items in a <see cref="MudGrid"/>, measured in increments of <c>4px</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>6</c> (24px).  
        /// Maximum is <c>20</c> (80px).
        /// </remarks>
        public static int Spacing { set; get; } = 6;
    }

    /// <summary>
    /// Defaults for the <see cref="MudBaseInput{T}"/> component.
    /// </summary>
    public static class InputDefaults
    {
        /// <summary>
        /// The default label shrink setting for <see cref="MudBaseInput{T}"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, the label will not move into the input when the input is empty.
        /// </remarks>
        public static bool ShrinkLabel { get; set; }

        /// <summary>
        /// The default variant for <see cref="MudBaseInput{T}"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Text"/>.
        /// </remarks>
        public static Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// The default margin for <see cref="MudBaseInput{T}"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Margin.None"/>.
        /// </remarks>
        public static Margin Margin { get; set; } = Margin.None;
    }

    /// <summary>
    /// Defaults for the <see cref="MudLink"/> component.
    /// </summary>
    public static class LinkDefaults
    {
        /// <summary>
        /// The default color for <see cref="MudLink"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Primary"/>.
        /// </remarks>
        public static Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// The default typography variant for <see cref="MudLink"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Typo.body1"/>.
        /// </remarks>
        public static Typo Typo { get; set; } = Typo.body1;

        /// <summary>
        /// The default underline setting for <see cref="MudLink"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Underline.Hover"/>.
        /// </remarks>
        public static Underline Underline { get; set; } = Underline.Hover;
    }

    /// <summary>
    /// Defaults for the <see cref="MudMenu"/> component.
    /// </summary>
    public static class MenuDefaults
    {
        /// <summary>
        /// The time in milliseconds before the menu opens on pointer hover or closes on pointer leave.
        /// </summary>
        public static int HoverDelay { get; set; } = 300;
    }

    /// <summary>
    /// Defaults for the <see cref="MudOverlay"/> component.
    /// </summary>
    public static class OverlayDefaults
    {
        /// <summary>
        /// The default transition delay for <see cref="MudOverlay"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="TransitionDefaults.Delay"/>.
        /// </remarks>
        public static TimeSpan Delay { get; set; } = TransitionDefaults.Delay;

        /// <summary>
        /// The default transition time for <see cref="MudOverlay"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="TransitionDefaults.Duration"/>.
        /// </remarks>
        public static TimeSpan Duration { get; set; } = TransitionDefaults.Duration;
    }

    /// <summary>
    /// Defaults for the <see cref="MudPicker{T}"/> component.
    /// </summary>
    public static class PickerDefaults
    {
        /// <summary>
        /// The default transition delay for <see cref="MudPicker{T}"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="TransitionDefaults.Delay"/>.
        /// </remarks>
        public static TimeSpan Delay { get; set; } = TransitionDefaults.Delay;

        /// <summary>
        /// The default transition time for <see cref="MudPicker{T}"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="TransitionDefaults.Duration"/>.
        /// </remarks>
        public static TimeSpan Duration { get; set; } = TransitionDefaults.Duration;
    }

    /// <summary>
    /// Defaults for the <see cref="MudPopover"/> component.
    /// </summary>
    public static class PopoverDefaults
    {
        /// <summary>
        /// The default elevation level for <see cref="MudPopover"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>8</c>.
        /// </remarks>
        public static int Elevation { get; set; } = 8;
    }

    /// <summary>
    /// Defaults for the <see cref="MudStack"/> component.
    /// </summary>
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

    /// <summary>
    /// Defaults for the <see cref="MudTabs"/> component.
    /// </summary>
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
        /// Defaults to <c>null</c> (no maximum height).
        /// </remarks>
        public static int? MaxHeight { get; set; } = null;

        /// <summary>
        /// The default minimum tab width setting for <see cref="MudTabs"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>160px</c>.
        /// </remarks>
        public static string MinimumTabWidth { get; set; } = "160px";

        /// <summary>
        /// The default position for <see cref="MudTabs"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Position.Top"/>.
        /// </remarks>
        public static Position Position { get; set; } = Position.Top;

        /// <summary>
        /// The default color for <see cref="MudTabs"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref=" Color.Default"/>.
        /// </remarks>
        public static Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The default slider color for <see cref="MudTabs"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref=" Color.Inherit"/>.
        /// </remarks>
        public static Color SliderColor { get; set; } = Color.Inherit;

        /// <summary>
        /// The default elevation setting for <see cref="MudTabs"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.
        /// </remarks>
        public static int Elevation { set; get; } = 0;

        /// <summary>
        /// The default apply effects to container setting for <see cref="MudTabs"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. When <c>true</c>, the effects will be applied to the container as well.
        /// </remarks>
        public static bool ApplyEffectsToContainer { get; set; }
    }

    /// <summary>
    /// Defaults for the <see cref="MudTooltip"/> component.
    /// </summary>
    public static class TooltipDefaults
    {
        /// <summary>
        /// The default transition delay for <see cref="MudTooltip"/>.
        /// </summary>
        public static TimeSpan Delay { get; set; } = TransitionDefaults.Delay;

        /// <summary>
        /// The default transition time for <see cref="MudTooltip"/>.
        /// </summary>
        public static TimeSpan Duration { get; set; } = TransitionDefaults.Duration;
    }

    /// <summary>
    /// Defaults for MudBlazor components which use transitions.
    /// </summary>
    public static class TransitionDefaults
    {
        /// <summary>
        /// The default transition delay for <see cref="MudOverlay"/>, <see cref="MudPicker{T}"/>, <see cref="MudPopover"/>, and <see cref="MudTooltip"/>.
        /// </summary>
        public static TimeSpan Delay { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// The default transition time for components like <see cref="MudOverlay"/>, <see cref="MudPicker{T}"/>, <see cref="MudPopover"/>, and <see cref="MudTooltip"/>.
        /// </summary>
        public static TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(251);
    }

    /// <summary>
    /// Defaults for the <see cref="MudPaper"/> component.
    /// </summary>
    public static class PaperDefaults
    {
        /// <summary>
        /// Gets or sets the default elevation level for <see cref="MudPaper"/>.
        /// </summary>
        public static int Elevation { get; set; } = 1;

        /// <summary>
        /// Gets or sets the default square setting for <see cref="MudPaper"/>.
        /// </summary>
        public static bool Square { get; set; }

        /// <summary>
        /// Gets or sets the default square setting for <see cref="MudPaper"/>.
        /// </summary>
        public static bool Outlined { get; set; }
    }

    /// <summary>
    /// The handler for unhandled MudBlazor component exceptions.
    /// </summary>
    /// <remarks>
    /// Exceptions which use this handler are typically rare, such as errors which occur during a "fire-and-forget" <see cref="Task"/> which cannot be awaited.<br />
    /// By default, exceptions are logged to the console via <see cref="Console.Write(object?)"/>.<br />
    /// To handle all .NET exceptions, see: <see href="https://learn.microsoft.com/aspnet/core/fundamentals/error-handling">Handle errors in ASP.NET Core</see>.
    /// </remarks>
    public static Action<Exception> UnhandledExceptionHandler { get; set; } = (exception) => Console.Write(exception);
}
