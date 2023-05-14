// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using MudBlazor.Extensions;

namespace MudBlazor
{
    /// <summary>
    /// Hacky mess below!!!
    /// </summary>
    public class GridCSSGenerator : IAsyncDisposable
    {
        private OrderedDictionary CSSRules = new OrderedDictionary(); //a list of the existing rules AND there indexes
        private List<CSSRule> Delta = new List<CSSRule>();
        private bool JSIsReady;
        private readonly string styleSheetId;
        private IJSRuntime JSruntime;

        public GridCSSGenerator(IJSRuntime jsRuntime, string styleSheetId)
        {
            JSruntime = jsRuntime;
            this.styleSheetId = styleSheetId;
        }

        /// <summary>
        /// Activate the CSS Generator with an active JSRuntime
        /// </summary>
        public ValueTask Ready()
        {
            if (JSIsReady) return ValueTask.CompletedTask;
            JSIsReady = true;
            return JSruntime.InvokeVoidAsync("window.createStylesheet", styleSheetId);
        }

        public void AddOrUpdateClass(string className, string value)
        {
            CSSRules.Add(className, value);
            Delta.Add(new CSSRule(CSSRules.IndexOf(className), $".{className} {{ {value} }}"));
        }

        public void RemoveClass(string className)
        {
            Delta.Add(new CSSRule(CSSRules.IndexOf(className), null));
            CSSRules.Remove(className);
        }

        public async ValueTask<bool> ApplyDelta()
        {
            if (!JSIsReady) return false;
            var result = await JSruntime.InvokeAsync<bool>("window.applyDelta", styleSheetId, Delta);
            Delta.Clear(); //clear existing delta elements
            return result;
        }

        //hacky method because I lack cell service to debug my delta code :(
        public async ValueTask<bool> Apply()
        {
            if (!JSIsReady) return false;
            var content = "";
            foreach (var element in CSSRules)
            {
                var rule = element.ToString().Replace("[", null).Replace("]", null);
                var parsed = rule.Split(',');
                content += $".{parsed[0]} {{ {parsed[1]} }}";
            }

            var result = await JSruntime.InvokeAsync<bool>("window.apply", styleSheetId, content);
            return result;
        }

        public ValueTask DisposeAsync()
        {
            if (JSIsReady)
                return JSruntime.InvokeVoidAsyncIgnoreErrors("window.removeStylesheet", styleSheetId);
            return ValueTask.CompletedTask;
        }
    }
}
