// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
    public class DiscordApiClient : IDisposable
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public DiscordApiClient()
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri("https://discord.com/")
            };
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                TypeInfoResolver = JsonTypeInfoResolver.Combine(DiscordApiJsonSerializerContext.Default)
            };
        }

        public async Task<DiscordInvite?> GetDiscordInviteAsync()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<DiscordInvite>("api/v10/invites/mudblazor?with_counts=true", _jsonSerializerOptions);
                return result;
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
