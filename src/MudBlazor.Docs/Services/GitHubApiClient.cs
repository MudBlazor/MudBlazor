// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using MudBlazor.Docs.Models;
using MudBlazor.Docs.Models.Context;

namespace MudBlazor.Docs.Services
{
#nullable enable
    public class GitHubApiClient : IDisposable
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public GitHubApiClient()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri("https://api.github.com:443/")
            };
            _http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.106 Mobile Safari/537.36");
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                TypeInfoResolver = JsonTypeInfoResolver.Combine(GithubApiJsonSerializerContext.Default)
            };
        }

        public async Task<GithubContributors[]> GetContributorsAsync()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<GithubContributors[]>("repos/MudBlazor/MudBlazor/contributors?per_page=100", _jsonSerializerOptions);
                return result ?? Array.Empty<GithubContributors>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Array.Empty<GithubContributors>();
            }
        }

        public async Task<GitHubReleases[]> GetReleasesAsync()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<GitHubReleases[]>("repos/MudBlazor/MudBlazor/releases?per_page=100", _jsonSerializerOptions);
                return result ?? Array.Empty<GitHubReleases>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Array.Empty<GitHubReleases>();
            }
        }

        public async Task<GitHubRepository?> GetRepositoryAsync(string owner, string repo)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<GitHubRepository>($"repos/{owner}/{repo}", _jsonSerializerOptions);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<int> GetContributorsCountAsync(string owner, string repo)
        {
            try
            {
                var result = await _http.GetAsync($"repos/{owner}/{repo}/contributors?per_page=1&anon=true");
                var value = result.Headers.GetValues("Link").FirstOrDefault();
                value = value?.Substring(value.LastIndexOf("page=", StringComparison.Ordinal) + 5);
                value = value?.Substring(0, value.LastIndexOf(">;", StringComparison.Ordinal));

                return value is not null ? int.Parse(value) : 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        public void Dispose()
        {
            _http.Dispose();
        }
    }
}
