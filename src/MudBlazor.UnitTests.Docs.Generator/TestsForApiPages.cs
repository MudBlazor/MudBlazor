using System.Text.RegularExpressions;
using MudBlazor.Charts;

namespace MudBlazor.UnitTests.Docs.Generator;

public partial class TestsForApiPages
{
    /// <summary>
    /// The current production links to API documentation.
    /// </summary>
    private readonly string[] _legacyApiAddresses = [
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
    /// A list of types which have an example page ("/component/*") to link to.
    /// </summary>
    /// <remarks>
    /// This list should match the types mentioned in the <c>MenuService</c> class in MudBlazor.Docs.
    /// </remarks>
    public List<Type> TypesWithExamples =
    [
        typeof(MudContainer), typeof(MudGrid), typeof(MudItem), typeof(MudHidden),  typeof(MudBreakpointProvider), typeof(MudChip<object>), typeof(MudChipSet<object>),
        typeof(MudBadge), typeof(MudAppBar), typeof(MudDrawer), typeof(MudDrawerHeader), typeof(MudDrawerContainer), typeof(MudDropZone<object>), typeof(MudDropContainer<object>),
        typeof(MudDynamicDropItem<object>), typeof(MudLink), typeof(MudMenu), typeof(MudMenuItem), typeof(MudMessageBox), typeof(MudNavMenu), typeof(MudNavLink), typeof(MudNavGroup),
        typeof(MudTabs), typeof(MudTabPanel), typeof(MudDynamicTabs), typeof(MudProgressCircular), typeof(MudProgressLinear), typeof(MudDialog), typeof(MudDialogContainer),
        typeof(MudDialogProvider), typeof(SnackbarService), typeof(MudSnackbarProvider), typeof(MudSnackbarElement), typeof(MudAvatar), typeof(MudAvatarGroup),
        typeof(MudAlert), typeof(MudCard), typeof(MudCardActions), typeof(MudCardContent), typeof(MudCardHeader), typeof(MudCardMedia), typeof(MudDivider),
        typeof(MudExpansionPanels), typeof(MudExpansionPanel), typeof(MudImage), typeof(MudIcon), typeof(MudList<object>), typeof(MudListItem<object>), typeof(MudListSubheader),
        typeof(MudPaper), typeof(MudRating), typeof(MudRatingItem), typeof(MudSkeleton), typeof(MudTableBase), typeof(MudTable<object>), typeof(MudTablePager),
        typeof(MudTableGroupRow<object>), typeof(MudTableSortLabel<object>), typeof(MudTd), typeof(MudTh), typeof(MudTr), typeof(MudTFootRow), typeof(MudTHeadRow),
        typeof(MudDataGrid<object>), typeof(Column<object>), typeof(FilterHeaderCell<object>), typeof(FooterCell<object>), typeof(HeaderCell<object>), typeof(HierarchyColumn<object>), typeof(MudDataGridPager<object>),
        typeof(TemplateColumn<object>), typeof(MudSimpleTable), typeof(MudTooltip), typeof(MudText), typeof(MudOverlay), typeof(MudHighlighter), typeof(MudElement),
        typeof(MudFocusTrap), typeof(MudTreeView<object>), typeof(MudTreeViewItem<object>), typeof(MudTreeViewItemToggleButton),  typeof(MudBreadcrumbs), typeof(MudScrollToTop),
        typeof(MudPopover),  typeof(MudSwipeArea), typeof(MudToolBar), typeof(MudCarousel<object>), typeof(MudCarouselItem), typeof(MudTimeline), typeof(MudTimelineItem),
        typeof(MudPagination), typeof(MudStack), typeof(MudSpacer), typeof(MudCollapse), typeof(MudStepper), typeof(MudStep), typeof(MudRadio<object>), typeof(MudRadioGroup<object>),
        typeof(MudCheckBox<object>), typeof(MudSelect<object>), typeof(MudSelectItem<object>), typeof(MudSlider<int>), typeof(MudSwitch<object>), typeof(MudTextField<object>),
        typeof(MudNumericField<object>), typeof(MudForm), typeof(MudAutocomplete<object>), typeof(MudField), typeof(MudFileUpload<object>), typeof(MudToggleGroup<object>), typeof(MudToggleItem<object>),
        typeof(MudDatePicker), typeof(MudDateRangePicker), typeof(MudTimePicker), typeof(MudColorPicker),  typeof(MudButton),  typeof(MudButtonGroup), typeof(MudIconButton),
        typeof(MudToggleIconButton), typeof(MudFab), typeof(ChartOptions), typeof(Donut), typeof(Line), typeof(Legend), typeof(Pie), typeof(Bar), typeof(HeatMap),typeof(StackedBar),
        typeof(TimeSeries), typeof(MudTimeSeriesChartBase), typeof(MudTimeSeriesChart)
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
            cb.AddLine("using Bunit;");
            cb.AddLine("using FluentAssertions;");
            cb.AddLine("using Microsoft.AspNetCore.Components;");
            cb.AddLine("using Microsoft.Extensions.DependencyInjection;");
            cb.AddLine("using MudBlazor.Docs.Pages.Api;");
            cb.AddLine("using MudBlazor.Docs.Services;");
            cb.AddLine("using NUnit.Framework;");
            cb.AddLine();
            cb.AddLine("namespace MudBlazor.UnitTests.Docs.Generated");
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
        var mudBlazorAssembly = typeof(_Imports).Assembly;
        var mudBlazorComponents = mudBlazorAssembly.GetTypes()
            .Where(type =>
                // Include public types
                type.IsPublic
                // ... which aren't excluded
                && !IsExcluded(type)
                // ... which aren't interfaces
                && !type.IsInterface
                // ... which aren't source generators
                && !type.Name.Contains("SourceGenerator")
                // ... which aren't extension classes
                && !type.Name.Contains("Extensions")
                // ... which aren't clone strategies
                && !type.Name.Contains("SystemTextJson"))
            .ToList();
        foreach (var type in mudBlazorComponents)
        {
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
            cb.AddLine(@$"comp.Markup.Should().NotContain(""Sorry, the type {type.Name} was not found"");");
            // Should there be a link to the example page?
            if (TypesWithExamples.Exists(exampleType => exampleType.Name == type.Name))
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
        foreach (var url in _legacyApiAddresses)
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

    /// <summary>
    /// Gets whether a type is excluded from documentation.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>When <c>true</c>, the type is excluded from documentation.</returns>
    private static bool IsExcluded(Type type)
    {
        if (ExcludedTypes.Contains(type.Name))
        {
            return true;
        }
        if (type.FullName != null && ExcludedTypes.Contains(type.FullName))
        {
            return true;
        }
        if (type.FullName != null && ExcludedTypes.Any(type.FullName.StartsWith))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Any types to exclude from documentation.
    /// </summary>
    public static List<string> ExcludedTypes { get; } =
    [
        "ActivatableCallback",
        "AbstractLocalizationInterceptor",
        "CloneableCloneStrategy`1",
        "CssBuilder",
        "MudBlazor._Imports",
        "MudBlazor.CategoryAttribute",
        "MudBlazor.CategoryTypes",
        "MudBlazor.CategoryTypes+",
        "MudBlazor.Colors",
        "MudBlazor.Colors+",
        "MudBlazor.Icons",
        "MudBlazor.Icons+",
        "MudBlazor.LabelAttribute",
        "MudBlazor.Resources.LanguageResource",
        "object",
        "string"
    ];
}
