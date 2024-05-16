using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;
namespace MudBlazor.Docs.Compiler
{
    public partial class DocStrings
    {
        private static string[] hiddenMethods = { "ToString", "GetType", "GetHashCode", "Equals", "SetParametersAsync", "ReferenceEquals" };

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
                cb.AddLine("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"MudBlazor.Docs.Compiler\", \"0.0.0.0\")]");
                cb.AddLine("public static partial class DocStrings");
                cb.AddLine("{");
                cb.IndentLevel++;

                var assembly = typeof(MudText).Assembly;
                foreach (var type in assembly.GetTypes().OrderBy(t => GetSaveTypename(t)))
                {
                    foreach (var property in type.GetPropertyInfosWithAttribute<ParameterAttribute>())
                    {
                        var doc = property.GetDocumentation() ?? "";
                        doc = convertSeeTags(doc);
                        doc = XmlTagRegularExpression().Replace(doc, "");  // remove all other XML tags
                        cb.AddLine($"public const string {GetSaveTypename(type)}_{property.Name} = @\"{EscapeDescription(doc).Trim()}\";\n");
                    }

                    // TableContext was causing conflicts due to the imperfect mapping from the name of class to the name of field in DocStrings
                    if (type.IsSubclassOf(typeof(Attribute)) || GetSaveTypename(type) == "TypeInference"
                            || type == typeof(Utilities.CssBuilder) || type == typeof(TableContext) || GetSaveTypename(type).StartsWith("EventUtil_"))
                        continue;

                    // Check if base class has same name as derived class and use only declared to prevent double generation of methods
                    var declaredOnly = type.BaseType is not null && GetSaveTypename(type.BaseType) == GetSaveTypename(type);

                    foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | (declaredOnly ? BindingFlags.DeclaredOnly : BindingFlags.Default)))
                    {
                        if (!hiddenMethods.Any(x => x.Contains(method.Name)) && !method.Name.StartsWith("get_") && !method.Name.StartsWith("set_"))
                        {
                            // omit methods defined in System.Enum
                            if (GetBaseDefinitionClass(method) == typeof(Enum))
                                continue;

                            var doc = method.GetDocumentation() ?? "";
                            cb.AddLine($"public const string {GetSaveTypename(type)}_method_{GetSaveMethodIdentifier(method)} = @\"{EscapeDescription(doc)}\";\n");
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

        private static string GetSaveTypename(Type t) => SaveTypenameRegularExpression().Replace(t.ConvertToCSharpSource(), "_").TrimEnd('_');

        /* Methods can be overloaded so the method name doesn't identify it uniquely. Instead of method name we need the method signature.
         * Currently the return type of a method is also used, but probably it can be removed.
         *
         * Alternatively we could use the format similar to this used in XML documentation - it will be even better because I think it is
         * less likely to be changed in the future. See XmlDocumentation.cs for a method computing identifiers.
         */
        private static string GetSaveMethodIdentifier(MethodInfo method) => AlphanumericUnderscoreRegularExpression().Replace(method.ToString(), "_");

        private static Type GetBaseDefinitionClass(MethodInfo m) => m.GetBaseDefinition().DeclaringType;

        /* Replace <see cref="TYPE_OR_MEMBER_QUALIFIED_NAME"/> tags by TYPE_OR_MEMBER_QUALIFIED_NAME without "MudBlazor." at the beginning.
         * It is a quick fix. It should be rather represented by <a href="...">...</a> but it is more difficult.
         */
        private static string convertSeeTags(string doc)
        {
            return SeeCrefRegularExpression().Replace(doc, match =>
            {
                var result = match.Groups[2].Value;     // get the name of Type or type member (Field, Property, Method, or Event)
                result = BacktickRegularExpression().Replace(result, "");  // remove `1 from generic type name
                return result;
            });
        }

        private static string EscapeDescription(string doc)
        {
            return doc.Replace("\"", "\"\"");
        }

        [GeneratedRegex(@"</?.+?>")]
        private static partial Regex XmlTagRegularExpression();

        [GeneratedRegex(@"[\.,<>]")]
        private static partial Regex SaveTypenameRegularExpression();

        [GeneratedRegex("[^A-Za-z0-9_]")]
        private static partial Regex AlphanumericUnderscoreRegularExpression();

        [GeneratedRegex("<see cref=\"[TFPME]:(MudBlazor\\.)?([^>]+)\" */>")]
        private static partial Regex SeeCrefRegularExpression();

        [GeneratedRegex("`1")]
        private static partial Regex BacktickRegularExpression();
    }
}
