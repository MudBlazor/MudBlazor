// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using MudBlazor.SourceGenerator.EnumGenerators;

namespace MudBlazor.UnitTests.SourceGenerator
{
    [TestFixture]
    public class EnumSourceGeneratorTests
    {
        [Test]
        public void DescriptionEnumGeneratorShouldCreateFileWithoutDiagnostics()
        {
            // Create the 'input' compilation that the generator will act on
            Compilation inputCompilation = CreateCompilation(@"
using System.ComponentModel;

namespace MudBlazor
{
    public enum Align
    {
        [Description(""inherit"")]
        Inherit,
        [Description(""left"")]
        Left,
        [Description(""center"")]
        Center,
        [Description(""right"")]
        Right,
        [Description(""justify"")]
        Justify,
        [Description(""start"")]
        Start,
        [Description(""end"")]
        End,
    }
}
");

            // directly create an instance of the generator
            var generator = new DescriptionEnumGenerator();

            // Create the driver that will control the generation, passing in our generator
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            // Run the generation pass
            // (Note: the generator driver itself is immutable, and all calls return an updated version of the driver that you should use for subsequent calls)
            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

            // We can look at the results directly:
            var runResult = driver.GetRunResult();

            // The runResult contains the combined results of all generators passed to the driver
            Debug.Assert(runResult.GeneratedTrees.Length == 1);
            Debug.Assert(runResult.Diagnostics.IsEmpty);

            // We can access the individual results on a by-generator basis
            var generatorResult = runResult.Results[0];
            Debug.Assert(generatorResult.Generator == generator);
            Debug.Assert(generatorResult.Diagnostics.IsEmpty);
            Debug.Assert(generatorResult.GeneratedSources.Length == 1);
            Debug.Assert(generatorResult.Exception is null);
        }

        private static Compilation CreateCompilation(string source)
            => CSharpCompilation.Create("compilation",
                new[] { CSharpSyntaxTree.ParseText(source) },
                new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));
    }
}
