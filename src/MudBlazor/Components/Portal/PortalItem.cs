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

        public string CssPosition { get; set; } = "absolute";

        public Type Type { get; set; }

        public bool OpenOnHover { get; set; }
    }
}
