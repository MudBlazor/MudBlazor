using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MudBlazor.Docs.Compiler;

public class GitHubInfoFetcher
{
    private const string GitHubApiUrl = "https://api.github.com/users/danielchalmers";

    public bool Execute()
    {
        try
        {
            var info = FetchGitHubInfo().GetAwaiter().GetResult();
            if (info != null)
            {
                Console.WriteLine($"GitHub User: {info.login}\nName: {info.name}\nPublic Repos: {info.public_repos}\nFollowers: {info.followers}\nFollowing: {info.following}");
            }
            else
            {
                Console.WriteLine("Failed to fetch GitHub info.");
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching GitHub info: {ex.Message}");
            return false;
        }
    }

    private async Task<GitHubUser?> FetchGitHubInfo()
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("MudBlazor.Docs.Compiler");
        var response = await client.GetAsync(GitHubApiUrl);
        if (!response.IsSuccessStatusCode)
            return null;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GitHubUser>(json);
    }

    private class GitHubUser
    {
        public string login { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public int public_repos { get; set; }
        public int followers { get; set; }
        public int following { get; set; }
    }
}
