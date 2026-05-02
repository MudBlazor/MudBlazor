// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace MudBlazor.UnitTests.Analyzers.Verifiers;

public static class VerifierSettings
{
    // Add reference assemblies for .NET
    // Keep the version in sync with the MudBlazor project
    // TODO: Use standard .NET 10.0 reference assemblies when available
    // Copied from https://github.com/dotnet/roslyn-sdk/blob/f500e81ba5596809e711a3022bf8e80a00c8371b/src/Microsoft.CodeAnalysis.Testing/Microsoft.CodeAnalysis.Analyzer.Testing/ReferenceAssemblies.cs#L1216
    public static readonly ReferenceAssemblies DefaultReferenceAssemblies =
        new("net10.0",
            new PackageIdentity("Microsoft.NETCore.App.Ref", "10.0.0-rc.1.25451.107"),
            Path.Combine("ref", "net10.0"));

    public static readonly MetadataReference ComponentBaseReference =
        MetadataReference.CreateFromFile(typeof(Microsoft.AspNetCore.Components.ComponentBase).Assembly.Location);

    public static readonly MetadataReference MudBlazorReference =
        MetadataReference.CreateFromFile(typeof(MudBlazor._Imports).Assembly.Location);
}
