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
        /// The default gap between items for <see cref="MudStack"/>, measured in increments of <c>4px</c>..
        /// </summary>
        /// <remarks>
        /// Default is <c>3</c>.
        /// Maximum is <c>20</c>.
        /// </remarks>
        public static int Spacing { get; set; } = 3;
    }

    /// <summary>
    /// Defaults for the <see cref="MudTooltip"/> component.
    /// </summary>
    public static class TooltipDefaults
    {
        /// <summary>
        /// The default transition delay for <see cref="MudTooltip"/>.
        /// </summary>
        public static TimeSpan Delay { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// The default transition time for <see cref="MudTooltip"/>.
        /// </summary>
        public static TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(251);
    }

    /// <summary>
    /// Defaults for components which use transitions.
    /// </summary>
    public static class TransitionDefaults
    {
        /// <summary>
        /// The default transition delay for overlays, popovers, and pickers.
        /// </summary>
        public static TimeSpan Delay { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// The default transition time for overlays, popovers, and pickers.
        /// </summary>
        public static TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(251);
    }

    public static bool Rounded { get; set; }

    /// <summary>
    /// The handler for unhandled component exceptions.
    /// </summary>
    /// <remarks>
    /// Exceptions which use this handler are typically rare, such as errors which occur during a "fire-and-forget" <see cref="Task"/> which cannot be awaited.<br />
    /// By default, exceptions are logged to the console via <see cref="Console.Write(object?)"/>.<br />
    /// To handle all .NET exceptions, see: <see href="https://learn.microsoft.com/aspnet/core/fundamentals/error-handling">Handle errors in ASP.NET Core</see>.
    /// </remarks>
    public static Action<Exception> UnhandledExceptionHandler { get; set; } = Console.Write;
}
