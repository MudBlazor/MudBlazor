﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Services;

#nullable enable
/// <summary>
/// <para>Configuration for preventDefault() and stopPropagation() control</para>
/// <para>
/// For PreventDown, PreventUp, StopDown and StopUp the configuration which key combinations should match
/// is a JavaScript boolean expression.
/// </para>
/// <para>
/// Examples:
/// For the examples, let's assume the Tab key was pressed.
/// Note: for combinations of more than one modifier the following order of modifiers must be followed strictly: shift+ctrl+alt+meta
/// </para>
/// <para>
///  * Don't prevent key down:
///          PreventDown=null or PreventDown="none"
///  * Prevent key down of unmodified keystrokes such as "Tab":
///          PreventDown="key+none"
///  * Prevent key down of Tab and Ctrl+Tab
///          PreventDown="key+none|key+ctrl"
///  * Prevent key down of just Ctrl+Tab
///          PreventDown="key+ctrl"
///  * Prevent key down of Ctrl+Tab and Shift+Tab but not Shift+Ctrl+Tab:
///          PreventDown="key+shift|key+ctrl"
///  * Prevent key down of Shift+Ctrl+Tab and Ctrl+Tab but not Shift+Tab:
///          PreventDown="key+shift+ctrl|key+ctrl"
///  * Prevent any combination of key and modifiers, but not the unmodified key:
///          PreventDown="key+any"
///  * Prevent any combination of key and modifiers, even the unmodified key:
///          PreventDown="any"
/// </para>
/// </summary>
public class KeyOptions
{
    /// <summary>
    /// <para>JavaScript keyboard event.key</para>
    /// <para>
    /// Examples: " " for space, "Tab" for tab, "a" for lowercase A-key.
    /// Also allowed: JS regex such as "/[a-z]/" or "/a|b/" but NOT "/[a-z]/g" or "/[a-z]/i"
    ///      regex must be enclosed in two forward slashes!
    /// </para>
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Subscribe down key and invoke event KeyDown on C# side
    /// </summary>
    public bool SubscribeDown { get; set; }

    /// <summary>
    /// Subscribe up key and invoke event KeyUp on C# side
    /// </summary>
    public bool SubscribeUp { get; set; }

    /// <summary>
    /// Configuration for preventDefault() on key down events.
    /// </summary>
    public string PreventDown { get; set; } = "none";

    /// <summary>
    /// Configuration for preventDefault() on key up events.
    /// </summary>
    public string PreventUp { get; set; } = "none";

    /// <summary>
    /// Configuration for stopPropagation() on key down events.
    /// </summary>
    public string StopDown { get; set; } = "none";

    /// <summary>
    /// Configuration for stopPropagation() on key up events.
    /// </summary>
    public string StopUp { get; set; } = "none";

    public static KeyOptions Of(string? key = null,
        bool subscribeDown = false,
        bool subscribeUp = false,
        string preventDown = "none",
        string preventUp = "none",
        string stopDown = "none",
        string stopUp = "none")
    {
        return new KeyOptions
        {
            Key = key,
            PreventDown = preventDown,
            PreventUp = preventUp,
            SubscribeDown = subscribeDown,
            SubscribeUp = subscribeUp,
            StopDown = stopDown,
            StopUp = stopUp
        };
    }
}
