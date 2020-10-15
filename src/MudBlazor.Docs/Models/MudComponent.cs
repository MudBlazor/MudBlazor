using System;
using System.Collections.Generic;
using System.Text;

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

    }
}
