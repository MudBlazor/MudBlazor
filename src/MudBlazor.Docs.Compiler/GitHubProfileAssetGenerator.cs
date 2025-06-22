using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MudBlazor.Docs.Compiler;

public class GitHubProfileAssetGenerator
{
    private static readonly string[] CoreTeam = new[]
    {
        "tungi52",
        "mikes-gh",
        "just-the-benno",
        "JonBunator",
        "Mr-Technician",
        "ScarletKuro",
        "danielchalmers"
    };

    private static readonly string[] Contributors = new[]
    {
        "HClausing",
        "porkopek",
        "mckaragoz",
        "jperson2000",
        "Flaflo",
        "igotinfected",
        "meenzen",
        "xC0dex",
        "ralvarezing",
        "versile2",
        "anu6is"
    };

    private const string OutputDir = "../MudBlazor.Docs/wwwroot/github-profiles";

    public bool Execute()
    {
        try
        {
            Directory.CreateDirectory(OutputDir);
            var allUsers = new List<string>();
            allUsers.AddRange(CoreTeam);
            allUsers.AddRange(Contributors);
            foreach (var username in allUsers)
            {
                var info = FetchGitHubInfo(username).GetAwaiter().GetResult();
                if (info != null)
                {
                    var json = JsonSerializer.Serialize(new
                    {
                        login = info.login,
                        name = info.name,
                        bio = info.bio,
                        location = info.location,
                        avatar_url = info.avatar_url
                    }, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(Path.Combine(OutputDir, $"{username}.json"), json);
                }
            }
            Console.WriteLine($"GitHub profile assets generated for {allUsers.Count} users.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating GitHub profile assets: {ex.Message}");
            return false;
        }
    }

    private async Task<GitHubUser?> FetchGitHubInfo(string username)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("MudBlazor.Docs.Compiler");
        var response = await client.GetAsync($"https://api.github.com/users/{username}");
        if (!response.IsSuccessStatusCode)
            return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GitHubUser>(json);
    }

    private class GitHubUser
    {
        public string login { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string bio { get; set; } = string.Empty;
        public string location { get; set; } = string.Empty;
        public string avatar_url { get; set; } = string.Empty;
    }
}
