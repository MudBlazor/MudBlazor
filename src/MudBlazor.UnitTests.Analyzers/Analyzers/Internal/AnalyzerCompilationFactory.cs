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
    /// <summary>
    /// Metadata references used by the in-memory analyzer test compilation.
    /// </summary>
    private static readonly ImmutableArray<MetadataReference> _metadataReferences = CreateMetadataReferences();

    /// <summary>
    /// Builds an in-memory compilation for generated-style component code and runs the unknown-parameters analyzer.
    /// </summary>
    /// <param name="source">The C# source to compile and analyze.</param>
    /// <param name="allowedAttributePattern">The analyzer option that controls which unknown attributes are allowed.</param>
    /// <param name="allowedAttributeList">An optional explicit allow-list for <c>HTMLAttributes</c> mode.</param>
    /// <param name="sourcePath">The logical source path used for the syntax tree.</param>
    internal static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(
        string source,
        MudBlazorAnalyzer::MudBlazor.Analyzers.AllowedAttributePattern allowedAttributePattern,
        string allowedAttributeList = "",
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
        var analyzerOptions = TestAnalyzerOptions.Create(allowedAttributePattern, [], allowedAttributeList);

        var compilationWithAnalyzers = compilation.WithAnalyzers([analyzer], analyzerOptions);
        return await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync(CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates metadata references from the trusted platform assemblies for the current test runtime.
    /// </summary>
    private static ImmutableArray<MetadataReference> CreateMetadataReferences()
    {
        var trustedPlatformAssemblies = (string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES");
        trustedPlatformAssemblies.Should().NotBeNullOrWhiteSpace("trusted platform assemblies should be available");

        return trustedPlatformAssemblies!
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Append(typeof(Microsoft.AspNetCore.Components.ComponentBase).Assembly.Location)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToImmutableArray();
    }
}
#nullable restore
