using System;
using System.Collections.Generic;
using System.Linq;

namespace MudBlazor.Docs.Models
{
    public class DocsComponents
    {
        private List<MudComponent> _mudComponents;

        public DocsComponents()
        {
            _mudComponents = new List<MudComponent>();
        }

        public DocsComponents AddItem(string name, Type component)
        {
            var componentItem = new MudComponent
            {
                Name = name,
                Link = name.ToLower().Replace(" ", ""),
                Component = component,
                IsNavGroup = false
            };

            _mudComponents.Add(componentItem);

            return this;
        }

        public DocsComponents AddNavGroup(string name, bool expanded, DocsComponents groupItems)
        {
            var componentItem = new MudComponent
            {
                Name = name,
                NavGroupExpanded = expanded,
                GroupItems = groupItems,
                IsNavGroup = true
            };

            _mudComponents.Add(componentItem);

            return this;
        }

        internal List<MudComponent> ToList() => _mudComponents;
    }
}
