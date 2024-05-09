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
/// <para>
/// This class documents the MudBlazor assembly, including all public types, properties, methods, events, and fields.  Inherited
/// members are supported, as well as "see cref" links.  Once all documentation has been loaded, several types are made available 
/// to the <c>MudBlazor.Docs</c> such as <see cref="DocumentedType"/>, <see cref="DocumentedMethod"/>, <see cref="DocumentedProperty"/>,
/// <see cref="DocumentedEvent"/>, and <see cref="DocumentedField"/>, in a strongly typed manner. 
/// </para>
/// <para>
/// This class also produces metrics which can be used during DevOps builds to enforce a minimum level of documentation.  You can
/// use the <see cref="CriticalTypes"/> property to control which types are most important for documentation.
/// </para>
/// </remarks>
public partial class MudComponentDocumenter()
{
    /// <summary>
    /// The assembly to document.
    /// </summary>
    public Assembly Assembly { get; set; } = typeof(_Imports).Assembly;

    /// <summary>
    /// The types in the assembly.
    /// </summary>
    public SortedDictionary<string, Type> Types { get; set; } = [];

    /// <summary>
    /// The generated documentation for events.
    /// </summary>
    public SortedDictionary<string, DocumentedEvent> DocumentedEvents { get; private set; } = [];

    /// <summary>
    /// The generated documentation for fields.
    /// </summary>
    public SortedDictionary<string, DocumentedField> DocumentedFields { get; private set; } = [];

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
    /// The types which have documentation but could not be linked to a reflected type.
    /// </summary>
    /// <remarks>
    /// When items exist in this list, the code may need to be improved to find the reflected type.
    /// </remarks>
    public List<string> UnresolvedTypes { get; set; } = [];

    /// <summary>
    /// The properties which have documentation but could not be linked to a reflected property.
    /// </summary>
    /// <remarks>
    /// When items exist in this list, the code may need to be improved to find the reflected property.
    /// </remarks>
    public List<string> UnresolvedProperties { get; set; } = [];

    /// <summary>
    /// The types which have documentation but could not be linked to a reflected field.
    /// </summary>
    /// <remarks>
    /// When items exist in this list, the code may need to be improved to find the reflected field.
    /// </remarks>
    public List<string> UnresolvedFields { get; set; } = [];

    /// <summary>
    /// The types which have documentation but could not be linked to a reflected method.
    /// </summary>
    /// <remarks>
    /// When items exist in this list, the code may need to be improved to find the reflected method.
    /// </remarks>
    public List<string> UnresolvedMethods { get; set; } = [];

    /// <summary>
    /// The types which have documentation but could not be linked to a reflected event.
    /// </summary>
    /// <remarks>
    /// When items exist in this list, the code may need to be improved to find the reflected event.
    /// </remarks>
    public List<string> UnresolvedEvents { get; set; } = [];

    /// <summary>
    /// (For documentation coverage metrics), the most important types to have well documented.
    /// </summary>
    /// <remarks>
    /// This property is only used to calculate metrics.
    /// </remarks>
    public static List<string> CriticalTypes =
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
    private static List<string> ExcludedMethods =
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
    /// Generates documentation for all types.
    /// </summary>
    public bool Execute()
    {
        AddTypesToDocument();
        MergeXmlDocumentation();
        ExportApiDocumentation();
        CalculateDocumentationCoverage();
        return true;
    }

    /// <summary>
    /// Adds an empty documented type for each MudBlazor component and related public type.
    /// </summary>
    public void AddTypesToDocument()
    {
        // Get all public types as a sorted dictionary
        Types = new(Assembly.GetTypes().Where(type => type.IsPublic).ToDictionary(r => r.Name, v => v));

        foreach (var type in Types)
        {
            var documentedType = AddTypeToDocument(type.Value);
            AddPropertiesToDocument(type.Value, documentedType);
            AddMethodsToDocument(type.Value, documentedType);
            AddFieldsToDocument(type.Value, documentedType);
            AddEventsToDocument(type.Value, documentedType);
        }
    }

