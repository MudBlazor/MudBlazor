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
        public static Size Size { get; set; } = Size.Medium;
        public static Variant Variant { get; set; } = Variant.Text;
    }

    public static class CardDefaults
    {
        public static int Elevation { get; set; } = 1;
        public static bool Square { get; set; }
        public static bool Outlined { get; set; }
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
        public static TimeSpan Delay { get; set; } = TimeSpan.Zero;
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
