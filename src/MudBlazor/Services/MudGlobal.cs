// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// A collection of settings that let you control the default behavior or appearance of MudBlazor components.
/// </summary>
public static class MudGlobal
{
    public static class ButtonDefaults
    {
        public static Color Color { get; set; } = Color.Default;
        public static Variant Variant { get; set; } = Variant.Text;
    }

    public static class DialogDefaults
    {
        public static DefaultFocus DefaultFocus { get; set; } = DefaultFocus.Element;
    }

    public static class GridDefaults
    {
        public static int Spacing { set; get; } = 6;
    }

    public static class InputDefaults
    {
        public static bool ShrinkLabel { get; set; }
        public static Variant Variant { get; set; } = Variant.Text;
        public static Margin Margin { get; set; } = Margin.None;
    }

    public static class LinkDefaults
    {
        public static Color Color { get; set; } = Color.Primary;
        public static Typo Typo { get; set; } = Typo.body1;
        public static Underline Underline { get; set; } = Underline.Hover;
    }

    public static class MenuDefaults
    {
        /// <summary>
        /// The time in milliseconds before a menu is activated by the cursor hovering over it
        /// or before it is hidden after the cursor leaves the menu.
        /// </summary>
        public static int HoverDelay { get; set; } = 300;
    }

    public static class PopoverDefaults
    {
        public static int Elevation { get; set; } = 8;
    }

    public static class StackDefaults
    {
        public static int Spacing { get; set; } = 3;
    }

    public static class TooltipDefaults
    {
        public static TimeSpan Delay { get; set; } = TimeSpan.Zero;
        public static TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(251);
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
    /// Rounds the corners of components by default using the theme's border radius, or squares them if set to false.
    /// </summary>
    public static bool? Rounded { get; set; }

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
