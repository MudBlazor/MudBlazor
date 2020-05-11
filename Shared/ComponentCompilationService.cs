using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor.Extensions;
using Microsoft.CodeAnalysis.Razor;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components;
using System.Data;
using System.Diagnostics;

namespace BlazorFiddlePoC.Shared
{
    public class ComponentCompilationService
    {
        private static Stopwatch _sw;

        public static async Task Init(HttpClient httpClient)
        {
            var referenceAssemblyRoots = new[]
            {
                typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly, // System.Runtime
                typeof(NavLink).Assembly, // Microsoft.AspNetCore.Components.Web
                typeof(DataTable).Assembly, // System.Data
                typeof(IQueryable).Assembly, // System.Linq
                typeof(HttpClientJsonExtensions).Assembly, // System.Net.Http.Json
                typeof(HttpClient).Assembly, // System.Net.Http
                typeof(TelerikGrid<>).Assembly, // Telerik.Blazor.Components
                typeof(IJSRuntime).Assembly // Microsoft.JSInterop
            };

            var assemblyNames = referenceAssemblyRoots
                .SelectMany(assembly => assembly.GetReferencedAssemblies().Concat(new[] { assembly.GetName() }))
                .Distinct()
                .Select(x => x.Name)
                .ToList();

            var assemblyStreams = await GetStreamFromHttp(httpClient, assemblyNames);

            var referenceAssemblies = assemblyStreams.Select(a => MetadataReference.CreateFromStream(a)).ToList();

            BaseCompilation = CSharpCompilation.Create(
                "TestAssembly",
                Array.Empty<SyntaxTree>(),
                referenceAssemblies,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            CSharpParseOptions = new CSharpParseOptions(LanguageVersion.Preview);
        }

        private static async Task<IEnumerable<Stream>> GetStreamFromHttp(HttpClient httpClient, IEnumerable<string> assemblyNames)
        {
            var streams = new ConcurrentBag<Stream>();

            await Task.WhenAll(
                assemblyNames.Select(async assemblyName =>
                {
                    var result = await httpClient.GetAsync($"/_framework/_bin/{assemblyName}.dll");

                    result.EnsureSuccessStatusCode();

                    streams.Add(await result.Content.ReadAsStreamAsync());
                }));

            return streams;

        }

        private static CSharpParseOptions CSharpParseOptions { get; set; }

        // Used to force a specific style of line-endings for testing. This matters
        // for the baseline tests that exercise line mappings. Even though we normalize
        // newlines for testing, the difference between platforms affects the data through
        // the *count* of characters written.
        internal virtual string LineEnding { get; } = "\n";

        internal virtual string DefaultRootNamespace { get; } = "UserComponents";

        internal virtual RazorConfiguration Configuration { get; }
            = RazorConfiguration.Create(RazorLanguageVersion.Latest, "MVC-3.0", Array.Empty<RazorExtension>());

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

        public async Task<CompileToAssemblyResult> CompileToAssembly(
            string cshtmlRelativePath,
            string cshtmlContent,
            Func<string, Task> updateStatusFunc)
        {
            _sw = Stopwatch.StartNew();
            var cSharpResult = await CompileToCSharp(cshtmlRelativePath, cshtmlContent, updateStatusFunc);

            Console.WriteLine("CompileToCSharp " + _sw.Elapsed.TotalSeconds);
            _sw.Restart();

            await (updateStatusFunc?.Invoke("Compiling Assembly") ?? Task.CompletedTask);
            var result = CompileToAssembly(cSharpResult);

            Console.WriteLine("CompileToAssembly " + _sw.Elapsed.TotalSeconds);

            return result;
        }

        public CompileToAssemblyResult CompileToAssembly(CompileToCSharpResult cSharpResult)
        {
            if (cSharpResult.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
            {
                return new CompileToAssemblyResult { Diagnostics = cSharpResult.Diagnostics, };
            }

            var syntaxTrees = new[] { Parse(cSharpResult.Code), };

            var compilation = cSharpResult.BaseCompilation.AddSyntaxTrees(syntaxTrees);

            var diagnostics = compilation
                .GetDiagnostics()
                .Where(d => d.Severity > DiagnosticSeverity.Info)
                .ToList();

            foreach (var diagnostic in diagnostics)
            {
                Console.WriteLine(diagnostic);
            }

            var result = new CompileToAssemblyResult
            {
                Compilation = compilation,
                Diagnostics = diagnostics.Select(CompilationDiagnostic.FromCSharpDiagnostic).Concat(cSharpResult.Diagnostics).ToList(),
            };

            if (result.Diagnostics.All(x => x.Severity != DiagnosticSeverity.Error))
            {
                using var peStream = new MemoryStream();
                compilation.Emit(peStream);

                result.AssemblyBytes = peStream.ToArray();
            }

            return result;
        }

        protected static CSharpSyntaxTree Parse(string text, string path = null)
        {
            return (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(text, CSharpParseOptions, path: path);
        }

        protected async Task<CompileToCSharpResult> CompileToCSharp(
            string cshtmlRelativePath,
            string cshtmlContent,
            Func<string, Task> updateStatusFunc)
        {
            if (true)
            {
                // The first phase won't include any metadata references for component discovery. This mirrors
                // what the build does.
                var projectEngine = CreateProjectEngine(Array.Empty<MetadataReference>());

                //Console.WriteLine("CreateProjectEngine " + _sw.Elapsed.TotalSeconds);
                //_sw.Restart();

                RazorCodeDocument codeDocument;
                foreach (var item in AdditionalRazorItems)
                {
                    // Result of generating declarations
                    codeDocument = projectEngine.ProcessDeclarationOnly(item);

                    var syntaxTree = Parse(codeDocument.GetCSharpDocument().GeneratedCode, path: item.FilePath);
                    AdditionalSyntaxTrees.Add(syntaxTree);
                }

                //Console.WriteLine("AdditionalRazorItems " + _sw.Elapsed.TotalSeconds);
                //_sw.Restart();

                // Result of generating declarations
                var projectItem = CreateProjectItem(cshtmlRelativePath, cshtmlContent);

                //Console.WriteLine("CreateProjectItem " + _sw.Elapsed.TotalSeconds);
                //_sw.Restart();

                codeDocument = projectEngine.ProcessDeclarationOnly(projectItem);

                //Console.WriteLine("ProcessDeclarationOnly " + _sw.Elapsed.TotalSeconds);
                //_sw.Restart();

                var declaration = new CompileToCSharpResult
                {
                    BaseCompilation = BaseCompilation.AddSyntaxTrees(AdditionalSyntaxTrees),
                    Code = codeDocument.GetCSharpDocument().GeneratedCode,
                    Diagnostics = codeDocument.GetCSharpDocument().Diagnostics.Select(CompilationDiagnostic.FromRazorDiagnostic).ToList(),
                };

                //Console.WriteLine("CompileToCSharpResult " + _sw.Elapsed.TotalSeconds);
                //_sw.Restart();

                // Result of doing 'temp' compilation
                var tempAssembly = CompileToAssembly(declaration);
                if (tempAssembly.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
                {
                    return new CompileToCSharpResult { Diagnostics = tempAssembly.Diagnostics, };
                }

                //Console.WriteLine("CompileToAssembly " + _sw.Elapsed.TotalSeconds);
                //_sw.Restart();

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

                //Console.WriteLine("CreateProjectEngine +  AdditionalRazorItems" + _sw.Elapsed.TotalSeconds);
                //_sw.Restart();


                await (updateStatusFunc?.Invoke("Preparing Project") ?? Task.CompletedTask);
                // Result of real code generation for the document under test
                codeDocument = DesignTime ? projectEngine.ProcessDesignTime(projectItem) : projectEngine.Process(projectItem);

                //Console.WriteLine("ProcessDesignTime " + _sw.Elapsed.TotalSeconds);
                //Console.WriteLine("DesignTime " + DesignTime);
                //_sw.Restart();

                return new CompileToCSharpResult
                {
                    BaseCompilation = BaseCompilation.AddSyntaxTrees(AdditionalSyntaxTrees),
                    Code = codeDocument.GetCSharpDocument().GeneratedCode,
                    Diagnostics = codeDocument.GetCSharpDocument().Diagnostics.Select(CompilationDiagnostic.FromRazorDiagnostic).ToList(),
                };
            }
            else
            {
                // For single phase compilation tests just use the base compilation's references.
                // This will include the built -in Blazor components.
                var projectEngine = CreateProjectEngine(BaseCompilation.References.ToList());

                var projectItem = CreateProjectItem(cshtmlRelativePath, cshtmlContent);
                var codeDocument = DesignTime ? projectEngine.ProcessDesignTime(projectItem) : projectEngine.Process(projectItem);

                return new CompileToCSharpResult
                {
                    BaseCompilation = BaseCompilation.AddSyntaxTrees(AdditionalSyntaxTrees),
                    Code = codeDocument.GetCSharpDocument().GeneratedCode,
                    Diagnostics = codeDocument.GetCSharpDocument().Diagnostics.Select(CompilationDiagnostic.FromRazorDiagnostic).ToList(),
                };
            }
        }

        internal RazorProjectItem CreateProjectItem(string cshtmlRelativePath, string cshtmlContent)
        {
            var fullPath = WorkingDirectory + PathSeparator + cshtmlRelativePath;

            // FilePaths in Razor are **always** of the form '/a/b/c.cshtml'
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

        private RazorProjectEngine CreateProjectEngine(IReadOnlyList<MetadataReference> references)
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
                CompilerFeatures.Register(b);

                b.Features.Add(new CompilationTagHelperFeature());
                b.Features.Add(new DefaultMetadataReferenceFeature { References = references, });
            });
        }
    }
}
