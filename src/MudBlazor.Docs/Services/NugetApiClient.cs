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
    public class NugetApiClient : IDisposable
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public NugetApiClient()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri("https://azuresearch-usnc.nuget.org/")
            };
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                TypeInfoResolver = JsonTypeInfoResolver.Combine(NugetApiJsonSerializerContext.Default)
            };
        }

        public async Task<NugetPackage?> GetPackageAsync(string packageName)
        {
            try
            {
                var result = await _http.GetFromJsonAsync<NugetResponse>($"query?q=packageid:{packageName}&take=1", _jsonSerializerOptions);
                return result?.Data.FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public void Dispose()
        {
            _http.Dispose();
        }
    }
}
