// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;

namespace MudBlazor.Docs.Models.Context
{
    [JsonSerializable(typeof(GithubContributors[]))]
    [JsonSerializable(typeof(GitHubReleases[]))]
    [JsonSerializable(typeof(GitHubRepository))]
    public sealed partial class GithubApiJsonSerializerContext : JsonSerializerContext
    {
    }
}
