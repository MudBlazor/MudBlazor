using System;
using System.Reflection;

namespace MudBlazor.Docs.Models
{
    public class ApiProperty
    {
        public PropertyInfo PropertyInfo { get; set; }

        public string Name { get; set; }
        public Type Type { get; set; }
        public bool IsTwoWay { get; set; }
        public object Default { get; set; }
        public string Description { get; set; }
    }
}
