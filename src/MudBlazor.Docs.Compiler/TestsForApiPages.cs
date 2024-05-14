using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Docs.Compiler
{
#nullable enable
    public partial class TestsForApiPages
    {
        public bool Execute()
        {
            var paths = new Paths();
            var success = true;
            try
            {
                Directory.CreateDirectory(paths.TestDirPath);

                var currentCode = string.Empty;
                if (File.Exists(paths.ApiPageTestsFilePath))
                {
                    currentCode = File.ReadAllText(paths.ApiPageTestsFilePath);
                }

                var cb = new CodeBuilder();

                cb.AddHeader();
                cb.AddLine("using Microsoft.AspNetCore.Components;");
                cb.AddLine("using Microsoft.Extensions.DependencyInjection;");
                cb.AddLine("using MudBlazor.Charts;");
                cb.AddLine("using MudBlazor.Docs.Components;");
                cb.AddLine("using MudBlazor.Internal;");
                cb.AddLine("using MudBlazor.UnitTests.Mocks;");
                cb.AddLine("using NUnit.Framework;");
                cb.AddLine("using ComponentParameter = Bunit.ComponentParameter;");
                cb.AddLine();

                cb.AddLine("namespace MudBlazor.UnitTests.Components");
                cb.AddLine("{");
                cb.IndentLevel++;
                cb.AddLine("// These tests just check all the API pages to see if they throw any exceptions");
                cb.AddLine("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"MudBlazor.Docs.Compiler\", \"0.0.0.0\")]");
                cb.AddLine("public partial class ApiDocsTests");
                cb.AddLine("{");
                cb.IndentLevel++;
                var mudBlazorComponents = typeof(MudAlert).Assembly.GetTypes().OrderBy(t => t.FullName).Where(t => t.IsSubclassOf(typeof(ComponentBase)));
                foreach (var type in mudBlazorComponents)
                {
                    if (type.IsAbstract)
                        continue;
                    if (type.Name.Contains("Base"))
                        continue;
                    if (type.Namespace is not null && type.Namespace.Contains("InternalComponents"))
                        continue;
                    if (IsObsolete(type))
                        continue;
                    cb.AddLine("[Test]");
                    cb.AddLine($"public void {SafeTypeName(type, removeT: true)}_API_Test()");
                    cb.AddLine("{");
                    cb.IndentLevel++;
                    cb.AddLine(@$"ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager(""https://localhost:2112/"", ""https://localhost:2112/api/{SafeTypeName(type)}""));");
                    cb.AddLine(@$"ctx.RenderComponent<DocsApi>(ComponentParameter.CreateParameter(""Type"", typeof({SafeTypeName(type)})));");
                    cb.IndentLevel--;
                    cb.AddLine("}");
                }

                cb.IndentLevel--;
                cb.AddLine("}");
                cb.IndentLevel--;
                cb.AddLine("}");

                if (currentCode != cb.ToString())
                {
                    File.WriteAllText(paths.ApiPageTestsFilePath, cb.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($@"Error generating {paths.ApiPageTestsFilePath} : {e.Message}");
                success = false;
            }

            return success;
        }

        public static bool IsObsolete(Type type)
        {
            var attributes = (ObsoleteAttribute[])type.GetCustomAttributes(typeof(ObsoleteAttribute), false);

            return attributes is { Length: > 0 };
        }

        private static string SafeTypeName(Type type, bool removeT = false)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }

            if (removeT)
            {
                return _genericTypeRegex.Replace(type.Name, string.Empty);
            }

            return _genericTypeRegex.Replace(type.Name, $"<{string.Join(',', GetGenericTypeArguments(type))}>");
        }

        private static IEnumerable<string> GetGenericTypeArguments(Type type)
        {
            if (!type.IsGenericType)
            {
                yield break;
            }

            if (_genericTypeIndexCache.TryGetValue(type.GetGenericTypeDefinition(), out var genericTypes))
            {
                foreach (var genericType in genericTypes)
                {
                    yield return genericType;
                }
            }
            else
            {
                for (var i = 0; i < type.GetGenericArguments().Length; i++)
                {
                    yield return "string";
                }
            }
        }

        /// <summary>
        /// Regular expression to match generic number at the end of a type name.
        /// example: for input MyType`2 it matches `2
        /// </summary>
        private static readonly Regex _genericTypeRegex = GenericTypeRegex();

        /// <summary>
        /// Cache for generic types that have a specific type for each generic argument.
        /// </summary>
        private static readonly Dictionary<Type, string[]> _genericTypeIndexCache = new()
        {
            { typeof(MudSlider<>), ["decimal"]},
            { typeof(MudSwitch<>), ["bool"]}
        };

        [GeneratedRegex("`\\d?$", RegexOptions.Compiled)]
        private static partial Regex GenericTypeRegex();
    }
}
