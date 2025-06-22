using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Docs.Pages.Mud.Project;

public class GitHubProfile
{
    public string login { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string bio { get; set; } = string.Empty;
    public string location { get; set; } = string.Empty;
    public string avatar_url { get; set; } = string.Empty;
}

public class GitHubProfileService
{
    private readonly HttpClient _http;
    private readonly NavigationManager _nav;
    public GitHubProfileService(HttpClient http, NavigationManager nav)
    {
        _http = http;
        _nav = nav;
    }

    public async Task<List<GitHubProfile>> LoadProfilesAsync(IEnumerable<string> usernames)
    {
        var profiles = new List<GitHubProfile>();
        foreach (var username in usernames)
        {
            try
            {
                var url = _nav.BaseUri + $"github-profiles/{username}.json";
                var profile = await _http.GetFromJsonAsync<GitHubProfile>(url);
                if (profile != null)
                    profiles.Add(profile);
            }
            catch { }
        }
        return profiles;
    }
}
