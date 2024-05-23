// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace MudBlazor
{
    /// <summary>
    /// The inputmode global attribute is an enumerated attribute that hints at the type of data that might be entered by the user while editing the element or its contents.
    /// Not supported by safari. Use Pattern to achieve special mobile keyboards in safari.
    /// https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/inputmode
    /// </summary>
    [ExcludeFromCodeGenerator]
    public enum InputMode
    {
        [Description("No virtual keyboard. For when the page implements its own keyboard input control.")]
        none,

        [Description("Default. Standard input keyboard for the user's current locale.")]
        text,

        [Description("Fractional numeric input keyboard containing the digits and decimal separator for the user's locale (typically . or ,). Devices may or may not show a minus key (-).")]
        @decimal,

        [Description("Numeric input keyboard, but only requires the digits 0–9. Devices may or may not show a minus key.")]
        numeric,

        [Description("A telephone keypad input, including the digits 0–9, the asterisk (*), and the pound (#) key. Inputs that require a telephone number should typically use <input type=\"tel\">instead.")]
        tel,

        [Description("A virtual keyboard optimized for search input. For instance, the return/submit key may be labeled “Search”, along with possible other optimizations. Inputs that require a search query should typically use <input type=\"search\"> instead.")]
        search,

        [Description("A virtual keyboard optimized for entering email addresses. Typically includes the @ character as well as other optimizations. Inputs that require email addresses should typically use <input type=\"email\"> instead.")]
        email,

        [Description("A keypad optimized for entering URLs. This may have the / key more prominent, for example. Enhanced features could include history access and so on. Inputs that require a URL should typically use <input type=\"url\"> instead.")]
        url
    }
}
