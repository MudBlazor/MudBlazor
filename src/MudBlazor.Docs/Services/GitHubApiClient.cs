// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Services
{
    public class GitHubApiClient
    {
        private readonly HttpClient _http;

        public GitHubApiClient(HttpClient http)
        {
            _http = http;
            http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.106 Mobile Safari/537.36");
        }

        public async Task<GithubContributors[]> GetContributorsAsync()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<GithubContributors[]>("https://api.github.com:443/repos/MudBlazor/MudBlazor/contributors?per_page=100");
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new GithubContributors[0];
            }
        }
        
        public async Task<GitHubReleases[]> GetReleasesAsync()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<GitHubReleases[]>("https://api.github.com:443/repos/MudBlazor/MudBlazor/releases?per_page=100");
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new GitHubReleases[0];
            }
        }
    }
}
