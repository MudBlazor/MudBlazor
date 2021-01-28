using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudNavMenu : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-navmenu")
          .AddClass(Class)
        .Build();

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public EventCallback<MudNavLink> OnNavigation { get; set; }

        [CascadingParameter] MudNavMenu NavMenu { get; set; }

        public MudNavMenu()
        {
            NavMenu = this;
        }

        internal Task RaiseOnNavigation(MudNavLink navLink)
            => OnNavigation.InvokeAsync(navLink);
    }
}
