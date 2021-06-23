// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Extensions
{
    public class GitHubApiClient
    {
        private readonly HttpClient http;

        public GitHubApiClient(HttpClient http)
        {
            this.http = http;
        }

        public async Task<GithubContributors[]> GetContributorsAsync()
        {
            try
            {
                var result = await http.GetFromJsonAsync<GithubContributors[]>("repos/Garderoben/MudBlazor/contributors");
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new GithubContributors[0];
            }
        }
    }
}
