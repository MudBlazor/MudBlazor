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

        public bool IsRendered { get; set; }

        public Type Type { get; set; }

        public PortalItem Clone()
        {
            return new PortalItem
            {
                Id = Id,
                IsRendered = IsRendered,
                AnchorRect = AnchorRect?.Clone(),
                FragmentRect = FragmentRect?.Clone(),
                CssPosition = CssPosition,
                Fragment = Fragment,
                Type = Type
            };
        }
    }

    public static class PortalItemExtensions
    {
        public static bool IsEqualTo(this PortalItem sourceItem, PortalItem targetItem)
        {
            return sourceItem.IsRendered == targetItem.IsRendered
                && sourceItem.AnchorRect.IsEqualTo(targetItem.AnchorRect)
                && sourceItem.FragmentRect.IsEqualTo(targetItem.FragmentRect);
        }
    }
}
