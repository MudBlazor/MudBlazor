using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MudBlazor.Docs.Compiler
{
    public partial class CodeSnippets
    {
        public bool Execute()
        {
            var success = true;
            try
            {
                var currentCode = string.Empty;
                if (File.Exists(Paths.SnippetsFilePath))
                {
                    currentCode = File.ReadAllText(Paths.SnippetsFilePath);
                }

                var cb = new CodeBuilder();
                cb.AddHeader();
                cb.AddLine("namespace MudBlazor.Docs.Models");
                cb.AddLine("{");
                cb.IndentLevel++;
                cb.AddLine("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"MudBlazor.Docs.Compiler\", \"0.0.0.0\")]");
                cb.AddLine("public static partial class Snippets");
                cb.AddLine("{");
                cb.IndentLevel++;

                foreach (var entry in Directory.EnumerateFiles(Paths.DocsDirPath, "*.razor", SearchOption.AllDirectories)
                    .OrderBy(e => e.Replace("\\", "/"), StringComparer.Ordinal))
                {
                    var filename = Path.GetFileName(entry);
                    var componentName = Path.GetFileNameWithoutExtension(filename);
                    if (!componentName.Contains(Paths.ExampleDiscriminator))
                        continue;
                    cb.AddLine($"public const string {componentName} = @\"{EscapeComponentSource(entry)}\";\n");
                }

                cb.IndentLevel--;
                cb.AddLine("}");
                cb.IndentLevel--;
                cb.AddLine("}");

                if (currentCode != cb.ToString())
                {
                    File.WriteAllText(Paths.SnippetsFilePath, cb.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error generating {Paths.SnippetsFilePath} : {e.Message}");
                success = false;
            }

            return success;
        }

        private static string EscapeComponentSource(string path)
        {
            var source = File.ReadAllText(path, Encoding.UTF8);
            source = NamespaceLayoutOrPageRegularExpression().Replace(source, string.Empty);
            return source.Replace("\"", "\"\"").Trim();
        }

        [GeneratedRegex("@(namespace|layout|page) .+?\n")]
        private static partial Regex NamespaceLayoutOrPageRegularExpression();
    }
}
