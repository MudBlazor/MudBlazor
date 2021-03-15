using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudDrawerContainer : MudComponentBase
    {
        protected bool Fixed { get; set; } = false;
        private List<MudDrawer> _drawers = new List<MudDrawer>();

        protected virtual string Classname =>
        new CssBuilder()
            .AddClass(GetDrawerClass(Anchor.Left))
            .AddClass(GetDrawerClass(Anchor.Right))
            .AddClass(Class)
        .Build();

        protected string Stylename =>
        new StyleBuilder()
            .AddStyle("--mud-drawer-width-left", GetDrawerWidth(Anchor.Left), !string.IsNullOrEmpty(GetDrawerWidth(Anchor.Left)))
            .AddStyle("--mud-drawer-width-right", GetDrawerWidth(Anchor.Right), !string.IsNullOrEmpty(GetDrawerWidth(Anchor.Right)))
            .AddStyle(Style)
        .Build();

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        internal void FireDrawersChanged() => StateHasChanged();

        internal void Add(MudDrawer drawer)
        {
            if (Fixed && !drawer.Fixed)
                return;

            _drawers.Add(drawer);
            StateHasChanged();
        }

        internal void Remove(MudDrawer drawer) => _drawers.Remove(drawer);

        private string GetDrawerClass(Anchor anchor)
        {
            var drawer = _drawers.FirstOrDefault(d => d.Open && d.Anchor == anchor);
            if (drawer == null)
                return string.Empty;

            var className = $"mud-drawer-open-{drawer.Variant.ToDescriptionString()}";
            if (drawer.Variant == DrawerVariant.Responsive)
            {
                className += $"-{drawer.Breakpoint.ToDescriptionString()}";
            }
            className += $"-{anchor.ToDescriptionString()}";

            className += $" mud-drawer-{anchor.ToDescriptionString()}-clipped-{drawer.ClipMode.ToDescriptionString()}";

            return className;
        }

        private string GetDrawerWidth(Anchor anchor)
        {
            var drawer = _drawers.FirstOrDefault(d => d.Open && d.Anchor == anchor);
            if (drawer == null)
                return string.Empty;

            return drawer.Width;
        }
    }
}
