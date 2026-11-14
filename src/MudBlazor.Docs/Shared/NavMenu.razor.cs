using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Services;

namespace MudBlazor.Docs.Shared
{
    public partial class NavMenu
    {
        [Inject] IMenuService MenuService { get; set; }
        [Inject] NavigationManager NavMan { get; set; }

        //sections are "getting-started","components", "api", ...
        string _section;

        protected override void OnInitialized()
        {
            Refresh();
            base.OnInitialized();
        }

        public void Refresh()
        {
            var section = NavMan.GetSection();

            // The menu only depends on the active section (which group is expanded), so skip re-rendering when it did not change.
            // The links highlight the current page on their own, so moving between pages of one section needs no menu render.
            if (section == _section)
            {
                return;
            }

            _section = section;
            StateHasChanged();
        }
    }
}
