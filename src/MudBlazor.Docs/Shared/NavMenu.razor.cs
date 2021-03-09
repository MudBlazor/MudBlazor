﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Models;
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
            _section = NavMan.GetSection();
            _componentLink = NavMan.GetComponentLink();
            StateHasChanged();
        }

        bool IsSubGroupExpanded(MudComponent item)
        {
            #region comment about is subgroup expanded
            //if the route contains any of the links of the subgroup, then the subgroup
            //should be expanded
            //Example:
            //subgroup: form inputs & controls
            //the subgroup "form inputs & controls" should be open if the current page has in the route
            //a component included in the subgroup elements, that in this case are autocomplete, form, field,
            //radio, select...
            //this route `/components/autocomplete` should open the subgroup "form inputs..."
            #endregion
            return item.GroupItems.Elements.Any(i => i.Link == _componentLink);
        }
    }
}
