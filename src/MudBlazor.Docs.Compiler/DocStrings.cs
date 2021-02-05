using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
namespace MudBlazor.Docs.Compiler
{
    public class DocStrings
    {
        public bool Execute()
        {
            var paths = new Paths();
            var success = true;
            try
            {
                var currentCode = string.Empty;
                if (File.Exists(paths.DocStringsFilePath))
                {
                    currentCode = File.ReadAllText(paths.DocStringsFilePath);
                }

                var cb = new CodeBuilder();
                cb.AddHeader();
                cb.AddLine("namespace MudBlazor.Docs.Models");
                cb.AddLine("{");
                cb.IndentLevel++;
                cb.AddLine("public static partial class DocStrings");
                cb.AddLine("{");
                cb.IndentLevel++;

                var assembly = typeof(MudText).Assembly;
                foreach (var type in assembly.GetTypes().OrderBy(t => GetSaveTypename(t)))
                {
                    foreach (var info in type.GetPropertyInfosWithAttribute<ParameterAttribute>())
                    {
                        var doc = info.GetDocumentation();
                        doc = Regex.Replace(doc ?? "", @"</?.+?>", "");
                        cb.AddLine($"public const string {GetSaveTypename(type).TrimEnd('_')}_{info.Name} = @\"{EscapeDescription(doc)}\";\n");
                    }
                }

                cb.IndentLevel--;
                cb.AddLine("}");
                cb.IndentLevel--;
                cb.AddLine("}");

                if (currentCode != cb.ToString())
                {
                    File.WriteAllText(paths.DocStringsFilePath, cb.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error generating {paths.DocStringsFilePath} : {e.Message}");
                success = false;
            }

            return success;
        }

        private static string GetSaveTypename(Type t) => Regex.Replace(t.ConvertToCSharpSource(), @"[\.,<>]", "_");

        private static string EscapeDescription(string doc)
        {
            return doc.Replace("\"", "\"\"").Trim();
        }
    }
}
