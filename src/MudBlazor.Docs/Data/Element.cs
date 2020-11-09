using Newtonsoft.Json;

namespace MudBlazor.Docs.Data
{
    public class Element
    {
        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("position")]
        public int Position { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("small")]
        public string Sign { get; set; }

        [JsonProperty("molar")]
        public double Molar { get; set; }

        public override string ToString()
        {
            return $"{Sign} - {Name}";
        }
    }
}
