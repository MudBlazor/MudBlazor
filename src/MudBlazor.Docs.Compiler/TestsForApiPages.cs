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
                cb.AddUsings();
                
                cb.AddLine("namespace MudBlazor.UnitTests.Components");
                cb.AddLine("{");
                cb.IndentLevel++;
                cb.AddLine("// These tests just check all the API pages to see if they throw any exceptions");
                cb.AddLine("[TestFixture]");
                cb.AddLine("public class _AllApiPages");
                cb.AddLine("{");
                cb.IndentLevel++;
                cb.AddLine("private Bunit.TestContext ctx;");
                cb.AddLine();
                cb.AddLine("[SetUp]");
                cb.AddLine("public void Setup()");
                cb.AddLine("{");
                cb.IndentLevel++;
                cb.AddLine("ctx = new Bunit.TestContext();");
                cb.AddLine("ctx.JSInterop.Mode = JSRuntimeMode.Loose;");
                cb.AddLine("ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());");
                cb.AddLine("ctx.Services.AddSingleton<IDialogService>(new DialogService());");
                cb.AddLine("ctx.Services.AddSingleton<ISnackbar>(new SnackbarService());");
                cb.AddLine("ctx.Services.AddSingleton<IResizeListenerService>(new MockResizeListenerService());");
                cb.AddLine("ctx.Services.AddSingleton<IHeadElementHelper>(new MockHeadElementHelper());");
                cb.AddLine("ctx.Services.AddSingleton<IBrowserWindowSizeProvider>(new MockBrowserWindowSizeProvider());");
                cb.AddLine("ctx.Services.AddScoped(sp => new HttpClient());");
                cb.IndentLevel--;
                cb.AddLine("}");
                cb.AddLine();
                cb.AddLine("[TearDown]");
                cb.AddLine("public void TearDown() => ctx.Dispose();");
                cb.AddLine();
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