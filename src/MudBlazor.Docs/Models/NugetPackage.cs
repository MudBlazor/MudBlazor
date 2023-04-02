// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MudBlazor.Docs.Models;

public class NugetRespons
{
    public int TotalHits { get; set; }
    public List<NugetPackage> Data { get; set; }
}
public class NugetPackage
{
    [JsonPropertyName("@type")]
    public string Type { get; set; }
    public string Registration { get; set; }
    public string Id { get; set; }
    public string Version { get; set; }
    public string Description { get; set; }
    public string Summary { get; set; }
    public string Title { get; set; }
    public string IconUrl { get; set; }
    public string LicenseUrl { get; set; }
    public string ProjectUrl { get; set; }
    public List<string> Tags { get; set; }
    public List<string> Authors { get; set; }
    public List<string> Owners { get; set; }
    public int TotalDownloads { get; set; }
    public bool Verified { get; set; }
    public List<PackageType> PackageTypes { get; set; }
    public List<Versions> Versions { get; set; }
}

public class Versions
{
    public string Version { get; set; }
    public int Downloads { get; set; }

    [JsonPropertyName("@id")]
    public string Id { get; set; }
}

public class PackageType
{
    public string Name { get; set; }
}
