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

        public const string Company = @"#nullable enable

using System.Collections.Generic;

public class Company
{
    public string? Department { get; set; }

    public string? Team { get; set; }

    public string? Role { get; set; }

    public string? Name { get; set; }

    public int Score { get; set; }

    public static IReadOnlyCollection<Company> GetRecords() =>
    [
        new() { Department = ""Engineering"", Team = ""Platform"", Role = ""Backend"", Name = ""Alice"", Score = 120 },
        new() { Department = ""Engineering"", Team = ""Platform"", Role = ""Backend"", Name = ""Bob"", Score = 115 },
        new() { Department = ""Engineering"", Team = ""Platform"", Role = ""Frontend"", Name = ""Carol"", Score = 110 },
        new() { Department = ""Engineering"", Team = ""Platform"", Role = ""Frontend"", Name = ""Dave"", Score = 108 },
        new() { Department = ""Engineering"", Team = ""Data"", Role = ""Analytics"", Name = ""Eve"", Score = 105 },
        new() { Department = ""Engineering"", Team = ""Data"", Role = ""Analytics"", Name = ""Frank"", Score = 102 },
        new() { Department = ""Sales"", Team = ""Enterprise"", Role = ""Account Exec"", Name = ""Grace"", Score = 150 },
        new() { Department = ""Sales"", Team = ""Enterprise"", Role = ""Account Exec"", Name = ""Henry"", Score = 145 },
        new() { Department = ""Sales"", Team = ""SMB"", Role = ""Account Exec"", Name = ""Ivy"", Score = 130 },
        new() { Department = ""Sales"", Team = ""SMB"", Role = ""Sales Dev"", Name = ""Jack"", Score = 125 },
        new() { Department = ""Support"", Team = ""Tier1"", Role = ""Agent"", Name = ""Kelly"", Score = 95 },
        new() { Department = ""Support"", Team = ""Tier2"", Role = ""Agent"", Name = ""Liam"", Score = 100 }
    ];
}";
    }
}
