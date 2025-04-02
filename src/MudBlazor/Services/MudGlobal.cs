// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// A collection of settings that let you control the default behavior or appearance of MudBlazor components.
/// </summary>
public static class MudGlobal
{
    public static class ButtonDefaults
    {
        /// <summary>
        /// The color of the <see cref="MudButton"/> icons.
        /// </summary>
        public static Color IconColor { get; set; } = Color.Inherit;
        
        /// <summary>
        /// The size of the <see cref="MudButton"/> icons.
        /// </summary>
        public static Size? IconSize { get; set; }

        /// <summary>
        /// The CSS classes applied to the <see cref="MudButton"/> icons.
        /// </summary>
        public static string? IconClass { get; set; }
        
        /// <summary>
        /// The color of the <see cref="MudButton"/>.
        /// </summary>
        public static Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The display variation to use for <see cref="MudButton"/>.
        /// </summary>
        public static Variant Variant { get; set; } = Variant.Text;
        
        /// <summary>
        /// Expands the <see cref="MudButton"/> to 100% of the container width.
        /// </summary>
        public static bool FullWidth { get; set; }
        
        /// <summary>
        /// The size of the <see cref="MudButton"/>.
        /// </summary>
        public static Size Size { get; set; } = Size.Medium;
        
        /// <summary>
        /// Allows the user to interact with this <see cref="MudButton"/>.
        /// </summary>
        public static bool Disabled { get; set; }
        
        /// <summary>
        /// Displays a shadow under the <see cref="MudButton"/>.
        /// </summary>
        public static bool DropShadow { get; set; } = true;
        
        /// <summary>
        /// Shows a ripple effect when the user clicks the <see cref="MudButton"/>.
        /// </summary>
        public static bool Ripple { get; set; } = true;
        
        /// <summary>
        /// The CSS classes applied to the <see cref="MudButton"/>.
        /// </summary>
        public static string? Class { get; set; }

        /// <summary>
        /// The CSS styles applied to the <see cref="MudButton"/>.
        /// </summary>
        public static string? Style { get; set; }
    }

    public static class DialogDefaults
    {
        /// <summary>
        /// The element which will receive focus when this <see cref="MudDialog"/> is shown.
        /// </summary>
        public static DefaultFocus DefaultFocus { get; set; } = DefaultFocus.Element;
    }

    public static class GridDefaults
    {
        /// <summary>
        /// The gap between items in <see cref="MudGrid"/>, measured in increments of <c>4px</c>.
        /// </summary>
        public static int Spacing { set; get; } = 6;
    }

    public static class InputDefaults
    {
        /// <summary>
        /// Shows the label inside the input if no <c>Value</c> is specified.
        /// </summary>
        public static bool ShrinkLabel { get; set; }

        /// <summary>
        /// The appearance variation to use.
        /// </summary>
        public static Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// The amount of vertical spacing for this input.
        /// </summary>
        public static Margin Margin { get; set; } = Margin.None;
    }

    public static class LinkDefaults
    {
        /// <summary>
        /// The color of the <see cref="MudLink"/>.
        /// </summary>
        public static Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// The typography variant to use for <see cref="MudLink"/>.
        /// </summary>
        public static Typo Typo { get; set; } = Typo.body1;

        /// <summary>
        /// Applies an underline to the <see cref="MudLink"/>.
        /// </summary>
        public static Underline Underline { get; set; } = Underline.Hover;
    }

    public static class MenuDefaults
    {
        /// <summary>
        /// The delay in milliseconds before a <see cref="MudMenu"/> is shown when hovered, or hidden after the cursor moves away.
        /// </summary>
        public static int HoverDelay { get; set; } = 300;
    }

    public static class PopoverDefaults
    {
        /// <summary>
        /// The amount of drop shadow to apply to <see cref="MudPopover"/>.
        /// </summary>
        public static int Elevation { get; set; } = 8;
    }

    public static class StackDefaults
    {
        /// <summary>
        /// The gap between items in <see cref="MudStack"/>, measured in increments of <c>4px</c>.
        /// </summary>
        public static int Spacing { get; set; } = 3;
    }

    public static class TooltipDefaults
    {
        /// <summary>
        /// The amount of time in milliseconds to wait from opening the <see cref="MudTooltip"/> before beginning to perform the transition. 
        /// </summary>
        public static TimeSpan Delay { get; set; } = TransitionDefaults.Delay;

        /// <summary>
        /// The length of time that the opening transition for <see cref="MudTooltip"/> takes to complete.
        /// </summary>
        public static TimeSpan Duration { get; set; } = TransitionDefaults.Duration;
    }

    public static class TransitionDefaults
    {
        /// <summary>
        /// The length of time that the opening transition takes to complete.
        /// </summary>
        public static TimeSpan Delay { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// The amount of time in milliseconds to wait from opening the popover before beginning to perform the transition. 
        /// </summary>
        public static TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(251);
    }

    /// <summary>
    /// Applies regular rounding by default; additional rounding if set to true; or squares them if set to false for MudBlazor components.
    /// </summary>
    public static bool? Rounded { get; set; }

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
