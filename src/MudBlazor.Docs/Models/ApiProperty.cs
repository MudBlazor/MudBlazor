using System;
using System.Collections.Generic;
using System.Text;

namespace MudBlazor.Docs.Models
{
    public class ApiProperty
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public object Default { get; set; }
    }
}
