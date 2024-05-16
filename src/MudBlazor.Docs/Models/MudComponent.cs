using System;
using System.Collections.Generic;

namespace MudBlazor.Docs.Models
{
    public class MudComponent
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public bool IsNavGroup { get; set; }
        public bool NavGroupExpanded { get; set; }

        /// <summary>
        /// A bunch of components that are grouped in the nav menu
        /// </summary>
        public List<MudComponent> GroupComponents { get; set; }

        Type _type;
        public Type Type
        {
            get => _type;
            set
            {
                _type = value;
                ComponentName = Type.Name.Replace("`1", "<T>");
            }
        }
        public Type[] ChildTypes { get; set; }

        public string ComponentName { get; private set; }
    }
}
