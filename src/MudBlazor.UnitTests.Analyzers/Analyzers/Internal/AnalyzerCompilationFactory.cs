using System.Collections.Immutable;
using System.Text;
using AwesomeAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace MudBlazor.UnitTests.Analyzers.Internal;

extern alias MudBlazorAnalyzer;

#nullable enable
internal static class AnalyzerCompilationFactory
{
    private static readonly ImmutableArray<MetadataReference> _metadataReferences = CreateMetadataReferences();

    internal static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(
        string source,
        MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern allowedAttributePattern,
        string customAllowedAttributes = "",
        string sourcePath = "AttributeTest.razor.g.cs")
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(SourceText.From(source, Encoding.UTF8), path: sourcePath);
        var compilation = CSharpCompilation.Create(
            assemblyName: "MudBlazor.UnitTests.Analyzers.Generated",
            syntaxTrees: [syntaxTree],
            references: _metadataReferences,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var compilationDiagnostics = compilation.GetDiagnostics()
            .Where(x => x.Severity is DiagnosticSeverity.Error)
            .ToArray();

        compilationDiagnostics.Should().BeEmpty("the generated analyzer test input should compile cleanly");

        var analyzer = new MudBlazorAnalyzer::MudBlazor.Analyzers.MudComponentUnknownParametersAnalyzer();
        var analyzerOptions = TestAnalyzerOptions.Create(allowedAttributePattern, [], customAllowedAttributes);

        var compilationWithAnalyzers = compilation.WithAnalyzers([analyzer], analyzerOptions);
        return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None).ConfigureAwait(false);
    }

    private static ImmutableArray<MetadataReference> CreateMetadataReferences()
    {
        var trustedPlatformAssemblies = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
        trustedPlatformAssemblies.Should().NotBeNullOrWhiteSpace("Trusted platform assemblies should be available for test compilation");

        var references = trustedPlatformAssemblies!
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Append(typeof(Microsoft.AspNetCore.Components.ComponentBase).Assembly.Location)
            .Append(typeof(MudBlazor.MudComponentBase).Assembly.Location)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(path => MetadataReference.CreateFromFile(path));

        return [.. references];
    }
}
#nullable restore
