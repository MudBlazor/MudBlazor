// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MudBlazor.SourceCodeGenerator;
using NUnit.Framework;

namespace MudBlazor.UnitTests.SourceCodeGenerator;

[TestFixture]
public class FastEnumDescriptionGeneratorTest
{
    private static Compilation CreateCompilation(string source)
        => CSharpCompilation.Create("compilation",
            new[] {CSharpSyntaxTree.ParseText(source)},
            new[] {MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location)},
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));
    
    [Test]
    public void Initialize_ShouldGenerateExtensionClass_WhenDescriptionAttributeIsDefined()
    {
        // Arrange
        const string SourceCodeToTest = """
namespace MudBlazor;

public enum Priority
{
    [Description("Lowest")]
    Lowest,

    [Description("Low")]
    Low,

    [Description("Medium")]
    Medium,

    [Description("High")]
    High,

    [Description("Highest")]
    Highest
}
""";
        var generator = new FastEnumDescriptionGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        var compiledSourceCode = CreateCompilation(SourceCodeToTest);
        
        // Act
        driver = driver.RunGeneratorsAndUpdateCompilation(compiledSourceCode, out var outputCompilation, out var diagnostics);
        var result = driver.GetRunResult();
        
        // Assert
        result.GeneratedTrees.Length.Should().Be(1);
        result.Results[0].Exception.Should().BeNull();
        diagnostics.Should().HaveCount(0);
    }
    
    [Test]
    public void Initialize_ShouldNotGenerateExtensionClass_WhenEnumIsPrivate()
    {
        // Arrange
        const string SourceCodeToTest = """
namespace MudBlazor;

private enum Priority
{
    [Description("Lowest")]
    Lowest,

    [Description("Low")]
    Low,

    [Description("Medium")]
    Medium,

    [Description("High")]
    High,

    [Description("Highest")]
    Highest
}
""";
        var generator = new FastEnumDescriptionGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        var compiledSourceCode = CreateCompilation(SourceCodeToTest);
        
        // Act
        driver = driver.RunGeneratorsAndUpdateCompilation(compiledSourceCode, out _, out _);
        var result = driver.GetRunResult();
        
        // Assert
        result.GeneratedTrees.Length.Should().Be(0);

    }
    
    [Test]
    public void Initialize_ShouldUseContainingAccessModifier_WhenNestedEnumIsUsed()
    {
        
        // Arrange
        const string SourceCodeToTest = """
namespace MudBlazor;

internal class ParentClass
{
    public enum Priority
    {
        [Description("Lowest")]
        Lowest,
    }
}
""";
        var generator = new FastEnumDescriptionGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        var compiledSourceCode = CreateCompilation(SourceCodeToTest);
        
        // Act
        driver.RunGeneratorsAndUpdateCompilation(compiledSourceCode, out var outputCompilation, out _);
        
        // Assert
        var generatedSourceCode = outputCompilation.SyntaxTrees.Last().ToString();
        generatedSourceCode.Should().Contain("internal static class PriorityMudEnumExtensions").And.Contain("internal static string ToDescriptionString");
    }
}
