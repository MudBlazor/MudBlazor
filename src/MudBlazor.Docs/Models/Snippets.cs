using System.Linq;
using System.Reflection;

namespace MudBlazor.Docs.Models
{
    // this is needed for the copy-to-clipboard feature
    public static partial class Snippets
    {
        public static string GetCode(string component)
        {
            var field = typeof(Snippets).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField)
                .FirstOrDefault(f => f.Name == component);
            if (field == null)
                return null;
            return (string)field.GetValue(null);
        }

        // used for webapi examples
        public const string Element = @"using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MudBlazor.Examples.Data.Models
{
    public class Element
    {
        public string Group { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }

        [JsonPropertyName(""small"")]
        public string Sign { get; set; }
        public double Molar { get; set; }
        public IList<int> Electrons { get; set; }

        public override string ToString()
        {
            return $""{Sign} - {Name}"";
        }
    }
}";

        public const string Server = @"using System;

namespace MudBlazor.Examples.Data.Models
{
    public class Server
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string IpAddress { get; set; }
    }
}";
    }
}
