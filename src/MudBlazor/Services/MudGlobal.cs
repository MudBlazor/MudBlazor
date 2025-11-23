// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// <para>
/// Static properties that let you control the default behavior of some parts of MudBlazor.
/// </para>
/// <para>
/// <b>Warning:</b> This feature is under development and breaking changes to the API <b>will occur</b> between releases.
/// See <see href="https://mudblazor.com/customization/globals#usage">our website</see> for more info including our support policy.
/// </para>
/// </summary>
public static class MudGlobal
{
    /// <summary>
    /// Default settings for <see cref="MudDialog"/>.
    /// <br/>
    /// <b>Warning:</b> This feature is under development and breaking changes to the API <b>will occur</b> between releases.
    /// </summary>
    public static class DialogDefaults
    {
        /// <summary>
        /// The element which will receive focus when this <see cref="MudDialog"/> is shown.
        /// </summary>
        public static DefaultFocus DefaultFocus { get; set; } = DefaultFocus.Element;
    }

    /// <summary>
    /// Default settings for <see cref="MudMenu"/>.
    /// <br/>
    /// <b>Warning:</b> This feature is under development and breaking changes to the API <b>will occur</b> between releases.
    /// </summary>
    public static class MenuDefaults
    {
        /// <summary>
        /// The delay in milliseconds before a <see cref="MudMenu"/> is shown when hovered, or hidden after the cursor moves away.
        /// </summary>
        public static int HoverDelay { get; set; } = 300;
    }

    /// <summary>
    /// Default settings for <see cref="MudPopover"/>.
    /// <br/>
    /// <b>Warning:</b> This feature is under development and breaking changes to the API <b>will occur</b> between releases.
    /// </summary>
    public static class PopoverDefaults
    {
        /// <summary>
        /// Prevents interaction with background elements.
        /// </summary>
        /// <remarks>
        /// Only applies to components that use a <see cref="MudPopover"/> in conjunction with a <see cref="MudOverlay"/>
        /// to close the popover when a user clicks outside, such as <see cref="MudSelect{T}"/>.
        /// </remarks>
        public static bool ModalOverlay { get; set; }
    }

    /// <summary>
    /// Default settings for <see cref="MudTooltip"/>.
    /// <br/>
    /// <b>Warning:</b> This feature is under development and breaking changes to the API <b>will occur</b> between releases.
    /// </summary>
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

    /// <summary>
    /// Default settings for transitions in MudBlazor components.
    /// <br/>
    /// <b>Warning:</b> This feature is under development and breaking changes to the API <b>will occur</b> between releases.
    /// </summary>
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
    /// The handler for unhandled MudBlazor component exceptions.
    /// </summary>
    /// <remarks>
    /// Exceptions which use this handler are typically rare, such as errors which occur during a "fire-and-forget" <see cref="Task"/> which cannot be awaited.<br />
    /// By default, exceptions are logged to the console via <see cref="Console.Write(object?)"/>.<br />
    /// To handle all .NET exceptions, see: <see href="https://learn.microsoft.com/aspnet/core/fundamentals/error-handling">Handle errors in ASP.NET Core</see>.
    /// </remarks>
    public static Action<Exception> UnhandledExceptionHandler { get; set; } = (exception) => Console.Write(exception);
}
