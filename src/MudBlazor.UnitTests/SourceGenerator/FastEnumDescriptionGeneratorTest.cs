// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using MudBlazor.SourceGenerator;
using NUnit.Framework;

namespace MudBlazor.UnitTests.SourceGenerator;

[TestFixture]
public class FastEnumDescriptionGeneratorTest
{
    [SetUp]
    public void Setup()
    {
        var generator = new FastEnumDescriptionGenerator();
        _driver = CSharpGeneratorDriver.Create(generator);
    }

    private GeneratorDriver _driver;

    private static CSharpCompilation CreateCompilation(string source)
    {
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic)
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
            .Cast<MetadataReference>();

        return CSharpCompilation.Create("SourceGeneratorTest",
            [CSharpSyntaxTree.ParseText(source)],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    [Test]
    public void Generator_ShouldGenerateExtensionClass_WhenDescriptionAttributeIsDefined()
    {
        // Arrange
        const string SourceCodeToTest = """
                                        using System.ComponentModel;

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

        var compiledSourceCode = CreateCompilation(SourceCodeToTest);

        // Act
        _driver.RunGeneratorsAndUpdateCompilation(compiledSourceCode, out var outputCompilation, out var diagnostics);

        // Assert
        outputCompilation.SyntaxTrees.Should().HaveCount(2);
        diagnostics.Should().HaveCount(0);
    }

    [Test]
    public void Generator_ShouldGenerateDiagnosticWarning_WhenDescriptionAttributeUsageIsInconsistent()
    {
        // Arrange
        const string SourceCodeToTest = """
                                        using System.ComponentModel;

                                        namespace MudBlazor;

                                        public enum Priority
                                        {
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

        var compiledSourceCode = CreateCompilation(SourceCodeToTest);

        // Act
        _driver.RunGeneratorsAndUpdateCompilation(compiledSourceCode, out var outputCompilation, out var diagnostics);

        // Assert
        outputCompilation.SyntaxTrees.Should().HaveCount(2);
        diagnostics.Should().HaveCount(1);
        diagnostics.First().Id.Should().Be("MUD0101");
    }

    [Test]
    public void Generator_ShouldNotGenerateExtensionClass_WhenEnumIsPrivate()
    {
        // Arrange
        const string SourceCodeToTest = """
                                        using System.ComponentModel;

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
        var compiledSourceCode = CreateCompilation(SourceCodeToTest);

        // Act
        _driver.RunGeneratorsAndUpdateCompilation(compiledSourceCode, out var outputCompilation, out _);

        // Assert
        outputCompilation.SyntaxTrees.Should().HaveCount(1);
    }

    [Test]
    public void Generator_ShouldNotGenerateExtensionClass_WhenNoDescriptionAttributeIsUsed()
    {
        // Arrange
        const string SourceCodeToTest = """
                                        namespace MudBlazor;

                                        public enum Priority
                                        {
                                            Highest
                                        }
                                        """;
        var compiledSourceCode = CreateCompilation(SourceCodeToTest);

        // Act
        _driver.RunGeneratorsAndUpdateCompilation(compiledSourceCode, out var outputCompilation, out _);

        // Assert
        outputCompilation.SyntaxTrees.Should().HaveCount(1);
    }

    [Test]
    public void Generator_ShouldEscapeReservedKeywords_WhenKeywordIsUsedInEnumMember()
    {
        // Arrange
        const string SourceCodeToTest = """
                                        using System.ComponentModel;

                                        namespace MudBlazor;

                                        public enum Priority
                                        {
                                            [Description("byte")]
                                            @byte
                                        }
                                        """;
        var compiledSourceCode = CreateCompilation(SourceCodeToTest);

        // Act
        _driver.RunGeneratorsAndUpdateCompilation(compiledSourceCode, out var outputCompilation, out _);

        // Assert
        outputCompilation.SyntaxTrees.Should().HaveCount(2);
        var generatedSourceCode = outputCompilation.SyntaxTrees.Last().ToString();
        generatedSourceCode.Should().Contain("@byte => \"byte\"");
    }

    [Test]
    public void Generator_ShouldUseContainingAccessModifier_WhenNestedEnumIsUsed()
    {
        // Arrange
        const string SourceCodeToTest = """
                                        using System.ComponentModel;

                                        namespace MudBlazor;

                                        internal class ParentClass
                                        {
                                            public enum Priority
                                            {
                                                [Description("Lowest")]
                                                Lowest
                                            }
                                        }
                                        """;
        var compiledSourceCode = CreateCompilation(SourceCodeToTest);

        // Act
        _driver.RunGeneratorsAndUpdateCompilation(compiledSourceCode, out var outputCompilation, out _);

        // Assert
        outputCompilation.SyntaxTrees.Should().HaveCount(2);
        var generatedSourceCode = outputCompilation.SyntaxTrees.Last().ToString();
        generatedSourceCode.Should().Contain("internal static class PrioritySourceGeneratorEnumExtensions").And.Contain("internal static string ToDescriptionString");
    }

    [Test]
    public void Generator_ShouldEscapeDescriptionValue_WhenValueContainsEscapedCharacters()
    {
        // Arrange
        const string SourceCodeToTest = """
                                        using System.ComponentModel;

                                        namespace MudBlazor;

                                        public enum Priority
                                        {
                                            [Description("Low\"est")]
                                            Lowest,
                                        
                                            [Description("\"Low\"")]
                                            Low,
                                        }
                                        """;

        var compiledSourceCode = CreateCompilation(SourceCodeToTest);

        // Act
        _driver.RunGeneratorsAndUpdateCompilation(compiledSourceCode, out var outputCompilation, out _);

        // Assert
        outputCompilation.SyntaxTrees.Should().HaveCount(2);
        const string Expected = """
                                        return mudEnum switch
                                        {
                                            MudBlazor.Priority.Lowest => "Low\"est",
                                            MudBlazor.Priority.Low => "\"Low\"",
                                            _ => mudEnum.ToString()
                                        };
                                """;
        var generatedSourceCode = outputCompilation.SyntaxTrees.Last().ToString().ReplaceLineEndings();
        generatedSourceCode.Should().Contain(Expected.ReplaceLineEndings());
    }
}
