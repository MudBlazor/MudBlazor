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

                cb.AddLine("using Microsoft.AspNetCore.Components;");
                cb.AddLine("using Microsoft.Extensions.DependencyInjection;");
                cb.AddLine("using NUnit.Framework;");
                cb.AddLine("using MudBlazor.UnitTests.Mocks;");
                cb.AddLine("using MudBlazor.Docs.Examples;");
                cb.AddLine("using MudBlazor.Dialog;");
                cb.AddLine("using MudBlazor.Services;");
                cb.AddLine();
                cb.AddLine("namespace MudBlazor.UnitTests.Components");
                cb.AddLine("{");
                cb.IndentLevel++;
                cb.AddLine("// These tests just check if all the examples from the doc page render without errors");
                cb.AddLine("[TestFixture]");
                cb.AddLine("public class _AllComponents");
                cb.AddLine("{");
                cb.IndentLevel++;

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
                    cb.AddLine("using var ctx = new Bunit.TestContext();");
                    cb.AddLine("ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());");
                    cb.AddLine("ctx.Services.AddSingleton<IDialogService>(new DialogService());");
                    cb.AddLine("ctx.Services.AddSingleton<ISnackbar>(new MockSnackbar());");
                    cb.AddLine("ctx.Services.AddSingleton<IResizeListenerService>(new MockResizeListenerService());");
                    cb.AddLine($"var comp = ctx.RenderComponent<{componentName}>();");
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