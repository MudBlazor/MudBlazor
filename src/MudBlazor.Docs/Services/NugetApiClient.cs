// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Services
{
    public class NugetApiClient
    {
        private readonly HttpClient _http;

        public NugetApiClient(HttpClient http)
        {
            _http = http;
            http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.106 Mobile Safari/537.36");
        }
        public async Task<NugetPackage> GetPackageAsync(string packageName)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<NugetRespons>($"https://azuresearch-usnc.nuget.org/query?q=packageid:{packageName}&take=1");
                return result.data.FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
