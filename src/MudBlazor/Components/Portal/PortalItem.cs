using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interop;

namespace MudBlazor.Services
{
    public class PortalItem
    {
        public Guid Id { get; set; }

        public RenderFragment Fragment { get; set; }

        public BoundingClientRect ClientRect { get; set; }

        public bool AutoDirection { get; set; }

        public string Position { get; set; } = "absolute";

    }
}
