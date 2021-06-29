// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Interop;

namespace MudBlazor.Docs.Models
{
    public class DocsSectionLink
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public bool Active { get; set; }
        public ElementReference Reference { get; set; }
        public BoundingClientRect Location { get; set; }
    }
}
