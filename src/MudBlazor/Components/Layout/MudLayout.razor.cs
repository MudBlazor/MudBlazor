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

        public DrawerContainer DrawerContainer { get; private set; } = new DrawerContainer();

        public void FireDrawersChanged()
        {
            StateHasChanged();
        }
    }
}
