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
            bool success = true;
            try
            {
                Directory.CreateDirectory(paths.TestDirPath);

                string currentCode = string.Empty;
                if (File.Exists(paths.ApiPageTestsFilePath))
                {
                    currentCode = File.ReadAllText(paths.ApiPageTestsFilePath);
                }

                var cb = new CodeBuilder();

                cb.AddHeader();
                cb.AddLine("using Microsoft.AspNetCore.Components;");
                cb.AddLine("using Microsoft.Extensions.DependencyInjection;");
                cb.AddLine("using NUnit.Framework;");
                cb.AddLine("using MudBlazor.UnitTests.Mocks;");
                cb.AddLine("using MudBlazor.Docs.Examples;");
                cb.AddLine("using MudBlazor.Services;");
                cb.AddLine("using MudBlazor.Docs.Components;");
                cb.AddLine("using Bunit.Rendering;");
                cb.AddLine("using System;");
                cb.AddLine("using Toolbelt.Blazor.HeadElement;");
                cb.AddLine("using MudBlazor.UnitTests;");
                cb.AddLine("using MudBlazor.Charts;");
                cb.AddLine("using Bunit;");
                cb.AddLine();
                cb.AddLine("#if NET5_0");
                cb.AddLine("using ComponentParameter = Bunit.ComponentParameter;");
                cb.AddLine("#endif");
                cb.AddLine();
                cb.AddLine("namespace MudBlazor.UnitTests.Components");
                cb.AddLine("{");
                cb.IndentLevel++;
                cb.AddLine("// These tests just check all the API pages to see if they throw any exceptions");
                cb.AddLine("[TestFixture]");
                cb.AddLine("public class _AllApiPages");
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
                    cb.AddLine("[Test]");
                    cb.AddLine($"public void {SafeTypeName(type, removeT: true)}_API_Test()");
                    cb.AddLine("{");
                    cb.IndentLevel++;
                    cb.AddLine("using var ctx = new Bunit.TestContext();");
                    cb.AddLine("ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());");
                    cb.AddLine("ctx.Services.AddSingleton<IDialogService>(new DialogService());");
                    cb.AddLine("ctx.Services.AddSingleton<IResizeListenerService>(new MockResizeListenerService());");
                    cb.AddLine("ctx.Services.AddSingleton<IHeadElementHelper>(new MockHeadElementHelper());");
                    cb.AddLine("ctx.Services.AddSingleton<ISnackbar>(new MockSnackbar());");
                    cb.AddLine(@$"var comp = ctx.RenderComponent<DocsApi>(ComponentParameter.CreateParameter(""Type"", typeof({SafeTypeName(type)})));");
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

        private static string SafeTypeName(Type type, bool removeT = false)
        {
            if (!type.IsGenericType)
                return type.Name;
            var genericTypename = type.Name;
            if (removeT)
                return genericTypename.Replace("`1", string.Empty);
            return genericTypename.Replace("`1", "<T>");
        }
    }
}