using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudLayout : MudDrawerContainer
    {
        protected override string Classname =>
        new CssBuilder("mud-layout")
            .AddClass("mud-application-layout-rtl", RightToLeft)
            .AddClass(base.Classname)
            .Build();

        private bool _rtl;

        public MudLayout()
        {
            Fixed = true;
        }

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
    }
}
