using System.Text.Json;
using System.Text.Json.Serialization;

namespace MudBlazor.Docs.Data
{
    public class Element
    {
        [JsonPropertyName("group")]
        public string Group { get; set; }

        [JsonPropertyName("position")]
        public int Position { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("small")]
        public string Sign { get; set; }

        [JsonPropertyName("molar")]
        public double Molar { get; set; }

        public override string ToString()
        {
            return $"{Sign} - {Name}";
        }
    }
}
