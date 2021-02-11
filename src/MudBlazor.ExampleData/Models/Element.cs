using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MudBlazor.ExampleData.Models
{
    public class Element
    {
        public string Group { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }

        [JsonPropertyName("small")]
        public string Sign { get; set; }
        public double Molar { get; set; }
        public IList<int> Electrons { get; set; }

        public override string ToString()
        {
            return $"{Sign} - {Name}";
        }
    }
}
