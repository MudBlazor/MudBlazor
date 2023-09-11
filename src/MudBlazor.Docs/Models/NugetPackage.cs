// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MudBlazor.Docs.Models;

public class NugetResponse
{
    [JsonPropertyName("totalHits")]
    public int TotalHits { get; set; }

    [JsonPropertyName("data")]
    public List<NugetPackage> Data { get; set; }
}
public class NugetPackage
{
    [JsonPropertyName("@type")]
    public string Type { get; set; }

    [JsonPropertyName("registration")]
    public string Registration { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("summary")]
    public string Summary { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("iconUrl")]
    public string IconUrl { get; set; }

    [JsonPropertyName("licenseUrl")]
    public string LicenseUrl { get; set; }

    [JsonPropertyName("projectUrl")]
    public string ProjectUrl { get; set; }

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; }

    [JsonPropertyName("authors")]
    public List<string> Authors { get; set; }

    [JsonPropertyName("owners")]
    public List<string> Owners { get; set; }

    [JsonPropertyName("totalDownloads")]
    public int TotalDownloads { get; set; }

    [JsonPropertyName("verified")]
    public bool Verified { get; set; }

    [JsonPropertyName("packageTypes")]
    public List<PackageType> PackageTypes { get; set; }

    [JsonPropertyName("versions")]
    public List<Versions> Versions { get; set; }
}

public class Versions
{
    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("downloads")]
    public int Downloads { get; set; }

    [JsonPropertyName("@id")]
    public string Id { get; set; }
}

public class PackageType
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
