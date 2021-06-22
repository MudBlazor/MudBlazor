using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interop;

namespace MudBlazor.Services
{
    public class PortalItem
    {
        public Guid Id { get; set; }

        public RenderFragment Fragment { get; set; }

        public BoundingClientRect AnchorRect { get; set; }

        public BoundingClientRect FragmentRect { get; set; }

        public bool AutoDirection { get; set; }

        public string Position { get; set; } = "absolute";

        public int ViewportHeight { get; set; }

        public int ViewportWidth { get; set; }

        public Type PortalType { get; set; }

        public Placement Placement { get; set; }

        public Direction Direction { get; set; }

        public bool OffsetX { get; set; }

        public bool OffsetY { get; set; }

        public MudPortalItem Reference { get; set; }
    }
}
