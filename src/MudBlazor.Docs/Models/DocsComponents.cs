using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                Component = component
            };

            _mudComponents.Add(componentItem);

            return this;
        }

        internal List<MudComponent> ToList()
        {
            var Item = _mudComponents.ToList();

            return Item.ToList();
        }
    }
}
