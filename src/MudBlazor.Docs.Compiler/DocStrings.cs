using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
namespace MudBlazor.Docs.Compiler
{
    public class DocStrings
    {
        private static List<string> hiddenMethods = new() { "ToString", "GetType", "GetHashCode", "Equals", "SetParametersAsync", "ReferenceEquals" };

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
                    // -- properties -----------------------

                    foreach (var property in type.GetPropertyInfosWithAttribute<ParameterAttribute>())
                    {
                        var doc = property.GetDocumentation() ?? "";
                        doc = Regex.Replace(doc, @"</?.+?>", "");
                        cb.AddLine($"public const string {GetSaveTypename(type)}_{property.Name} = @\"{EscapeDescription(doc).Trim()}\";\n");
                    }

                    // -- methods --------------------------

                    // TableContext was causing conflicts due to the imperfect mapping from the name of class to the name of field in DocStrings
                    if (type.IsSubclassOf(typeof(Attribute)) || GetSaveTypename(type) == "TypeInference" || type == typeof(Utilities.CssBuilder) || type == typeof(TableContext))
                        continue;

                    foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy))
                    {
                        if (!hiddenMethods.Any(x => x.Contains(method.Name)) && !method.Name.StartsWith("get_") && !method.Name.StartsWith("set_"))
                        {
                            // omit methods defined in System.Enum
                            if (GetBaseDefinitionClass(method) == typeof(Enum))
                                continue;

                            var doc = method.GetDocumentation() ?? "";
                            cb.AddLine($"public const string {GetSaveTypename(type)}_method_{GetSaveMethodName(method)} = @\"{EscapeDescription(doc)}\";\n");
                        }
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

        private static string GetSaveTypename(Type t) => Regex.Replace(t.ConvertToCSharpSource(), @"[\.,<>]", "_").TrimEnd('_');
        private static string GetSaveMethodName(MethodInfo method) => Regex.Replace(method.ToString(), "[^A-Za-z0-9_]", "_");  // we need the method signature - it cannot be the method name alone, because methods can be overloaded

        private static Type GetBaseDefinitionClass(MethodInfo m) => m.GetBaseDefinition().DeclaringType;

        private static string EscapeDescription(string doc)
        {
            return doc.Replace("\"", "\"\"");
        }
    }
}
