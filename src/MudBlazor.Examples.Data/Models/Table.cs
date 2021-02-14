using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MudBlazor.Examples.Data.Models
{
    public class Table
    {
        [JsonPropertyName("table")]
        public IList<ElementGroup> ElementGroups { get; set; }
    }
}