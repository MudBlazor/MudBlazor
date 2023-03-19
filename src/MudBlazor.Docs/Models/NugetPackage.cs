// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MudBlazor.Docs.Models;

public class NugetRespons
{
    public int totalHits { get; set; }
    public List<NugetPackage> data { get; set; }
}
public class NugetPackage
{
    [JsonPropertyName("@id")]
    public string Id { get; set; }

    [JsonPropertyName("@type")]
    public string type { get; set; }
    public string registration { get; set; }
    public string id { get; set; }
    public string version { get; set; }
    public string description { get; set; }
    public string summary { get; set; }
    public string title { get; set; }
    public string iconUrl { get; set; }
    public string licenseUrl { get; set; }
    public string projectUrl { get; set; }
    public List<string> tags { get; set; }
    public List<string> authors { get; set; }
    public List<string> owners { get; set; }
    public int totalDownloads { get; set; }
    public bool verified { get; set; }
    public List<PackageType> packageTypes { get; set; }
    public List<Version> versions { get; set; }

    public class Version
    {
        public string version { get; set; }
        public int downloads { get; set; }

        [JsonPropertyName("@id")]
        public string id { get; set; }
    }

    public class PackageType
    {
        public string name { get; set; }
    }
}

