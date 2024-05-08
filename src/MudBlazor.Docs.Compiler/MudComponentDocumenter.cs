// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Docs.Compiler;

/// <summary>
/// Represents a generator of HTML documentation based on XML documentation files.
/// </summary>
/// <remarks>
/// This class documents a MudBlazor assembly by documenting types inheriting from <see cref="MudComponentBase"/>, including properties 
/// decorated with <see cref="ParameterAttribute"/>, as well as any public type mentioned in a public or protected method parameter. This
/// approach should automatically create "public-facing documentation" as more components and types are added.
/// </remarks>
public partial class MudComponentDocumenter(Assembly assembly)
{
    /// <summary>
    /// The types in the assembly.
    /// </summary>
    public SortedDictionary<string, Type> Types { get; set; } = [];

    /// <summary>
    /// The generated documentation for types.
    /// </summary>
    public SortedDictionary<string, DocumentedType> DocumentedTypes { get; private set; } = [];

    /// <summary>
    /// The generated documentation for the most important types.
    /// </summary>
    public SortedDictionary<string, DocumentedType> DocumentedCriticalTypes { get; private set; }

    /// <summary>
    /// The generated documentation for properties.
    /// </summary>
    public SortedDictionary<string, DocumentedProperty> DocumentedProperties { get; private set; } = [];

    /// <summary>
    /// The generated documentation for methods.
    /// </summary>
    public SortedDictionary<string, DocumentedMethod> DocumentedMethods { get; private set; } = [];

