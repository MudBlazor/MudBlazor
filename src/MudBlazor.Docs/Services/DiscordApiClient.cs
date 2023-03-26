// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Services
{
    public class DiscordApiClient
    {
        private readonly HttpClient _http;

        public DiscordApiClient(HttpClient http)
        {
            _http = http;
        }
        public async Task<DiscordInvite> GetDiscordInviteAsync()
        {
            try
            {
                var result = await _http.GetFromJsonAsync<DiscordInvite>($"https://discord.com/api/v10/invites/mudblazor?with_counts=true");
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
