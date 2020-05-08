using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Razor.Extensions;
using Microsoft.CodeAnalysis.Razor;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace BlazorFiddlePoC.Shared
{
    public class ComponentCompilationService
    {
        private static readonly StringBuilder _output = new StringBuilder();

        public static async Task Init()
        {
            var referenceAssemblyRoots = new[]
            {
                typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly, // System.Runtime
                //typeof(ComponentBase).Assembly,
                typeof(NavLink).Assembly,
                typeof(HttpClientJsonExtensions).Assembly,
                typeof(HttpClient).Assembly,
                typeof(IJSRuntime).Assembly,
                typeof(JsonSerializer).Assembly,
                typeof(ComponentCompilationService).Assembly, // Reference this assembly, so that we can refer to test component types
            };

            var temp = referenceAssemblyRoots
                .SelectMany(assembly => assembly.GetReferencedAssemblies().Concat(new[] { assembly.GetName() }))
                .Distinct()
                .ToList();

            Console.WriteLine(string.Join(", ", temp));


            //var temp2 = temp.Select(Assembly.Load)
            //    .ToList();

            var assemblyStreams = await GetStreamFromHttp(new HttpClient(), temp.Select(x => x.Name));

            ////Console.WriteLine(temp.Count);

            //////Console.WriteLine(string.Join(", ", temp2.SelectMany(assembly => assembly.GetManifestResourceNames().FirstOrDefault())));
            ////Console.WriteLine(string.Join(", ", temp2.Select(x => x.Location)));


            ////var referenceAssemblies = temp2
            ////    .Where(x => x.GetManifestResourceNames().Any())
            ////    .Select(assembly => MetadataReference.CreateFromStream(
            ////        assembly.GetManifestResourceStream(
            ////            assembly.GetManifestResourceNames().First())))
            ////    .ToList();

            ////Console.WriteLine(temp.Count);
            ///

            var referenceAssemblies = assemblyStreams.Select(a => MetadataReference.CreateFromStream(a)).ToList();

            Console.WriteLine("Finish Creating");

            BaseCompilation = CSharpCompilation.Create(
                "TestAssembly",
                Array.Empty<SyntaxTree>(),
                referenceAssemblies,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            CSharpParseOptions = new CSharpParseOptions(LanguageVersion.Preview);
        }

        private static async Task<IEnumerable<Stream>> GetStreamFromHttp(HttpClient httpClient, IEnumerable<string> enumerable)
        {
            var streams = new ConcurrentBag<Stream>();

            await Task.WhenAll(
                enumerable.Select(async e =>
                {
                    Console.WriteLine(e);
                    var result = await httpClient.GetAsync("https://localhost:44347/_framework/_bin/" + e + ".dll");

                    result.EnsureSuccessStatusCode();

                    streams.Add(await result.Content.ReadAsStreamAsync());
                }));

            Console.WriteLine("Finish Getting from http");

            return streams;

        }

        private static CSharpParseOptions CSharpParseOptions { get; set; }

        // Used to force a specific style of line-endings for testing. This matters
        // for the baseline tests that exercise line mappings. Even though we normalize
        // newlines for testing, the difference between platforms affects the data through
        // the *count* of characters written.
        internal virtual string LineEnding { get; } = "\n";

        internal virtual string DefaultRootNamespace { get; } = "UserComponents";

        internal virtual RazorConfiguration Configuration { get; } = RazorConfiguration.Create(RazorLanguageVersion.Latest, "MVC-3.0", Array.Empty<RazorExtension>());

        internal virtual VirtualRazorProjectFileSystem FileSystem { get; } = new VirtualRazorProjectFileSystem();

        internal List<RazorProjectItem> AdditionalRazorItems { get; } = new List<RazorProjectItem>();

        internal List<SyntaxTree> AdditionalSyntaxTrees { get; } = new List<SyntaxTree>();

        internal virtual bool DesignTime { get; }

        internal virtual string PathSeparator { get; } = Path.DirectorySeparatorChar.ToString();

        internal virtual string WorkingDirectory { get; } = "x:\\dir\\subdir\\Test";

        internal virtual bool NormalizeSourceLineEndings { get; } = true;

        internal virtual string FileKind { get; } = FileKinds.Component;

        // Creating the initial compilation + reading references is on the order of 250ms without caching
        // so making sure it doesn't happen for each test.
        private static CSharpCompilation BaseCompilation;

        public CompileToAssemblyResult CompileToAssembly(string cshtmlRelativePath, string cshtmlContent)
        {
            var cSharpResult = CompileToCSharp(cshtmlRelativePath, cshtmlContent);

            var result = CompileToAssembly(cSharpResult);

            //var test = AssemblyLoadContext.Default;

            return result;
        }

        public CompileToAssemblyResult CompileToAssembly(CompileToCSharpResult cSharpResult, bool throwOnFailure = true)
        {
            if (cSharpResult.Diagnostics.Any())
            {
                var diagnosticsLog = string.Join(Environment.NewLine, cSharpResult.Diagnostics.Select(d => d.ToString()).ToArray());
                throw new InvalidOperationException($"Aborting compilation to assembly because RazorCompiler returned nonempty diagnostics: {diagnosticsLog}");
            }

            var syntaxTrees = new[]
            {
                Parse(cSharpResult.Code),
            };

            var compilation = cSharpResult.BaseCompilation.AddSyntaxTrees(syntaxTrees);

            var diagnostics = compilation
                .GetDiagnostics()
                .Where(d => d.Severity != DiagnosticSeverity.Hidden);

            Console.WriteLine(JsonSerializer.Serialize(diagnostics));

            if (diagnostics.Any() && throwOnFailure)
            {
                throw new Exception(compilation.ToString());
            }
            else if (diagnostics.Any())
            {
                return new CompileToAssemblyResult
                {
                    Compilation = compilation,
                    Diagnostics = diagnostics,
                };
            }

            using (var peStream = new MemoryStream())
            {
                compilation.Emit(peStream);

                return new CompileToAssemblyResult
                {
                    Compilation = compilation,
                    Diagnostics = diagnostics,
                    Assembly = diagnostics.Any() ? null : Assembly.Load(peStream.ToArray()),
                    Base64Assembly = Convert.ToBase64String(peStream.ToArray()),
                    AssemblyBytes = peStream.ToArray()
                };
            }
        }

        protected static CSharpSyntaxTree Parse(string text, string path = null)
        {
            return (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(text, CSharpParseOptions, path: path);
        }

        protected CompileToCSharpResult CompileToCSharp(string cshtmlRelativePath, string cshtmlContent)
        {
            if (true)
            {
                // The first phase won't include any metadata references for component discovery. This mirrors
                // what the build does.
                var projectEngine = CreateProjectEngine(Array.Empty<MetadataReference>());

                RazorCodeDocument codeDocument;
                foreach (var item in AdditionalRazorItems)
                {
                    // Result of generating declarations
                    codeDocument = projectEngine.ProcessDeclarationOnly(item);
                    //Assert.Empty(codeDocument.GetCSharpDocument().Diagnostics);

                    var syntaxTree = Parse(codeDocument.GetCSharpDocument().GeneratedCode, path: item.FilePath);
                    AdditionalSyntaxTrees.Add(syntaxTree);
                }

                // Result of generating declarations
                var projectItem = CreateProjectItem(cshtmlRelativePath, cshtmlContent);
                codeDocument = projectEngine.ProcessDeclarationOnly(projectItem);
                var declaration = new CompileToCSharpResult
                {
                    BaseCompilation = BaseCompilation.AddSyntaxTrees(AdditionalSyntaxTrees),
                    CodeDocument = codeDocument,
                    Code = codeDocument.GetCSharpDocument().GeneratedCode,
                    Diagnostics = codeDocument.GetCSharpDocument().Diagnostics,
                };

                // Result of doing 'temp' compilation
                var tempAssembly = CompileToAssembly(declaration);

                // Add the 'temp' compilation as a metadata reference
                var references = BaseCompilation.References.Concat(new[] { tempAssembly.Compilation.ToMetadataReference() }).ToArray();
                projectEngine = CreateProjectEngine(references);

                // Now update the any additional files
                foreach (var item in AdditionalRazorItems)
                {
                    // Result of generating declarations
                    codeDocument = DesignTime ? projectEngine.ProcessDesignTime(item) : projectEngine.Process(item);
                    //Assert.Empty(codeDocument.GetCSharpDocument().Diagnostics);

                    // Replace the 'declaration' syntax tree
                    var syntaxTree = Parse(codeDocument.GetCSharpDocument().GeneratedCode, path: item.FilePath);
                    AdditionalSyntaxTrees.RemoveAll(st => st.FilePath == item.FilePath);
                    AdditionalSyntaxTrees.Add(syntaxTree);
                }

                // Result of real code generation for the document under test
                codeDocument = DesignTime ? projectEngine.ProcessDesignTime(projectItem) : projectEngine.Process(projectItem);

                _output.AppendLine("Use this output when opening an issue");
                _output.AppendLine(string.Empty);

                _output.AppendLine($"## Main source file ({projectItem.FileKind}):");
                _output.AppendLine("```");
                _output.AppendLine(ReadProjectItem(projectItem));
                _output.AppendLine("```");
                _output.AppendLine(string.Empty);

                foreach (var item in AdditionalRazorItems)
                {
                    _output.AppendLine($"### Additional source file ({item.FileKind}):");
                    _output.AppendLine("```");
                    _output.AppendLine(ReadProjectItem(item));
                    _output.AppendLine("```");
                    _output.AppendLine(string.Empty);
                }

                _output.AppendLine("## Generated C#:");
                _output.AppendLine("```C#");
                _output.AppendLine(codeDocument.GetCSharpDocument().GeneratedCode);
                _output.AppendLine("```");

                return new CompileToCSharpResult
                {
                    BaseCompilation = BaseCompilation.AddSyntaxTrees(AdditionalSyntaxTrees),
                    CodeDocument = codeDocument,
                    Code = codeDocument.GetCSharpDocument().GeneratedCode,
                    Diagnostics = codeDocument.GetCSharpDocument().Diagnostics,
                };
            }
            else
            {
                // For single phase compilation tests just use the base compilation's references.
                // This will include the built-in Blazor components.
                //var projectEngine = CreateProjectEngine(BaseCompilation.References.ToArray());

                //var projectItem = CreateProjectItem(cshtmlRelativePath, cshtmlContent);
                //var codeDocument = DesignTime ? projectEngine.ProcessDesignTime(projectItem) : projectEngine.Process(projectItem);

                //// Log the generated code for test results.
                //_output.Value.WriteLine("Use this output when opening an issue");
                //_output.Value.WriteLine(string.Empty);

                //_output.Value.WriteLine($"## Main source file ({projectItem.FileKind}):");
                //_output.Value.WriteLine("```");
                //_output.Value.WriteLine(ReadProjectItem(projectItem));
                //_output.Value.WriteLine("```");
                //_output.Value.WriteLine(string.Empty);

                //_output.Value.WriteLine("## Generated C#:");
                //_output.Value.WriteLine("```C#");
                //_output.Value.WriteLine(codeDocument.GetCSharpDocument().GeneratedCode);
                //_output.Value.WriteLine("```");

                //return new CompileToCSharpResult
                //{
                //    BaseCompilation = BaseCompilation.AddSyntaxTrees(AdditionalSyntaxTrees),
                //    CodeDocument = codeDocument,
                //    Code = codeDocument.GetCSharpDocument().GeneratedCode,
                //    Diagnostics = codeDocument.GetCSharpDocument().Diagnostics,
                //};
            }
        }

        private static string ReadProjectItem(RazorProjectItem item)
        {
            using (var reader = new StreamReader(item.Read()))
            {
                return reader.ReadToEnd();
            }
        }

        internal RazorProjectItem CreateProjectItem(string cshtmlRelativePath, string cshtmlContent)
        {
            var fullPath = WorkingDirectory + PathSeparator + cshtmlRelativePath;

            // FilePaths in Razor are **always** are of the form '/a/b/c.cshtml'
            var filePath = cshtmlRelativePath.Replace('\\', '/');
            if (!filePath.StartsWith('/'))
            {
                filePath = '/' + filePath;
            }

            if (NormalizeSourceLineEndings)
            {
                cshtmlContent = cshtmlContent.Replace("\r", "").Replace("\n", LineEnding);
            }

            return new VirtualProjectItem(
                WorkingDirectory,
                filePath,
                fullPath,
                cshtmlRelativePath,
                FileKind,
                Encoding.UTF8.GetBytes(cshtmlContent.TrimStart()));
        }

        private RazorProjectEngine CreateProjectEngine(MetadataReference[] references)
        {
            return RazorProjectEngine.Create(Configuration, FileSystem, b =>
            {
                b.SetRootNamespace(DefaultRootNamespace);

                // Turn off checksums, we're testing code generation.
                b.Features.Add(new SuppressChecksum());

                if (LineEnding != null)
                {
                    b.Phases.Insert(0, new ForceLineEndingPhase(LineEnding));
                }

                // Including MVC here so that we can find any issues that arise from mixed MVC + Components.
                RazorExtensions.Register(b);

                // Features that use Roslyn are mandatory for components
                Microsoft.CodeAnalysis.Razor.CompilerFeatures.Register(b);

                b.Features.Add(new CompilationTagHelperFeature());
                b.Features.Add(new DefaultMetadataReferenceFeature()
                {
                    References = references,
                });
            });
        }
    }

    public class ForceLineEndingPhase : RazorEnginePhaseBase
    {
        public ForceLineEndingPhase(string lineEnding)
        {
            LineEnding = lineEnding;
        }

        public string LineEnding { get; }

        protected override void ExecuteCore(RazorCodeDocument codeDocument)
        {
            var field = typeof(CodeRenderingContext).GetField("NewLineString", BindingFlags.Static | BindingFlags.NonPublic);
            var key = field.GetValue(null);
            codeDocument.Items[key] = LineEnding;
        }
    }

    public class SuppressChecksum : IConfigureRazorCodeGenerationOptionsFeature
    {
        public int Order => 0;

        public RazorEngine Engine { get; set; }

        public void Configure(RazorCodeGenerationOptionsBuilder options)
        {
            options.SuppressChecksum = true;
        }
    }

    public class CompileToAssemblyResult
    {
        public Assembly Assembly { get; set; }
        public Compilation Compilation { get; set; }
        public string VerboseLog { get; set; }
        public IEnumerable<Diagnostic> Diagnostics { get; set; }
        public string Base64Assembly { get; set; }
        public byte[] AssemblyBytes { get; set; }
    }



    public class CompileToCSharpResult
    {
        // A compilation that can be used *with* this code to compile an assembly
        public Compilation BaseCompilation { get; set; }
        public RazorCodeDocument CodeDocument { get; set; }
        public string Code { get; set; }
        public IEnumerable<RazorDiagnostic> Diagnostics { get; set; }
    }
}
