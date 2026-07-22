// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MudBlazor.Services
{
    public class JsEventOptions
    {
        /// <summary>
        /// <para>Class of the target node which should be observed for keyboard events</para>
        /// <para>Note: this must be a single class</para>
        /// </summary>
        public string TargetClass { get; set; }

        /// <summary>
        /// The tag name of the element to register events with. Must be all uppercase, like "INPUT"
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// Report resize events in the browser's console.
        /// </summary>
        public bool EnableLogging { get; set; } = false;
    }
}