    /// <summary>
    /// Adds the specified type and any related public types.
    /// </summary>
    /// <param name="type">The type to add.</param>
    public DocumentedType AddTypeToDocument(Type type)
    {
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

            // Add the populated type
            DocumentedTypes.Add(type.FullName, documentedType);
        }

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
            // Link the property to the type
            documentedType.Properties.Add(documentedProperty.Key, documentedProperty);
        }
    }

    /// <summary>
    /// Adds fields for the specified type.
    /// </summary>
    /// <param name="type">the type to examine.</param>
    public void AddFieldsToDocument(Type type, DocumentedType documentedType)
    {
        // Look for public properties 
        var fields = type.GetFields().ToList();
        // Add protected methods
        fields.AddRange(type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
        // Remove private and backing fields
        fields.RemoveAll(field => field.Name.Contains("k__BackingField") || field.Name == "value__");
        // Remove duplicates
        fields = fields.DistinctBy(property => property.Name).ToList();
        // Go through each property
        foreach (var field in fields)
        {
            var category = field.GetCustomAttribute<CategoryAttribute>();
            var blazorParameter = field.GetCustomAttribute<ParameterAttribute>();
            var key = $"{type.FullName}.{field.Name}";

            // Has this property been documented before?
            if (!DocumentedFields.TryGetValue(key, out var documentedField))
            {
                // No.
                documentedField = new DocumentedField()
                {
                    Key = key,
                    Name = field.Name,
                    Type = field.FieldType,
                    TypeName = field.FieldType.Name,
                    TypeFullName = field.FieldType.FullName,
                };
                DocumentedFields.Add(key, documentedField);
            }
            // Link the property to the type
            documentedType.Fields.Add(documentedField.Key, documentedField);
        }
    }

    /// <summary>
    /// Adds events for the specified type.
    /// </summary>
    /// <param name="type">the type to examine.</param>
    public void AddEventsToDocument(Type type, DocumentedType documentedType)
    {
        // Look for public properties 
        var events = type.GetEvents().ToList();
        // Add protected methods
        events.AddRange(type.GetEvents(BindingFlags.Instance | BindingFlags.NonPublic));
        // Remove duplicates
        events = events.DistinctBy(property => property.Name).ToList();
        // Go through each property
        foreach (var eventItem in events)
        {
            var category = eventItem.GetCustomAttribute<CategoryAttribute>();
            var blazorParameter = eventItem.GetCustomAttribute<ParameterAttribute>();
            var key = $"{eventItem.DeclaringType.FullName}.{eventItem.Name}";

            // Has this property been documented before?
            if (!DocumentedEvents.TryGetValue(key, out var documentedEvent))
            {
                // No.
                documentedEvent = new DocumentedEvent()
                {
                    Key = key,
                    Name = eventItem.Name,
                    Type = eventItem.EventHandlerType,
                };
                DocumentedEvents.Add(key, documentedEvent);
            }
            // Link the property to the type
            documentedType.Events.Add(documentedEvent.Name, documentedEvent);
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
        var path = Assembly.Location.Replace(".dll", ".xml", StringComparison.OrdinalIgnoreCase);
        using var reader = new XmlTextReader(path);
        reader.WhitespaceHandling = WhitespaceHandling.None;
        reader.DtdProcessing = DtdProcessing.Ignore;
        // Move to the first member
        reader.ReadToFollowing("member");
        // Read each "<member name=...>" element
        while (!reader.EOF)
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
                case "M": // Method
                    DocumentMethod(memberTypeAndName[1], content);
                    break;
                case "F": // Field (or Enum)
                    DocumentField(memberTypeAndName[1], content);
                    break;
                case "E": // Event
                    DocumentEvent(memberTypeAndName[1], content);
                    break;
            }
            // Are we at the end of the document?
            if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "members")
            {
                break;
            }
        }
    }

    /// <summary>
    /// Adds HTML documentation for the specified type.
    /// </summary>
    /// <param name="memberFullName">The namespace and class of the member.</param>
    /// <param name="xmlContent">The raw XML documentation for the member.</param>
    public void DocumentType(string memberFullName, string xmlContent)
    {
        if (DocumentedTypes.TryGetValue(memberFullName, out var type))
        {
            type.Remarks = GetRemarks(xmlContent);
            type.Summary = GetSummary(xmlContent);
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
        if (DocumentedProperties.TryGetValue(memberFullName, out var property))
        {
            property.Remarks = GetRemarks(xmlContent);
            property.Summary = GetSummary(xmlContent);
        }
        else
        {
            UnresolvedProperties.Add(memberFullName);
        }
    }

    /// <summary>
    /// Adds HTML documentation for the specified field.
    /// </summary>
    /// <param name="memberFullName">The namespace and class of the member.</param>
    /// <param name="xmlContent">The raw XML documentation for the member.</param>
    public void DocumentField(string memberFullName, string xmlContent)
    {
        if (DocumentedFields.TryGetValue(memberFullName, out var field))
        {
            field.Summary = GetSummary(xmlContent);
        }
        else
        {
            var enumPart = memberFullName.Substring(0, memberFullName.LastIndexOf("."));
            var valuePart = memberFullName.Substring(enumPart.Length + 1);

            if (DocumentedFields.TryGetValue(enumPart, out var enumerationItem))
            {
                enumerationItem.Summary = GetSummary(xmlContent);
            }
            else
            {
                UnresolvedFields.Add(memberFullName);
            }
        }
    }

    /// <summary>
    /// Adds HTML documentation for the specified field.
    /// </summary>
    /// <param name="memberFullName">The namespace and class of the member.</param>
    /// <param name="xmlContent">The raw XML documentation for the member.</param>
    public void DocumentMethod(string memberFullName, string xmlContent)
    {
        if (DocumentedMethods.TryGetValue(memberFullName, out var documentedType))
        {
            documentedType.Summary = GetSummary(xmlContent);
            documentedType.Remarks = GetRemarks(xmlContent);
        }
        else
        {
            UnresolvedMethods.Add(memberFullName);
        }
    }

    /// <summary>
    /// Adds HTML documentation for the specified field.
    /// </summary>
    /// <param name="memberFullName">The namespace and class of the member.</param>
    /// <param name="xmlContent">The raw XML documentation for the member.</param>
    public void DocumentEvent(string memberFullName, string xmlContent)
    {
        if (DocumentedEvents.TryGetValue(memberFullName, out var documentedType))
        {
            documentedType.Summary = GetSummary(xmlContent);
            documentedType.Remarks = GetRemarks(xmlContent);
        }
        else
        {
            UnresolvedEvents.Add(memberFullName);
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
    public void ExportApiDocumentation()
    {
        using var writer = new ApiDocumentationWriter(DocumentedTypes, Paths.ApiDocumentationFilePath);

        writer.WriteHeader();
        writer.WriteClassStart();
        writer.WriteConstructorStart();

        foreach (var type in DocumentedTypes)
        {
            writer.WriteType(type);
        }

        writer.WriteConstructorEnd();
        writer.WriteClassEnd();
    }

    /// <summary>
    /// Calculates how thoroughly types are documented.
    /// </summary>
    public void CalculateDocumentationCoverage()
    {
        // Calculate how well the most critical items are documented
        var totalCriticalTypes = DocumentedTypes.Count(type => CriticalTypes.Contains(type.Value.Name));
        var totalCriticalDocumentedTypes = DocumentedTypes.Count(type => CriticalTypes.Contains(type.Value.Name) && !string.IsNullOrEmpty(type.Value.Summary));
        var criticalTypeCoverage = totalCriticalDocumentedTypes / (double)totalCriticalTypes;
        // Calculate how many items have good documentation
        var summarizedTypes = DocumentedTypes.Count(type => !string.IsNullOrEmpty(type.Value.Summary));
        var summarizedProperties = DocumentedProperties.Count(property => !string.IsNullOrEmpty(property.Value.Summary));
        var summarizedMethods = DocumentedMethods.Count(method => !string.IsNullOrEmpty(method.Value.Summary));
        var summarizedFields = DocumentedFields.Count(field => !string.IsNullOrEmpty(field.Value.Summary));
        var summarizedEvents = DocumentedEvents.Count(eventItem => !string.IsNullOrEmpty(eventItem.Value.Summary));
        // Calculate the coverage metrics for documentation
        var typeCoverage = summarizedTypes / (double)DocumentedTypes.Count;
        var propertyCoverage = summarizedProperties / (double)DocumentedProperties.Count;
        var methodCoverage = summarizedMethods / (double)DocumentedMethods.Count;
        var fieldCoverage = summarizedFields / (double)DocumentedFields.Count;
        var eventCoverage = summarizedEvents / (double)DocumentedEvents.Count;

        Console.WriteLine("XML Documentation Coverage for MudBlazor:");
        Console.WriteLine();
        Console.WriteLine($"Core Types: {totalCriticalDocumentedTypes} of {totalCriticalTypes} ({criticalTypeCoverage:P0}) core types");
        Console.WriteLine($"Types:      {summarizedTypes} of {DocumentedTypes.Count} ({typeCoverage:P0}) other types");
        Console.WriteLine($"Properties: {summarizedProperties} of {DocumentedProperties.Count} ({propertyCoverage:P0}) properties");
        Console.WriteLine($"Methods:    {summarizedMethods} of {DocumentedMethods.Count} ({methodCoverage:P0}) methods");
        Console.WriteLine($"Fields:     {summarizedFields} of {DocumentedFields.Count} ({fieldCoverage:P0}) fields/enums");
        Console.WriteLine($"Events:     {summarizedEvents} of {DocumentedEvents.Count} ({eventCoverage:P0}) events");
        Console.WriteLine();

        if (UnresolvedTypes.Count > 0)
        {
            Console.WriteLine($"WARNING: {UnresolvedTypes.Count} types have XML documentation which couldn't be matched to a type.");
        }
        if (UnresolvedProperties.Count > 0)
        {
            Console.WriteLine($"WARNING: {UnresolvedProperties.Count} properties have XML documentation which couldn't be matched to a type's property.");
        }
        if (UnresolvedMethods.Count > 0)
        {
            Console.WriteLine($"WARNING: {UnresolvedMethods.Count} methods have XML documentation which couldn't be matched to a type's property.");
        }
        if (UnresolvedEvents.Count > 0)
        {
            Console.WriteLine($"WARNING: {UnresolvedEvents.Count} events have XML documentation which couldn't be matched to a type's property.");
        }
        if (UnresolvedFields.Count > 0)
        {
            Console.WriteLine($"WARNING: {UnresolvedFields.Count} fields have XML documentation which couldn't be matched to a type's property.");
        }
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
