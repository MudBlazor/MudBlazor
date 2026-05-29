// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json.Serialization;

namespace MudBlazor.Docs.Models;

public class GitHubRepository
{
    [JsonPropertyName("stargazers_count")]
    public int StargazersCount { get; set; }
}
