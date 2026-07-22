using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MudBlazor.Docs.Compiler
{
#nullable enable
    public partial class TestsForApiPages
    {
        /// <summary>
        /// The current production links to API documentation.
        /// </summary>
        public string[] LegacyApiAddresses = [
            // available from the main menu in "API" group
            "api/alert",
            "api/appbar",
            "api/autocomplete",
            "api/avatar",
            "api/badge",
            "api/barchart",
            "api/breadcrumbs",
            "api/breakpointprovider",
            "api/button",
            "api/buttonfab",
            "api/buttongroup",
            "api/card",
            "api/carousel",
            "api/checkbox",
            "api/chips",
            "api/chipset",
            "api/colorpicker",
            "api/container",
            "api/datagrid",
            "api/datepicker",
            "api/dialog",
            "api/divider",
            "api/donutchart",
            "api/drawer",
            "api/element",
            "api/expansionpanels",
            "api/field",
            "api/fileuploader",
            "api/focustrap",
            "api/form",
            "api/grid",
            "api/hidden",
            "api/highlighter",
            "api/iconbutton",
            "api/icons",
            "api/linechart",
            "api/link",
            "api/list",
            "api/menu",
            "api/messagebox",
            "api/navmenu",
            "api/numericfield",
            "api/overlay",
            "api/pagination",
            "api/paper",
            "api/piechart",
            "api/popover",
            "api/progress",
            "api/radio",
            "api/rating",
            "api/scrolltotop",
            "api/select",
            "api/simpletable",
            "api/skeleton",
            "api/slider",
            "api/snackbar",
            "api/swipearea",
            "api/switch",
            "api/table",
            "api/tabs",
            "api/textfield",
            "api/timepicker",
            "api/timeline",
            "api/toggleiconbutton",
            "api/toolbar",
            "api/tooltip",
            "api/treeview",
            "api/typography",

            // subelements - available from components/* pages
            "api/drawerheader",
            "api/drawercontainer",
            "api/navlink",
            "api/navgroup",
            "api/item",
            "api/dynamictabs",
            "api/expansionpanel",
            "api/timelineitem",
            "api/cardactions",
            "api/cardcontent",
            "api/cardheader",
            "api/cardmedia",
            "api/treeviewitem",
            "api/treeviewitemtogglebutton",
            "api/listitem",
            "api/listsubheader",
            "api/carouselitem",
            "api/dialoginstance",
            "api/dialogprovider",
            "api/avatargroup",
            "api/menuitem",
            "api/radiogroup",
            "api/selectitem",
            "api/ratingitem",

            // API pages not linked from the documentation web site, but still available through the URL
            "api/THeadRow",
            "api/TFootRow",
            "api/Tr",
            "api/Th",
            "api/Td",
            "api/TableGroupRow",
            "api/TableSortLabel",
            "api/TablePager",
            "api/InputLabel",
            "api/InputControl",
            "api/Input",
            "api/RangeInput",
            "api/MainContent",
            "api/DateRangePicker",
            "api/Collapse",
            "api/PageContentNavigation",
            "api/RTLProvider",
            "api/SnackbarElement",
            "api/SparkLine",
        ];

        /// <summary>
        /// Ensures that an API page is available for each MudBlazor component.
        /// </summary>
        public bool Execute()
        {
            var success = true;
            try
            {
                Directory.CreateDirectory(Paths.TestDirPath);

                var currentCode = string.Empty;
                if (File.Exists(Paths.ApiPageTestsFilePath))
                {
                    currentCode = File.ReadAllText(Paths.ApiPageTestsFilePath);
                }

                var cb = new CodeBuilder();

                cb.AddHeader();
                cb.AddLine("using System.Linq;");
                cb.AddLine("using System.Threading.Tasks;");
                cb.AddLine("using Bunit;");
                cb.AddLine("using FluentAssertions;");
                cb.AddLine("using Microsoft.AspNetCore.Components;");
                cb.AddLine("using Microsoft.Extensions.DependencyInjection;");
                cb.AddLine("using MudBlazor.Docs.Pages.Api;");
                cb.AddLine("using MudBlazor.Docs.Services;");
                cb.AddLine("using MudBlazor.UnitTests.Mocks;");
                cb.AddLine("using NUnit.Framework;");
                cb.AddLine();
                cb.AddLine("namespace MudBlazor.UnitTests.Components");
                cb.AddLine("{");
                cb.IndentLevel++;
                cb.AddLine("// These tests just check all the API pages to see if they throw any exceptions");
                cb.AddLine("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"MudBlazor.Docs.Compiler\", \"0.0.0.0\")]");
                cb.AddLine("public partial class ApiDocsTests");
                cb.AddLine("{");
                cb.IndentLevel++;

                WritePublicTypeTests(cb);
                WriteLegacyApiLinkTests(cb);

                cb.IndentLevel--;
                cb.AddLine("}");
                cb.IndentLevel--;
                cb.AddLine("}");

                if (currentCode != cb.ToString())
                {
                    File.WriteAllText(Paths.ApiPageTestsFilePath, cb.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($@"Error generating {Paths.ApiPageTestsFilePath} : {e.Message}");
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Creates tests for all public MudBlazor types.
        /// </summary>
        public void WritePublicTypeTests(CodeBuilder cb)
        {
            var mudBlazorComponents = typeof(_Imports).Assembly.GetTypes().Where(type => type.IsPublic);
            foreach (var type in mudBlazorComponents)
            {
                if (ApiDocumentationBuilder.IsExcluded(type))
                {
                    continue;
                }

                // Skip MudBlazor.Color and MudBlazor.Input types
                if (type.Name == "Color" || type.Name == "Input")
                {
                    continue;
                }

                cb.AddLine("[Test]");
                cb.AddLine($"public async Task {type.Name.Replace("`", "")}_API_TestAsync()");
                cb.AddLine("{");
                cb.IndentLevel++;
                // Create Api.razor with a type
                cb.AddLine(@$"ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager(""https://localhost:2112/"", ""https://localhost:2112/components/{type.Name}""));");
                cb.AddLine(@$"var comp = ctx.RenderComponent<Api>(ComponentParameter.CreateParameter(""TypeName"", ""{type.Name}""));");
                cb.AddLine(@$"await ctx.Services.GetService<IRenderQueueService>().WaitUntilEmpty();");
                // Make sure docs for the type were actually found
                cb.AddLine(@$"comp.Markup.Should().NotContain(""Sorry, the type {type} was not found"");");
                // Is this a component?
                if (type.IsSubclassOf(typeof(MudComponentBase)))
                {
                    // Yes.  Check for the example link
                    cb.AddLine(@$"var exampleLink = comp.FindComponents<MudLink>().FirstOrDefault(link => link.Instance.Href != null && link.Instance.Href.StartsWith(""/component""));");
                    cb.AddLine(@$"exampleLink.Should().NotBeNull();");
                }
                cb.IndentLevel--;
                cb.AddLine("}");
            }
        }

        /// <summary>
        /// Creates tests for existing API links (for backwards compatibility).
        /// </summary>
        public void WriteLegacyApiLinkTests(CodeBuilder cb)
        {
            foreach (var url in LegacyApiAddresses)
            {
                var component = url.Replace("api/", "");

                cb.AddLine("[Test]");
                cb.AddLine($"public async Task {component.Replace("/", "_")}_Legacy_API_TestAsync()");
                cb.AddLine("{");
                cb.IndentLevel++;
                // Create Api.razor with a type
                cb.AddLine(@$"ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager(""https://localhost:2112/"", ""https://localhost:2112/components/{url}""));");
                cb.AddLine(@$"var comp = ctx.RenderComponent<Api>(ComponentParameter.CreateParameter(""TypeName"", ""{component}""));");
                cb.AddLine(@$"await ctx.Services.GetService<IRenderQueueService>().WaitUntilEmpty();");
                // Make sure docs for the type were actually found
                cb.AddLine(@$"comp.Markup.Should().NotContain(""Sorry, the type {component} was not found"");");
                cb.IndentLevel--;
                cb.AddLine("}");
            }
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
