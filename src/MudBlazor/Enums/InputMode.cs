// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace MudBlazor;

/// <summary>
/// The inputmode global attribute is an enumerated attribute that hints at the type of data that might be entered by the user while editing the element or its contents.
/// Not supported by safari. Use Pattern to achieve special mobile keyboards in safari.
/// https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/inputmode
/// </summary>
public enum InputMode
{
    /// <summary>
    /// No virtual keyboard. For when the page implements its own keyboard input control.
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/inputmode#none">Reference</a>
    /// </summary>
    [Description("none")]
    none,

    /// <summary>
    /// Default. Standard input keyboard for the user's current locale.
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/inputmode#text">Reference</a>
    /// </summary>
    [Description("text")]
    text,

    /// <summary>
    /// Fractional numeric input keyboard containing the digits and decimal separator for the user's locale (typically . or ,). Devices may or may not show a minus key (-).
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/inputmode#decimal">Reference</a>
    /// </summary>
    [Description("decimal")]
    @decimal,

    /// <summary>
    /// Numeric input keyboard, but only requires the digits 0–9. Devices may or may not show a minus key.
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/inputmode#numeric">Reference</a>
    /// </summary>
    [Description("numeric")]
    numeric,

    /// <summary>
    /// A telephone keypad input, including the digits 0–9, the asterisk (*), and the pound (#) key. Inputs that require a telephone number should typically use &lt;input type="tel"&gt;instead.
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/inputmode#tel">Reference</a>
    /// </summary>
    [Description("tel")]
    tel,

    /// <summary>
    /// A virtual keyboard optimized for search input. For instance, the return/submit key may be labeled “Search”, along with possible other optimizations. Inputs that require a search query should typically use &lt;input type="search"&gt; instead.
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/inputmode#search">Reference</a>
    /// </summary>
    [Description("search")]
    search,

    /// <summary>
    /// A virtual keyboard optimized for entering email addresses. Typically includes the @ character as well as other optimizations. Inputs that require email addresses should typically use &lt;input type="email"&gt; instead.
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/inputmode#email">Reference</a>
    /// </summary>
    [Description("email")]
    email,

    /// <summary>
    /// A keypad optimized for entering URLs. This may have the / key more prominent, for example. Enhanced features could include history access and so on. Inputs that require a URL should typically use &lt;input type="url"&gt; instead.
    /// <a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Global_attributes/inputmode#url">Reference</a>
    /// </summary>
    [Description("url")]
    url
}
