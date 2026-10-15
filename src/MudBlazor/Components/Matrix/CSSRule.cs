// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor
{
    public struct CSSRule
    {
        public CSSRule(int index, string rule)
        {
            Index = index;
            Rule = rule;
        }
        public int Index { get; }
        public string Rule { get; }
    }
}
