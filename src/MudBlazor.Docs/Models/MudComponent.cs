﻿using System;
using System.Collections.Generic;

namespace MudBlazor.Docs.Models
{
    public class MudComponent
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public bool IsNavGroup { get; set; }
        public bool NavGroupExpanded { get; set; }
        public DocsComponents GroupItems { get; set; }
        public Type Component { get; set; }
        public List<Type> ChildComponents { get; set; }

        public string ComponentName
        {
            get
            {
                return Component.Name.Replace("`1", "<T>");
            }
        }
    }
}