    /// <summary>
    /// (For documentation coverage metrics), the most important types to have well documented.
    /// </summary>
    public List<string> CriticalTypes =
    [
        // Core MudBlazor Components (base classes aren't necessary)
        "MudAlert",
        "MudAppBar",
        "MudAutocomplete`1",
        "MudAvatar",
        "MudAvatarGroup",
        "MudBadge",
        "MudBreadcrumbs",
        "MudBreakpointProvider",
        "MudButton",
        "MudButtonGroup",
        "MudCard",
        "MudCardActions",
        "MudCardContent",
        "MudCardHeader",
        "MudCardMedia",
        "MudCarousel`1",
        "MudCarouselItem",
        "MudChart",
        "MudCheckBox`1",
        "MudChip`1",
        "MudChipSet`1",
        "MudCollapse",
        "MudColor",
        "MudColorPicker",
        "MudContainer",
        "MudDataGrid`1",
        "MudDataGridPager`1",
        "MudDatePicker",
        "MudDateRangePicker",
        "MudDialog",
        "MudDialogInstance",
        "MudDialogProvider",
        "MudDivider",
        "MudDragAndDropIndexChangedEventArgs",
        "MudDragAndDropItemTransaction`1",
        "MudDragAndDropTransactionFinishedEventArgs`1",
        "MudDrawer",
        "MudDrawerContainer",
        "MudDrawerHeader",
        "MudDropContainer`1",
        "MudDropZone`1",
        "MudDynamicDropItem`1",
        "MudDynamicTabs",
        "MudElement",
        "MudExpansionPanel",
        "MudExpansionPanels",
        "MudFab",
        "MudField",
        "MudFileUpload`1",
        "MudFlexBreak",
        "MudFocusTrap",
        "MudForm",
        "MudGlobal",
        "MudGrid",
        "MudHidden",
        "MudHighlighter",
        "MudIcon",
        "MudIconButton",
        "MudImage",
        "MudInput`1",
        "MudInputAdornment",
        "MudInputControl",
        "MudInputLabel",
        "MudInputString",
        "MudItem",
        "MudItemDropInfo`1",
        "MudLayout",
        "MudLink",
        "MudList`1",
        "MudListItem`1",
        "MudListSubheader",
        "MudLocalizer",
        "MudMainContent",
        "MudMask",
        "MudMenu",
        "MudMenuItem",
        "MudMessageBox",
        "MudNavGroup",
        "MudNavLink",
        "MudNavMenu",
        "MudNumericField`1",
        "MudOverlay",
        "MudPageContentNavigation",
        "MudPageContentSection",
        "MudPagination",
        "MudPaper",
        "MudPicker`1",
        "MudPickerContent",
        "MudPickerToolbar",
        "MudPopover",
        "MudPopoverBase",
        "MudPopoverHandler",
        "MudPopoverProvider",
        "MudProgressCircular",
        "MudProgressLinear",
        "MudRadio`1",
        "MudRadioGroup`1",
        "MudRangeInput`1",
        "MudRating",
        "MudRatingItem",
        "MudRender",
        "MudRTLProvider",
        "MudScrollToTop",
        "MudSelect`1",
        "MudSelectItem`1",
        "MudSimpleTable",
        "MudSkeleton",
        "MudSlider`1",
        "MudSnackbarProvider",
        "MudSortableColumn`2",
        "MudSpacer",
        "MudSparkLine",
        "MudStack",
        "MudSwipeArea",
        "MudSwitch`1",
        "MudTable`1",
        "MudTableGroupRow`1",
        "MudTablePager",
        "MudTableSortLabel`1",
        "MudTabPanel",
        "MudTabs",
        "MudTd",
        "MudText",
        "MudTextField`1",
        "MudTFootRow",
        "MudTh",
        "MudTHeadRow",
        "MudTheme",
        "MudThemeProvider",
        "MudTimeline",
        "MudTimelineItem",
        "MudTimePicker",
        "MudToggleGroup`1",
        "MudToggleIconButton",
        "MudToggleItem`1",
        "MudToolBar",
        "MudTooltip",
        "MudTr",
        "MudTreeView`1",
        "MudTreeViewItem`1",
        "MudTreeViewItemToggleButton",
        "MudVirtualize`1",
        // Charts
        "Bar",
        "Donut",
        "Line",
        "Pie",
        "StackedBar",
        "ChartOptions",
        "ChartSeries",
        // Common enumerations
        "Adornment",
        "Align",
        "Anchor",
        "ButtonType",
        "Color",
        "ColorPickerMode",
        "ColorPickerView",
        "Direction",
        "DrawerClipMode",
        "DrawerVariant",
        "Edge",
        "HorizontalAlignment",
        "InputMode",
        "InputType",
        "Justify",
        "Margin",
        "MaxWidth",
        "Orientation",
        "Origin",
        "PickerVariant",
        "Placement",
        "Position",
        "Severity",
        "Size",
        "SkeletonType",
        "SortDirection",
        "SortMode",
        "TimelineAlign",
        "TimelinePosition",
        "Transition",
        "Typo",
        "Variant",
        "Width",
        "Wrap",
        // Commonly classes for components
        "TableState",
        "BreadcrumbItem",
    ];

    /// <summary>
    /// Any methods to exclude from documentation.
    /// </summary>
    private List<string> ExcludedMethods =
    [
        // Object methods
        "ToString",
        "Equals",
        "MemberwiseClone",
        // Blazor component methods
        "OnInitialized",
        "OnInitializedAsync",
        "OnParametersSet",
        "OnParametersSetAsync",
        "OnAfterRender",
        "OnAfterRenderAsync",
        "StateHasChanged",
        "ShouldRender",
        "BuildRenderTree",
        "InvokeAsync",
        // Dispose methods
        "Dispose",
        "DisposeAsync",
        "Finalize",
        // Internal MudBlazor methods
        "DispatchExceptionAsync",
        "CreateRegisterScope",
        "DetectIllegalRazorParameters"
    ];

    /// <summary>
    /// The XML documentation elements and their HTML equivalents.
    /// </summary>
    private static Dictionary<string, string> XmlToHtmlElements = new()
    {
        { "<c>", "<code class=\"docs-code docs-code-primary\">" },
        { "</c>", "</code>" },
        { "<para>", "<p>" },
        { "</para>", "</p>" }
    };

