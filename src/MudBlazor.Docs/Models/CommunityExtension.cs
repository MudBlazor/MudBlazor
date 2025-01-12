// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Text.Json.Serialization;

namespace MudBlazor.Docs.Models;

[DebuggerDisplay($"Name = {nameof(Name)}")]
public class CommunityExtension
{
    [JsonPropertyName("AvatarImageSrc")]
    public string AvatarImageSrc { get; set; } = @"https://mudblazor.com/_content/MudBlazor.Docs/images/logo.png";

    [JsonPropertyName("Category")]
    public string Category { get; set; } = "Parent";

    [JsonPropertyName("Name")]
    public string Name { get; set; } = "MudBlazor";

    [JsonPropertyName("Description")]
    public string Description { get; set; } = "With millions of downloads, from hobby developers to large enterprises, MudBlazor enables you to rapidly build amazing web applications without leaving your beloved C# language and toolchain. ";

    [JsonPropertyName("Link")]
    public string Link { get; set; } = @"https://mudblazor.com";

    [JsonPropertyName("GitHubUserPath")]
    public string GitHubUserPath { get; set; } = "MudBlazor";

    [JsonPropertyName("GitHubRepoPath")]
    public string GitHubRepoPath { get; set; } = "MudBlazor";

    [JsonIgnore]
    public string GitHubLink => @$"https://github.com/{GitHubUserPath}/{GitHubRepoPath}";
}
