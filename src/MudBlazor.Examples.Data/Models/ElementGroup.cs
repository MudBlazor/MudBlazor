using System.Collections.Generic;

namespace MudBlazor.Examples.Data.Models
{
    public class ElementGroup
    {
        public string Wiki { get; set; }
        public IList<Element> Elements { get; set; }
    }
}