    /// <summary>
    /// Generates HTML documentation for all assemblies.
    /// </summary>
    public bool Execute()
    {
        AddTypesToDocument();
        MergeXmlDocumentation();
        ExportGeneratedCode();
        CalculateDocumentationCoverage();
        return true;
    }

    /// <summary>
    /// Adds an empty documented type for each MudBlazor component and related public type.
    /// </summary>
    public void AddTypesToDocument()
    {
        // Look for any public types
        Types = new(assembly.GetTypes().Where(type => type.IsPublic).ToDictionary(r => r.Name, v => v));

        var y = string.Join("\", \"", Types.Where(t => t.Value.Name.StartsWith("Mud")).Select(type => type.Value.Name));

        foreach (var type in Types)
        {
            var documentedType = AddTypeToDocument(type.Value);
            AddPropertiesToDocument(type.Value, documentedType);
            AddMethodsToDocument(type.Value, documentedType);
            // AddEventsToDocument(type);
        }
    }

    /// <summary>
    /// Adds the specified type and any related public types.
    /// </summary>
    /// <param name="type">The type to add.</param>
    public DocumentedType AddTypeToDocument(Type type)
    {
        // Is this a non-MudBlazor type?  Or is it already documented?  If so, skip it
        if (type.FullName == null || !type.FullName.StartsWith("MudBlazor"))
        {
            return null;
        }
        // Is the type already documented?
        if (!DocumentedTypes.TryGetValue(type.FullName, out var documentedType))
        {
            // No.
            documentedType = new DocumentedType()
            {
                BaseType = type.BaseType,
                IsPublic = type.IsPublic,
                IsAbstract = type.IsNestedFamORAssem,
                Key = type.FullName,
                Name = type.Name,
                Type = type,
            };
        }

        // Add the populated type
        DocumentedTypes.Add(type.FullName, documentedType);

        return documentedType;
    }

