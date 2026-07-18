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

        //component links are the part of the url that tells us what component is featured
        string _componentLink;

        protected override void OnInitialized()
        {
            Refresh();
            base.OnInitialized();
        }

        public void Refresh()
        {
            var section = NavMan.GetSection();
            var componentLink = NavMan.GetComponentLink();

            // Refresh() is called from DocsLayout.OnAfterRender on every render. The menu only depends on the
            // active section (which group is expanded), so skip re-rendering its ~250 links when nothing changed.
            if (section == _section && componentLink == _componentLink)
            {
                return;
            }

            _section = section;
            _componentLink = componentLink;
            StateHasChanged();
        }
    }
}
