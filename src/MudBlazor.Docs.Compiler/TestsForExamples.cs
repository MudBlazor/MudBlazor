using System;
using System.IO;
using System.Linq;

namespace MudBlazor.Docs.Compiler
{
    public class TestsForExamples
    {
        public bool Execute()
        {
            var success = true;
            try
            {
                Directory.CreateDirectory(Paths.TestDirPath);

                var currentCode = string.Empty;
                if (File.Exists(Paths.ComponentTestsFilePath))
                {
                    currentCode = File.ReadAllText(Paths.ComponentTestsFilePath);
                }

                var cb = new CodeBuilder();

                cb.AddHeader();
                cb.AddLine("using MudBlazor.Docs.Examples;");
                cb.AddLine("using MudBlazor.Docs.Wireframes;");
                cb.AddLine("using NUnit.Framework;");
                cb.AddLine();

                cb.AddLine("namespace MudBlazor.UnitTests.Components");
                cb.AddLine("{");
                cb.IndentLevel++;
                cb.AddLine("// These tests just check if all the examples from the doc page render without errors");
                cb.AddLine("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"MudBlazor.Docs.Compiler\", \"0.0.0.0\")]");
                cb.AddLine("public partial class ExampleDocsTests");
                cb.AddLine("{");
                cb.IndentLevel++;

                foreach (var entry in Directory.EnumerateFiles(Paths.DocsDirPath, "*.razor", SearchOption.AllDirectories)
                    .OrderBy(e => e.Replace("\\", "/"), StringComparer.Ordinal))
                {
                    if (entry.EndsWith("Code.razor"))
                        continue;
                    var filename = Path.GetFileName(entry);
                    var componentName = Path.GetFileNameWithoutExtension(filename);
                    if (!filename.Contains(Paths.ExampleDiscriminator))
                        continue;
                    // skip over table/data grid virutalization since it takes too long.
                    if (filename == "TableVirtualizationExample.razor" || filename == "DataGridVirtualizationExample.razor")
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
                    File.WriteAllText(Paths.ComponentTestsFilePath, cb.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error generating {Paths.ComponentTestsFilePath} : {e.Message}");
                success = false;
            }

            return success;
        }
    }
}
