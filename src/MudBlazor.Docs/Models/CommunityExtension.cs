// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Docs.Models;

public class CommunityExtension
{
    public string AvatarImageSrc { get; set; } = @"https://mudblazor.com/_content/MudBlazor.Docs/images/logo.png";

    public string Category { get; set; } = "Parent";

    public string Name { get; set; } = "MudBlazor";

    public string Description { get; set; } = "With millions of downloads, from hobby developers to large enterprises, MudBlazor enables you to rapidly build amazing web applications without leaving your beloved C# language and toolchain. ";

    public string Link { get; set; } = @"https://mudblazor.com";

    public string GitHubLink => @$"https://github.com/{GitHubUserPath}/{GitHubRepoPath}";

    public string GitHubUserPath { get; set; } = "MudBlazor";

    public string GitHubRepoPath { get; set; } = "MudBlazor";
}
