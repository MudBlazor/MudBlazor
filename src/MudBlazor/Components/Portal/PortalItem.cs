using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interop;

namespace MudBlazor.Services
{
    public class PortalItem
    {
        public Guid Id { get; set; }

        public RenderFragment Fragment { get; set; }

        public BoundingClientRect AnchorRect { get; set; } = new();

        public BoundingClientRect FragmentRect { get; set; } = new();

        public string CssPosition { get; set; } = "absolute";

        public bool IsVisible { get; set; }

        public PortalItemJsModel JavaScriptModel =>
            new()
            {
                Id = Id,
                IsVisible = IsVisible,
                CssPosition = CssPosition
            };


        public PortalItem Clone()
        {
            return new PortalItem
            {
                Id = Id,
                IsVisible = IsVisible,
                AnchorRect = AnchorRect?.Clone(),
                FragmentRect = FragmentRect?.Clone(),
                CssPosition = CssPosition,
                Fragment = Fragment,
            };
        }
    }



    public class PortalItemJsModel
    {
        public Guid Id { get; set; }
        public bool IsVisible { get; set; }
        public string CssPosition { get; set; }
    }
}
