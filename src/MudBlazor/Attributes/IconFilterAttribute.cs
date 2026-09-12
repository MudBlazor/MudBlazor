// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class IconFilterAttribute : Attribute
    {
        public IconFilterAttribute(string? categories, string? tags)
        {
            Categories = categories;
            Tags = tags;
        }

        public string? Categories { get; }
        public string? Tags { get; }
    }
}
