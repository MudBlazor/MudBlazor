
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Charts.SVG.Models
{
    public class SvgLegend
    {
        public int Index { get; set; }
        public string Labels { get; set; }
        public string Data { get; set; }
        public bool IsVisible { get; set; } = true;
        public EventCallback<SvgLegend> OnVisibilityChanged { get; set; }

        public async Task HandleCheckboxChangeAsync()
        {
            IsVisible = !IsVisible;
            await OnVisibilityChanged.InvokeAsync(this);
        }
    }
}
