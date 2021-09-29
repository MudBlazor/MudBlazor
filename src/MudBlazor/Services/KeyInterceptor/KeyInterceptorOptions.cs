﻿using System.Collections.Generic;

namespace MudBlazor.Services
{
    public class KeyInterceptorOptions
    {
        /// <summary>
        /// Class of the target node which should be observed for keyboard events
        ///
        /// Note: this must be a single class
        /// </summary>
        public string TargetClass { get; set; }

        /// <summary>
        /// Report resize events in the browser's console.
        /// </summary>
        public bool EnableLogging { get; set; } = false;

        /// <summary>
        /// Intercept configuration for keys of interest
        /// </summary>
        public List<KeyOptions> Keys { get; set; } = new List<KeyOptions>();
    }

    /// <summary>
    /// Configuration for preventDefault() and stopPropagation() control
    ///
    /// For PreventDown, PreventUp, StopDown and StopUp the configuration which key combinations should match
    /// is a Javascript boolean expression.
    ///
    /// Examples:
    /// For the examples, let's assume the Tab key was pressed.
    /// Note: for combinations of more than one modifier the following order of modifiers must be followed strictly: shift+ctrl+alt+meta
    /// 
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
    /// </summary>
    public class KeyOptions
    {
        /// <summary>
        /// Javascript keyboard event.key
        ///
        /// Examples: " " for space, "Tab" for tab, "a" for lowercase A-key.
        /// Also allowed: JS regex such as "/[a-z]/" or "/a|b/" but NOT "/[a-z]/g" or "/[a-z]/i"
        ///      regex must be enclosed in two forward slashes!
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Subscribe down key and invoke event KeyDown on c# side
        /// </summary>
        public bool SubscribeDown { get; set; }

        /// <summary>
        /// Subscribe up key and invoke event KeyUp on c# side
        /// </summary>
        public bool SubscribeUp { get; set; }

        public string PreventDown { get; set; } = "none";
        public string PreventUp { get; set; } = "none";
        public string StopDown { get; set; } = "none";
        public string StopUp { get; set; } = "none";
    }
}
