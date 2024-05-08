using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var success = true;
            try
            {
                var currentCode = string.Empty;
                if (File.Exists(Paths.DocStringsFilePath))
                {
                    currentCode = File.ReadAllText(Paths.DocStringsFilePath);
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
                foreach (var type in assembly.GetTypes())
                {
                    var saveTypename = GetSaveTypename(type);

                    // TableContext was causing conflicts due to the imperfect mapping from the name of class to the name of field in DocStrings
                    if (type.IsSubclassOf(typeof(Attribute))
                        || saveTypename == "TypeInference"
                        || type == typeof(Utilities.CssBuilder)
                        || type == typeof(TableContext)
                        || saveTypename.StartsWith("EventUtil_"))
                        continue;

                    // Check if base class has same name as derived class and use only declared to prevent double generation of methods
                    var declaredOnly = type.BaseType is not null && GetSaveTypename(type.BaseType) == saveTypename;
                    var properties = type.GetPropertyInfosWithAttribute<ParameterAttribute>();
                    var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | (declaredOnly ? BindingFlags.DeclaredOnly : BindingFlags.Default));

                    foreach (var property in properties)
                    {
                        var doc = property.GetDocumentation() ?? "";
                        doc = ConvertXmlDocumentationTags(type, doc);
                        cb.AddLine($"public const string {saveTypename}_{property.Name} = @\"{EscapeDescription(doc)}\";\n");
                    }

                    foreach (var method in methods)
                    {
                        if (hiddenMethods.Contains(method.Name) || method.Name.StartsWith("get_") || method.Name.StartsWith("set_") || GetBaseDefinitionClass(method) == typeof(Enum))
                        {
                            continue;
                        }

                        var doc = method.GetDocumentation() ?? "";
                        doc = ConvertXmlDocumentationTags(type, doc);
                        cb.AddLine($"public const string {saveTypename}_method_{GetSaveMethodIdentifier(method)} = @\"{EscapeDescription(doc)}\";\n");
                    }
                }

                cb.IndentLevel--;
                cb.AddLine("}");
                cb.IndentLevel--;
                cb.AddLine("}");

                if (currentCode != cb.ToString())
                {
                    File.WriteAllText(Paths.DocStringsFilePath, cb.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error generating {Paths.DocStringsFilePath} : {e.Message}");
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

        /// <summary>
        /// Converts XML documentation elements to their HTML equivalents.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static string ConvertXmlDocumentationTags(Type type, string doc)
        {
            // Anything to do?
            if (string.IsNullOrEmpty(doc)) { return string.Empty; }
            // Combine the summary and remarks
            var summary = SummaryRegEx().Match(doc).Groups.GetValueOrDefault("1");
            var remarks = RemarksRegEx().Match(doc).Groups.GetValueOrDefault("1");
            var documentation = $"{summary} {remarks}".Trim();
            // Convert common XML documentation elements to HTML
            documentation = documentation
                .Replace("<c>", "<code class=\"docs-code docs-code-primary\">", StringComparison.OrdinalIgnoreCase)
                .Replace("</c>", "</code>", StringComparison.OrdinalIgnoreCase)
                .Replace("<para>", "<p>")
                .Replace("</para>", "</p>");
            // Resolve "<see cref" links 
            foreach (var link in LinkableCrefRegularExpression().Matches(documentation).Cast<Match>())
            {
                var before = link.Value;
                var linkType = link.Groups.GetValueOrDefault("1").Value;
                var after = link.Groups.GetValueOrDefault("2").Value;
                var components = after.Split(".");
                if (after.StartsWith("System", StringComparison.OrdinalIgnoreCase) || after.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase))
                {
                    var className = components[components.Length - 2];
                    var memberName = components[components.Length - 1];
                    after = "<a target=\"microsoft\" href=\"https://learn.microsoft.com/en-us/dotnet/api/" + after + ">" + className + "." + memberName + "</a>";
                    documentation = documentation.Replace(before, after);
                }
                else if (after.StartsWith("MudBlazor."))
                {
                    if (linkType == "T")
                    {
                        // MudBlazor type
                        var className = components[components.Length - 1];
                        var folderName = className.Replace("Mud", "").Replace("Item", "");
                        /* Uncomment to use API documentation link instead of linking to GitHub:
                        var apiPageName = className.Replace("Mud", "").Replace("`1", "").ToLowerInvariant();
                        after = "<a href=\"api/" + apiPageName + "\"><code class=\"docs-code docs-code-primary\">" + className + "</code></a>";
                        */
                        after = "<a target=\"Source\" href=\"https://github.com/MudBlazor/MudBlazor/blob/dev/src/MudBlazor/Components/" + folderName + "\">"
                              + "<code class=\"docs-code docs-code-primary\">" + className + "</code></a>";
                        documentation = documentation.Replace(before, after);
                    }
                    else if (linkType == "F")
                    {
                        // MudBlazor field (probably an enum)
                        after = components[components.Length - 2] + "." + components[components.Length - 1];
                        after = "<code class=\"docs-code docs-code-primary\">" + after + "</code>";
                        documentation = documentation.Replace(before, after);
                    }
                    else if (linkType == "P" || linkType == "M" || linkType == "E")
                    {
                        if (after.StartsWith("MudBlazor.Icons"))
                        {
                            // MudBlazor icon
                            after = "<code class=\"docs-code docs-code-primary\">" + after + "</code>";
                            documentation = documentation.Replace(before, after);
                        }
                        else
                        {
                            // MudBlazor property or method
                            var namespaceAndClass = components[components.Length - 3] + "." + components[components.Length - 2];
                            var baseType = type.Assembly.GetType(namespaceAndClass);
                            var className = components[components.Length - 2];
                            var memberName = components[components.Length - 1];

                            // Do we inherit from the linked type?
                            if (baseType.IsSubclassOfGeneric(type))
                            {
                                // Member in a base class
                                var apiPageName = type.Name.Replace("Mud", "").Replace("`1", "").Replace("`2", "").ToLowerInvariant();
                                after = "<a href=\"api/" + apiPageName + "#" + memberName + "\"><code class=\"docs-code docs-code-primary\">" + memberName + "</code></a>";
                                documentation = documentation.Replace(before, after);
                            }
                            else
                            {
                                // Member in another MudBlazor class
                                var apiPageName = className.Replace("Mud", "").Replace("`1", "").ToLowerInvariant();
                                after = "<a href=\"api/" + apiPageName + "#" + memberName + "\"><code class=\"docs-code docs-code-primary\">" + memberName + "</code></a>";
                                documentation = documentation.Replace(before, after);
                            }
                            //if (className == "MudComponentBase")
                            //{

                            //}
                            //if (type.Name == className || type.BaseType.Name == className)
                            //{
                            //    after = components[components.Length - 1];
                            //    after = "<a href=\"api/" + apiPageName + "#" + after + "\"><code class=\"docs-code docs-code-primary\">" + after + "</code></a>";
                            //    documentation = documentation.Replace(before, after);
                            //}
                        }
                    }
                }
                else
                {
                    // Some other external type
                    after = components[components.Length - 2] + "." + components[components.Length - 1];
                    after = "<code class=\"docs-code docs-code-primary\">" + after + "</code>";
                    documentation = documentation.Replace(before, after);
                }
            }

            return documentation;
        }

        private static string EscapeDescription(string doc)
        {
            return doc.Replace("\"", "\"\"");
        }

        [GeneratedRegex(@"</?.+?>")]
        private static partial Regex XmlTagRegularExpression();

        [GeneratedRegex(@"<summary>\s*([ \S]*)\s*<\/summary>")]
        private static partial Regex SummaryRegEx();

        [GeneratedRegex(@"<remarks>\s*([ \S]*)\s*<\/remarks>")]
        private static partial Regex RemarksRegEx();

        [GeneratedRegex(@"[\.,<>]")]
        private static partial Regex SaveTypenameRegularExpression();

        [GeneratedRegex("[^A-Za-z0-9_]")]
        private static partial Regex AlphanumericUnderscoreRegularExpression();

        [GeneratedRegex("<see cref=\"[TFPME]:(MudBlazor\\.)?([^>]+)\" */> ")]
        private static partial Regex SeeCrefRegularExpression();

        [GeneratedRegex("<see cref=\"([TPFME]):([\\S]*)\"\\s\\/>")]
        private static partial Regex LinkableCrefRegularExpression();

        [GeneratedRegex("`1")]
        private static partial Regex BacktickRegularExpression();
    }
}
