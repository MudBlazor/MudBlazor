using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudLayout : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-layout")
            .AddClass("mud-application-layout-rtl", RightToLeft)
            .AddClass(Class)
            .Build();

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        private bool _rtl;

        /// <summary>
        /// If set, changes the layout to RightToLeft.
        /// </summary>
        [Parameter]
        public bool RightToLeft
        {
            get => _rtl;
            set
            {
                if (_rtl != value)
                {
                    _rtl = value;
                    StateHasChanged();
                }
            }
        }

        List<MudDrawer> _drawers = new List<MudDrawer>();

        public void Add(MudDrawer drawer)
        {
            _drawers.Add(drawer);
        }

        public void Remove(MudDrawer drawer)
        {
            _drawers.Remove(drawer);
        }

        public bool? IsDrawerOpen(Anchor anchor)
        {
            return _drawers.FirstOrDefault(d => d.Anchor == anchor)?.Open;
        }

        public bool? IsDrawerClipped(Anchor anchor)
        {
            return _drawers.FirstOrDefault(d => d.Anchor == anchor)?.Clipped;
        }

        public void FireDrawersChanged()
        {
            StateHasChanged();
        }

        public bool HasDrawer(Anchor anchor)
        {
            return _drawers.Any(d => d.Anchor == anchor);
        }
    }
}
