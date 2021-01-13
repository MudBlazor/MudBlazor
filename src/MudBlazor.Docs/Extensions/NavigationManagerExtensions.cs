using Microsoft.AspNetCore.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Docs.Extensions
{
    internal static class NavigationManagerExtensions
    {
        /// <summary>
        /// Gets the section part of the documentation page
        /// Ex: /components/button;  "components" is the section
        /// </summary>
        public static string GetSection(this NavigationManager navMan)
        {
            var currentUri = new Uri(navMan.Uri);
            return currentUri.AbsolutePath
                .Split("/", StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault();
        }
        /// <summary>
        /// Gets the link of the component on the documentation page
        /// Ex: /api/button; "button" is the component link, and "api" is the section
        /// </summary>
        public static string GetComponentLink(this NavigationManager navMan)
        {
            var currentUri = new Uri(navMan.Uri);
            return currentUri.AbsolutePath
                .Split("/", StringSplitOptions.RemoveEmptyEntries)
                //the second element
                .ElementAtOrDefault(1);
        }

        /// <summary>
        /// Determines if the current page is the base page
        /// </summary>
        public static bool IsHomePage(this NavigationManager navMan)
        {
            return navMan.Uri == navMan.BaseUri;
        }
    }
}
