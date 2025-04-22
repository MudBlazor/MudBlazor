// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Xml.Schema;

namespace MudBlazor.Analyzers
{
    internal static class HTMLAttributes
    {
        internal static string DefaultAttributes
        {
            get => string.Join(",", _defaultAttributes);
        }

        private static ImmutableHashSet<string> _defaultAttributes = (new string[] {
            //attributes https://www.w3schools.com/tags/ref_eventattributes.asp
            "accept", "accept-charset", "accesskey", "action", "align", "alt", "async",
            "autocomplete", "autofocus", "autoplay", "bgcolor", "border", "charset",
            "checked", "cite", "class", "color", "cols", "colspan", "content",
            "contenteditable", "controls", "coords", "data", "datetime",
            "default", "defer", "dir", "dirname", "disabled", "download", "draggable",
            "enctype", "enterkeyhint", "for", "form", "formaction", "headers", "height",
            "hidden", "high", "href", "hreflang", "http-equiv", "id", "inert", "inputmode",
            "ismap", "kind", "label", "lang", "list", "loop", "low", "max", "maxlength",
            "media", "method", "min", "multiple", "muted", "name", "novalidate", "onabort",
            "onafterprint", "onbeforeprint", "onbeforeunload", "onblur", "oncanplay",
            "oncanplaythrough", "onchange", "onclick", "oncontextmenu", "oncopy", "oncuechange",
            "oncut", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave",
            "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied",
            "onended", "onerror", "onfocus", "onhashchange", "oninput", "oninvalid",
            "onkeydown", "onkeypress", "onkeyup", "onload", "onloadeddata", "onloadedmetadata",
            "onloadstart", "onmousedown", "onmousemove", "onmouseout", "onmouseover",
            "onmouseup", "onmousewheel", "onoffline", "ononline", "onpagehide", "onpageshow",
            "onpaste", "onpause", "onplay", "onplaying", "onpopstate", "onprogress",
            "onratechange", "onreset", "onresize", "onscroll", "onsearch", "onseeked",
            "onseeking", "onselect", "onstalled", "onstorage", "onsubmit", "onsuspend",
            "ontimeupdate", "ontoggle", "onunload", "onvolumechange", "onwaiting", "onwheel",
            "open", "optimum", "pattern", "placeholder", "popover", "popovertarget",
            "popovertargetaction", "poster", "preload", "readonly", "rel", "required",
            "reversed", "rows", "rowspan", "sandbox", "scope", "selected", "shape", "size",
            "sizes", "span", "spellcheck", "src", "srcdoc", "srclang", "srcset", "start",
            "step", "style", "tabindex", "target", "title", "translate", "type", "usemap",
            "value", "width", "wrap",

            //events https://www.w3schools.com/jsref/dom_obj_event.asp
            "onabort", "onafterprint", "onanimationend", "onanimationiteration", "onanimationstart", "onbeforeprint",
            "onbeforeunload", "onblur", "oncanplay", "oncanplaythrough", "onchange", "onclick", "oncontextmenu",
            "oncopy", "oncut", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover",
            "ondragstart", "ondrop", "ondurationchange", "onended", "onerror", "onfocus", "onfocusin", "onfocusout",
            "onfullscreenchange", "onfullscreenerror", "onhashchange", "oninput", "oninvalid", "onkeydown",
            "onkeypress", "onkeyup", "onload", "onloadeddata", "onloadedmetadata", "onloadstart", "onmessage",
            "onmousedown", "onmouseenter", "onmouseleave", "onmousemove", "onmouseover", "onmouseout", "onmouseup",
            "onmousewheel", "onoffline", "ononline", "onopen", "onpagehide", "onpageshow", "onpaste", "onpause",
            "onplay", "onplaying", "onpopstate", "onprogress", "onratechange", "onresize", "onreset", "onscroll",
            "onsearch", "onseeked", "onseeking", "onselect", "onshow", "onstalled", "onstorage", "onsubmit",
            "onsuspend", "ontimeupdate", "ontoggle", "ontouchcancel", "ontouchend", "ontouchmove", "ontouchstart",
            "ontransitionend", "onunload", "onvolumechange", "onwaiting", "onwheel",

            //more events
            "onabort", "onanimationend", "onanimationiteration", "onanimationstart", "onauxclick",
            "onbeforecopy", "onbeforecut", "onbeforeinput", "onbeforematch", "onbeforepaste", "onbeforetoggle",
            "onbeforexrselect", "onblur", "oncancel", "oncanplay", "oncanplaythrough", "onchange", "onclick",
            "onclose", "oncontentvisibilityautostatechange", "oncontextlost", "oncontextmenu", "oncontextrestored",
            "oncopy", "oncuechange", "oncut", "ondblclick", "ondrag", "ondragend", "ondragenter", "ondragleave",
            "ondragover", "ondragstart", "ondrop", "ondurationchange", "onemptied", "onended", "onerror", "onfocus",
            "onformdata", "onfullscreenchange", "onfullscreenerror", "ongotpointercapture", "oninput", "oninvalid",
            "onkeydown", "onkeypress", "onkeyup", "onload", "onloadeddata", "onloadedmetadata", "onloadstart",
            "onlostpointercapture", "onmousedown", "onmouseenter", "onmouseleave", "onmousemove", "onmouseout",
            "onmouseover", "onmouseup", "onmousewheel", "onpaste", "onpause", "onplay", "onplaying",
            "onpointercancel", "onpointerdown", "onpointerenter", "onpointerleave", "onpointermove",
            "onpointerout", "onpointerover", "onpointerrawupdate", "onpointerup", "onprogress", "onratechange",
            "onreset", "onresize", "onscroll", "onscrollend", "onscrollsnapchange", "onscrollsnapchanging",
            "onsearch", "onsecuritypolicyviolation", "onseeked", "onseeking", "onselect", "onselectionchange",
            "onselectstart", "onslotchange", "onstalled", "onsubmit", "onsuspend", "ontimeupdate", "ontoggle",
            "ontransitioncancel", "ontransitionend", "ontransitionrun", "ontransitionstart", "onvolumechange",
            "onwaiting",

            //more attributes
            "autocapitalize","elementtiming", "nonce", "writingsuggestions"
            }
        ).ToImmutableHashSet(StringComparer.Ordinal);
    }
}
