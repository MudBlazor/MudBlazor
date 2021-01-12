using System;
using System.IO;
using System.Linq;

namespace MudBlazor.Docs.Compiler
{
    public class TestsForExamples
    {
        public bool Execute()
        {
            var paths = new Paths();
            bool success = true;
            try
            {
                Directory.CreateDirectory(paths.TestDirPath);

                string currentCode = string.Empty;
                if (File.Exists(paths.ComponentTestsFilePath))
                {
                    currentCode = File.ReadAllText(paths.ComponentTestsFilePath);
                }

                var cb = new CodeBuilder();

                cb.AddHeader();
                cb.AddUsings();
                
                cb.AddLine("namespace MudBlazor.UnitTests.Components");
                cb.AddLine("{");
                cb.IndentLevel++;
                cb.AddLine("// These tests just check if all the examples from the doc page render without errors");
                cb.AddLine("[TestFixture]");
                cb.AddLine("public class _AllComponents");
                cb.AddLine("{");
                cb.IndentLevel++;
                cb.AddLine("private Bunit.TestContext ctx;");
                cb.AddLine();
                cb.AddLine("[SetUp]");
                cb.AddLine("public void Setup()");
                cb.AddLine("{");
                cb.IndentLevel++;
                cb.AddLine("ctx = new Bunit.TestContext();");
                cb.AddLine("ctx.JSInterop.Mode = JSRuntimeMode.Loose;");
                cb.AddLine("ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());");
                cb.AddLine("ctx.Services.AddSingleton<IDialogService>(new DialogService());");
                cb.AddLine("ctx.Services.AddSingleton<ISnackbar>(new SnackbarService());");
                cb.AddLine("ctx.Services.AddSingleton<IResizeListenerService>(new MockResizeListenerService());");

                cb.AddLine("ctx.Services.AddTransient<IScrollManager, MockScrollManager>();");
                cb.AddLine("ctx.Services.AddTransient<IScrollListener, MockScrollListener>();");
                cb.AddLine("ctx.Services.AddSingleton<IBrowserWindowSizeProvider>(new MockBrowserWindowSizeProvider());");

                cb.AddLine("ctx.Services.AddScoped(sp => new HttpClient());");
                // options required for file upload in net
                cb.AddLine("ctx.Services.AddOptions();");
                cb.IndentLevel--;
                cb.AddLine("}");
                cb.AddLine();
                cb.AddLine("[TearDown]");
                cb.AddLine("public void TearDown() => ctx.Dispose();");
                cb.AddLine();

                foreach (var entry in Directory.EnumerateFiles(paths.DocsDirPath, "*.razor", SearchOption.AllDirectories)
                    .OrderBy(e => e.Replace("\\", "/"), StringComparer.Ordinal))
                {
                    if (entry.EndsWith("Code.razor"))
                        continue;
                    var filename = Path.GetFileName(entry);
                    var componentName = Path.GetFileNameWithoutExtension(filename);
                    if (!filename.Contains(Paths.ExampleDiscriminator))
                        continue;
                    cb.AddLine("[Test]");
                    cb.AddLine($"public void {componentName}_Test()");
                    cb.AddLine("{");
                    cb.IndentLevel++;
                    cb.AddLine($"ctx.RenderComponent<{componentName}>();");
                    cb.IndentLevel--;
                    cb.AddLine("}");
                }

                cb.IndentLevel--;
                cb.AddLine("}");
                cb.IndentLevel--;
                cb.AddLine("}");

                if (currentCode != cb.ToString())
                {
                    File.WriteAllText(paths.ComponentTestsFilePath, cb.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error generating {paths.ComponentTestsFilePath} : {e.Message}");
                success = false;
            }

            return success;
        }
    }
}