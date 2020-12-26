using Microsoft.AspNetCore.Components;

using System.Collections.Generic;
using System.Linq;

using MudBlazor.Docs.Extensions;
using MudBlazor.Extensions;


namespace MudBlazor.Docs.Models
{
    public interface IDocsNavigationService
    {
        string Next { get; }
        string Previous { get; }
        NavigationSection? Section { get; }
    }

    /// <summary>
    /// The aim of this class is to provide the next and previous links for navigation footer
    /// </summary>
    public class DocsNavigationService : IDocsNavigationService
    {
        private readonly NavigationManager _navigationManager;
        private readonly IMenuService _menuService;

        //the last part of the url of the component;
        //often is the name of the component, like in /components/button/
        private string CurrentLink => _navigationManager.GetComponentLink();

        /// <summary>
        /// Previous link in the menu
        /// </summary>
        public string Previous=> GetNavigationLink( NavigationOrder.Previous);       
       
        /// <summary>
        /// Next link in the menu
        /// </summary>
        public string Next=>GetNavigationLink( NavigationOrder.Next );          

        
        /// TODO add "get-started" and remaining sections
        /// <summary>
        /// The section of the menu: components or api
        /// </summary>
        public NavigationSection? Section
        {
            get
            {
                // return the enum corresponding to the section
                return _navigationManager.GetSection() switch
                {
                    "components"=> NavigationSection.Components,
                    "api" => NavigationSection.Api,
                    _ => null,
                };
            }
        }

        //constructor for DI
        public DocsNavigationService(NavigationManager navigationManager, IMenuService menuService)
        {
            _navigationManager = navigationManager;
            _menuService = menuService;
           

        }

        /// <summary>
        /// Get the link (next or previous) for a given url and a given section
        /// </summary>
        /// <param name="order">next or previous</param>
        /// <returns></returns>
        private string GetNavigationLink( NavigationOrder order)
        {          
            List<string> orderedLinks =
                Section == NavigationSection.Api
                    ? GetOrderedMenuLinks( NavigationSection.Api)
                    : GetOrderedMenuLinks( NavigationSection.Components);         

            var index = orderedLinks.FindIndex(e => e == CurrentLink);
            int increment =
                order == NavigationOrder.Next
                    ? 1
                    : -1;

            var position = index + increment;
            //if out of index
            if (position< 0 || position>= orderedLinks.Count)
            {
                return string.Empty;
            }

            var navigationLink = orderedLinks.ElementAt(position);
            return navigationLink;
        }

        /// <summary>
        /// Gets the last part of links from the menu in the same order as in the menu
        /// This is the part with the name of the component
        /// </summary>
        /// <param name="section"> components or api </param>
        /// <returns></returns>
        private List<string> GetOrderedMenuLinks( NavigationSection? section)
        {
            var menuElements = 
                section==NavigationSection.Components
                    ? _menuService.DocsComponents.Elements
                    : _menuService.DocsComponentsApi.Elements;

            var links = new List<string>();
            foreach (var menuElement in menuElements)
            {
                if (menuElement.Link != null)
                {
                    var link =
                        section ==
                        NavigationSection.Api
                            ? ApiLink.GetApiLinkFor(menuElement.Component).Split("/").Last()
                            : ApiLink.GetComponentLinkFor(menuElement.Component).Split("/").Last();
                    links.Add(link);
                }
                if (menuElement.GroupItems != null)
                    links.AddRange(menuElement.GroupItems.Elements.Select(i => i.Link).OrderBy(i => i));
            };
            return links;
        }
    }

    #region ENUMS

    public enum NavigationOrder { Previous, Next }

    public enum NavigationSection { Api, Components }
    #endregion
}
