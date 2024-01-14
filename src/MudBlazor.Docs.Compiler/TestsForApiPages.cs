using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Docs.Compiler
{
    public class TestsForApiPages
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
                    if (type.Namespace.Contains("InternalComponents"))
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
                Console.WriteLine($"Error generating {paths.ApiPageTestsFilePath} : {e.Message}");
                success = false;
            }

            return success;
        }

        public static bool IsObsolete(Type type)
        {
            var attributes = (ObsoleteAttribute[])
                type.GetCustomAttributes(typeof(ObsoleteAttribute), false);
            return (attributes != null && attributes.Length > 0);
        }

        private static string SafeTypeName(Type type, bool removeT = false)
        {
            if (!type.IsGenericType)
                return type.Name;
            var genericTypename = type.Name;
            if (removeT)
                return genericTypename.Replace("`1", string.Empty).Replace("`2", string.Empty);
            return genericTypename.Replace("`1", "<T>").Replace("`2", "<T, U>");
        }
    }
}