    /// <summary>
    /// Adds public properties for the specified type.
    /// </summary>
    /// <param name="type"></param>
    public void AddPropertiesToDocument(Type type, DocumentedType documentedType)
    {
        // Look for public properties 
        var properties = type.GetProperties().ToList();
        // Add protected methods
        properties.AddRange(type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic));
        // Remove duplicates
        properties = properties.DistinctBy(property => property.Name).ToList();
        // Go through each property
        foreach (var property in properties)
        {
            var category = property.GetCustomAttribute<CategoryAttribute>();
            var blazorParameter = property.GetCustomAttribute<ParameterAttribute>();
            var key = GetTypeFullName(property);

            // Has this property been documented before?
            if (!DocumentedProperties.TryGetValue(key, out var documentedProperty))
            {
                // No.
                documentedProperty = new DocumentedProperty()
                {
                    Category = category?.Name,
                    DeclaringType = property.DeclaringType,
                    DeclaringTypeName = property.DeclaringType.Name,
                    DeclaringTypeFullName = property.DeclaringType.FullName,
                    IsPublic = property.GetMethod.IsPublic,
                    IsProtected = property.GetMethod.IsFamily,
                    IsParameter = blazorParameter != null,
                    Key = key,
                    Name = property.Name,
                    Order = category?.Order,
                    PropertyType = property.PropertyType,
                    PropertyTypeName = property.PropertyType.Name,
                    PropertyTypeFullName = property.PropertyType.FullName
                };
                DocumentedProperties.Add(key, documentedProperty);
            }

            documentedType.Properties.Add(documentedProperty.Key, documentedProperty);
        }
    }

    /// <summary>
    /// Gets the full name of the property's declaring type.
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public string GetTypeFullName(PropertyInfo property)
    {
        // Is a full name already given?
        if (property.DeclaringType.FullName != null)
        {
            return $"{property.DeclaringType.FullName}.{property.Name}";
        }
        // Is there a type by name?
        else if (Types.TryGetValue(property.DeclaringType.Name, out var type))
        {
            return $"{type.FullName}.{property.Name}";
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Adds methods the specified documented type.
    /// </summary>
    /// <param name="type">The type to find methods for.</param>
    /// <param name="documentedType">The documentation for the type.</param>
    public void AddMethodsToDocument(Type type, DocumentedType documentedType)
    {
        // Look for public methods
        var methods = type.GetMethods(BindingFlags.Public).ToList();
        // Add protected methods
        methods.AddRange(type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(method => method.IsFamily));
        methods = methods
            // Remove duplicates
            .DistinctBy(method => method.Name)
            .Where(method =>
                // Exclude getter and setter methods
                !method.Name.StartsWith("get_")
                && !method.Name.StartsWith("set_")
                // Exclude inherited .NET methods
                && !method.Name.StartsWith("Microsoft")
                && !method.Name.StartsWith("System")
                // Exclude internal MudBlazor interfaces
                && !method.Name.StartsWith("MudBlazor.Interfaces")
                && !method.Name.StartsWith("MudBlazor.State")
                // Exclude common .NET and Blazor methods
                && !ExcludedMethods.Contains(method.Name))
                .ToList();
        // Look for methods and add related types
        foreach (var method in methods)
        {
            var key = $"{method.ReflectedType.FullName}.{method.Name}";

            // Has this been documented before?
            if (!DocumentedMethods.TryGetValue(key, out var documentedMethod))
            {
                // No.
                documentedMethod = new DocumentedMethod()
                {
                    Key = key,
                    Name = method.Name,
                    IsPublic = method.IsPublic,
                    IsProtected = method.IsFamily,
                    ReturnType = method.ReturnType,
                    ReturnTypeName = method.ReturnType.Name,
                    ReturnTypeFullName = method.ReturnType.FullName,
                };
                // Reach out and document types mentioned in these methods
                foreach (var parameter in method.GetParameters())
                {
                    var documentedParameter = new DocumentedParameter()
                    {
                        Name = parameter.Name,
                        Type = parameter.ParameterType,
                        TypeFullName = parameter.ParameterType.FullName,
                        TypeName = parameter.ParameterType.Name
                    };
                    documentedMethod.Parameters.Add(parameter.Name, documentedParameter);
                }
                // Add to the list
                DocumentedMethods.Add(key, documentedMethod);
            }
            else
            {
                Debugger.Break();
            }
            // Add the method to the type
            documentedType.Methods.Add(key, documentedMethod);
        }
    }

    /// <summary>
    /// Merges XML documentation with existing documentation types.
    /// </summary>
    /// <exception cref="FileNotFoundException"></exception>
    public void MergeXmlDocumentation()
    {
        // Load the XML documentation file
        var path = assembly.Location.Replace(".dll", ".xml", StringComparison.OrdinalIgnoreCase);
        using var reader = new XmlTextReader(path);
        reader.WhitespaceHandling = WhitespaceHandling.None;
        reader.DtdProcessing = DtdProcessing.Ignore;
        // Read each "<member name=...>" element
        while (reader.ReadToFollowing("member"))
        {
            var memberTypeAndName = reader.GetAttribute("name").Split(":");
            var content = reader.ReadInnerXml();
            switch (memberTypeAndName[0])
            {
                case "T": // Type
                    DocumentType(memberTypeAndName[1], content);
                    break;
                case "P": // Property
                    DocumentProperty(memberTypeAndName[1], content);
                    break;
            }
        }
    }

    public List<string> UnresolvedTypes { get; set; } = [];
    public List<string> UnresolvedProperties { get; set; } = [];

    /// <summary>
    /// Adds HTML documentation for the specified type.
    /// </summary>
    /// <param name="memberFullName">The namespace and class of the member.</param>
    /// <param name="xmlContent">The raw XML documentation for the member.</param>
    public void DocumentType(string memberFullName, string xmlContent)
    {
        if (DocumentedTypes.TryGetValue(memberFullName, out var documentedType))
        {
            documentedType.Remarks = GetRemarks(xmlContent);
            documentedType.Summary = GetSummary(xmlContent);
        }
        else
        {
            UnresolvedTypes.Add(memberFullName);
        }
    }

    /// <summary>
    /// Adds HTML documentation for the specified property.
    /// </summary>
    /// <param name="memberFullName">The namespace and class of the member.</param>
    /// <param name="xmlContent">The raw XML documentation for the member.</param>
    public void DocumentProperty(string memberFullName, string xmlContent)
    {
        // Get the documented type and property
        if (DocumentedProperties.TryGetValue(memberFullName, out var documentedProperty))
        {
            documentedProperty.Remarks = GetRemarks(xmlContent);
            documentedProperty.Summary = GetSummary(xmlContent);
        }
        else
        {
            UnresolvedProperties.Add(memberFullName);
        }
    }

    /// <summary>
    /// Gets the content of the "summary" element as HTML.
    /// </summary>
    /// <param name="xml">The member XML to search.</param>
    /// <returns>The HTML content of the member.</returns>
    public static string GetSummary(string xml)
    {
        var summary = SummaryRegEx().Match(xml).Groups.GetValueOrDefault("1");
        var content = summary?.Value;
        return ToHtml(content);
    }

    /// <summary>
    /// Gets the content of the "remarks" element as HTML.
    /// </summary>
    /// <param name="xml">The member XML to search.</param>
    /// <returns>The HTML content of the member.</returns>
    public static string GetRemarks(string xml)
    {
        var remarks = RemarksRegEx().Match(xml).Groups.GetValueOrDefault("1");
        var content = remarks?.Value;
        return ToHtml(content);
    }

    /// <summary>
    /// Gets the specified XML documentation string as HTML.
    /// </summary>
    /// <param name="xml">The XML content to convert.</param>
    /// <returns></returns>
    public static string ToHtml(string xml)
    {
        // Anything to do?
        if (string.IsNullOrEmpty(xml)) { return null; }
        // Convert common XML documentation elements to HTML
        foreach (var pair in XmlToHtmlElements)
        {
            xml = xml.Replace(pair.Key, pair.Value, StringComparison.OrdinalIgnoreCase);
        }
        return xml;
    }

    /// <summary>
    /// Serializes all documentation to the MudBlazor.Docs "Generated" folder.
    /// </summary>
    public void ExportGeneratedCode()
    {
        using var writer = File.CreateText(Paths.ApiDocumentationFilePath);

        writer.WriteLine("//-----------------------------------------------------------------------");
        writer.WriteLine("// This file is autogenerated by MudBlazor.Docs.Compiler");
        writer.WriteLine("// Any changes to this file will be overwritten on build");
        writer.WriteLine("// <auto-generated />");
        writer.WriteLine("//-----------------------------------------------------------------------");
        writer.WriteLine();
        writer.WriteLine("namespace MudBlazor.Docs.Models;");
        writer.WriteLine();
        writer.WriteLine("[System.CodeDom.Compiler.GeneratedCodeAttribute(\"MudBlazor.Docs.Compiler\", \"0.0.0.0\")]");
        writer.WriteLine("public static partial class ApiDocumentation");
        writer.WriteLine("{");
        writer.WriteLine("\tstatic ApiDocumentation()");
        writer.WriteLine("\t{");

        foreach (var type in DocumentedTypes)
        {
            writer.WriteLine($"\t\tTypes.Add(new()");
            writer.WriteLine("\t\t{");
            writer.WriteLine($"\t\t\tName = \"{type.Value.Name}\",");
            writer.WriteLine($"\t\t\tSummary = \"{Escape(type.Value.Summary)}\",");
            writer.WriteLine($"\t\t\tRemarks = \"{Escape(type.Value.Remarks)}\",");
            writer.WriteLine("\t\t\tProperties =");
            writer.WriteLine("\t\t\t[");

            foreach (var property in type.Value.Properties)
            {
                writer.WriteLine("\t\t\t\tnew()");
                writer.WriteLine("\t\t\t\t{");
                writer.WriteLine($"\t\t\t\t\tName = \"{property.Value.Name}\",");
                writer.WriteLine($"\t\t\t\t\tSummary = \"{Escape(property.Value.Summary)}\",");
                writer.WriteLine($"\t\t\t\t\tRemarks = \"{Escape(property.Value.Remarks)}\"");
                writer.WriteLine("\t\t\t\t},");
            }

            writer.WriteLine("\t\t\t]");
            writer.WriteLine("\t\t});");
        }

        writer.WriteLine("\t}");
        writer.WriteLine("}");
    }

    /// <summary>
    /// Formats a string for use in C# code.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public string Escape(string code)
    {
        return code?.Replace("\"", "\\\"");
    }

    /// <summary>
    /// Calculates how thoroughly types are documented.
    /// </summary>
    public void CalculateDocumentationCoverage()
    {
        var totalCriticalTypes = DocumentedTypes.Count(type => CriticalTypes.Contains(type.Value.Name));
        var totalCriticalDocumentedTypes = DocumentedTypes.Count(type => CriticalTypes.Contains(type.Value.Name) && !string.IsNullOrEmpty(type.Value.Summary));
        var totalTypes = DocumentedTypes.Count;
        var totalProperties = DocumentedProperties.Count;
        var totalMethods = DocumentedMethods.Count;
        var totalUnresolvedTypes = UnresolvedTypes.Count;
        var totalUnresolvedProperties = UnresolvedProperties.Count;
        var summarizedTypes = DocumentedTypes.Count(type => !string.IsNullOrEmpty(type.Value.Summary));
        var summarizedProperties = DocumentedProperties.Count(property => !string.IsNullOrEmpty(property.Value.Summary));
        var summarizedMethods = DocumentedMethods.Count(method => !string.IsNullOrEmpty(method.Value.Summary));
        var criticalTypeCoverage = totalCriticalDocumentedTypes / (double)totalCriticalTypes;
        var typeCoverage = summarizedTypes / (double)totalTypes;
        var methodCoverage = summarizedMethods / (double)totalMethods;
        var propertyCoverage = summarizedProperties / (double)totalProperties;

        Console.WriteLine("Summary of MudBlazor XML Documentation:");
        Console.WriteLine();
        Console.WriteLine($"Core Types: {totalCriticalDocumentedTypes} of {totalCriticalTypes} core types ({criticalTypeCoverage:P2})");
        Console.WriteLine($"Types:      {summarizedTypes} of {totalTypes} ({typeCoverage:P2}) other types");
        if (UnresolvedTypes.Count > 0)
        {
            Console.WriteLine($"            {UnresolvedTypes.Count} types have XML documentation which couldn't be matched to a type.");
        }
        Console.WriteLine($"Properties: {summarizedProperties}/{totalProperties} ({propertyCoverage:P2}) ({UnresolvedProperties.Count} ignored)");
        if (UnresolvedProperties.Count > 0)
        {
            Console.WriteLine($"            {UnresolvedProperties.Count} properties have XML documentation which couldn't be matched to a type's property.");
        }
        Console.WriteLine($"Methods:    {summarizedMethods}/{totalMethods} ({methodCoverage:P2})");
    }

    /// <summary>
    /// The regular expression used to extract XML documentation summaries.
    /// </summary>
    [GeneratedRegex(@"<summary>\s*([ \S]*)\s*<\/summary>")]
    private static partial Regex SummaryRegEx();

    /// <summary>
    /// The regular expression used to extract XML documentation remarks.
    /// </summary>
    [GeneratedRegex(@"<remarks>\s*([ \S]*)\s*<\/remarks>")]
    private static partial Regex RemarksRegEx();

    /// <summary>
    /// The regular expression used to extract XML documentation return values.
    /// </summary>
    [GeneratedRegex(@"<returns>\s*([ \S]*)\s*<\/returns>")]
    private static partial Regex ReturnsRegEx();
}
