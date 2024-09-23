// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Xml.XPath;
using LoxSmoke.DocXml;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Docs.Services.XmlDocs;

#nullable enable

/// <summary>
/// A service for looking up XML documentation for types and members.
/// </summary>
public sealed class XmlDocsService : IXmlDocsService
{
    private HttpClient client;
    static readonly Assembly mudBlazorAssembly;
    static DocXmlReader? reader;

    static XmlDocsService()
    {
        mudBlazorAssembly = typeof(MudBlazor._Imports).Assembly;
    }

    public XmlDocsService(HttpClient client)
    {
        this.client = client;
    }

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        if (reader == null)
        {
            using var stream = await client.GetStreamAsync("/_content/MudBlazor.Docs/xml/MudBlazor.xml");
            var document = new XPathDocument(stream!);
            reader = new(document);
        }
    }

    /// <inheritdoc />
    public Task<Type?> GetTypeAsync(string typeName)
    {
        // Is this a legacy links? (e.g. "alert" instead of "MudAlert")
        if (LegacyToModernTypeNames.TryGetValue(typeName.ToLowerInvariant(), out var newTypeName))
        {
            // Yes.  Coerce the type name to the newer value
            typeName = newTypeName;
        }

        var type = mudBlazorAssembly.GetType(typeName)
            ?? mudBlazorAssembly.GetType("MudBlazor." + typeName)
            ?? typeof(string).Assembly.GetType(typeName)
            ?? typeof(RenderFragment).Assembly.GetType(typeName);

        return Task.FromResult(type);
    }

    /// <inheritdoc />
    public async Task<MemberInfo?> GetMemberAsync(string memberName)
    {
        // Assume the last part of the name is a member
        var parameterIndex = memberName.IndexOf('(');
        if (parameterIndex >= 0)
        {
            memberName = memberName.Replace(memberName.Substring(parameterIndex), "");
        }
        var typePortion = memberName.Substring(0, memberName.LastIndexOf('.'));
        var memberPortion = memberName.Substring(memberName.LastIndexOf('.') + 1);

        var type = await GetTypeAsync(typePortion);
        var member = type?.GetMember(memberPortion);
        return member?.FirstOrDefault();
    }

    /// <inheritdoc />
    public Task<List<PropertyInfo>> GetPropertiesAsync(Type type)
    {
        var properties = new List<PropertyInfo>();
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
        {
            // Exclude event callbacks
            if (property.PropertyType.Name == "EventCallback`1")
            {
                continue;
            }
            if (string.IsNullOrEmpty(property.DeclaringType!.FullName))
            {
                // MudBlazor type but no FullName (?)
            }
            // Exclude non-MudBlazor properties
            else if (!property.DeclaringType!.FullName!.StartsWith("MudBlazor."))
            {
                continue;
            }
            properties.Add(property);
        }
        return Task.FromResult(properties);
    }

    /// <inheritdoc />
    public Task<List<FieldInfo>> GetFieldsAsync(Type type)
    {
        var fields = new List<FieldInfo>();
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
        {
            // Exclude event callbacks
            if (field.FieldType.Name == "EventCallback`1" || field.Name == "value__")
            {
                continue;
            }
            if (string.IsNullOrEmpty(field.DeclaringType!.FullName))
            {
                // MudBlazor type but no FullName (?)
            }
            // Exclude non-MudBlazor fields
            else if (!field.DeclaringType!.FullName!.StartsWith("MudBlazor."))
            {
                continue;
            }
            fields.Add(field);
        }
        return Task.FromResult(fields);
    }

    /// <inheritdoc />
    public Task<List<MethodInfo>> GetMethodsAsync(Type type)
    {
        var methods = new List<MethodInfo>();
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
        {
            if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
            {
                continue;
            }
            if (string.IsNullOrEmpty(method.DeclaringType!.FullName))
            {
                // MudBlazor type but no FullName (?)
            }
            // Exclude non-MudBlazor methods
            else if (!method.DeclaringType!.FullName!.StartsWith("MudBlazor."))
            {
                continue;
            }
            methods.Add(method);
        }
        return Task.FromResult(methods);
    }

    /// <inheritdoc />
    public Task<List<MemberInfo>> GetEventsAsync(Type type)
    {
        var events = new List<MemberInfo>();
        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
        {
            // Include event callbacks
            if (field.FieldType.Name == "EventCallback`1")
            {
                events.Add(field);
            }
        }
        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
        {
            // Include event callbacks
            if (property.PropertyType.Name == "EventCallback`1")
            {
                events.Add(property);
            }
        }
        foreach (var eventItem in type.GetEvents())
        {
            if (string.IsNullOrEmpty(eventItem.DeclaringType!.FullName))
            {
                // MudBlazor type but no FullName (?)
            }
            // Exclude non-MudBlazor properties
            else if (!eventItem.DeclaringType!.FullName!.StartsWith("MudBlazor."))
            {
                continue;
            }
            events.Add(eventItem);
        }
        return Task.FromResult(events);
    }

    /// <inheritdoc />
    public Task<List<Type>> GetTypesAsync()
    {
        var types = new List<Type>(mudBlazorAssembly.ExportedTypes);
        return Task.FromResult(types);
    }

    /// <inheritdoc />
    public async Task<TypeComments?> GetTypeCommentsAsync(string typeName)
    {
        var type = await GetTypeAsync(typeName);
        return await GetTypeCommentsAsync(type);
    }

    /// <inheritdoc />
    public Task<TypeComments?> GetTypeCommentsAsync(Type? type)
    {
        var comments = type == null ? null : reader!.GetTypeComments(type);
        return Task.FromResult(comments);
    }

    /// <inheritdoc />
    public Task<List<CommonComments>> GetPropertyCommentsAsync(Type type)
    {
        var comments = new List<CommonComments>();
        foreach (var property in type.GetProperties(BindingFlags.Public))
        {
            comments.Add(reader!.GetMemberComments(property));
        }
        return Task.FromResult(comments);
    }

    /// <inheritdoc />
    public Task<CommonComments> GetMemberCommentsAsync(MemberInfo? memberInfo)
    {
        if (memberInfo == null)
        {
            return null!;
        }
        var comments = reader!.GetMemberComments(memberInfo);
        return Task.FromResult(comments);
    }

    /// <summary>
    /// A dictionary that maps legacy api links to the new format.
    /// </summary>
    /// <remarks>
    /// This can be removed once it is decided that users are no longer using legacy API links.
    /// </remarks>
    private static readonly Dictionary<string, string> LegacyToModernTypeNames = new()
    {
        { "alert", "MudBlazor.MudAlert" },
        { "appbar", "MudBlazor.MudAppBar" },
        { "avatar", "MudBlazor.MudAvatar" },
        { "avatargroup", "MudBlazor.MudAvatarGroup" },
        { "autocomplete", "MudBlazor.MudAutocomplete`1" },
        { "badge", "MudBlazor.MudBadge" },
        { "bar", "MudBlazor.Charts.Bar" },
        { "barchart", "MudBlazor.Charts.Bar" },
        { "breadcrumbs", "MudBlazor.MudBreadcrumbs" },
        { "breakpointprovider", "MudBlazor.MudBreakpointProvider" },
        { "button", "MudBlazor.MudButton" },
        { "buttonfab", "MudBlazor.MudFab" },
        { "buttongroup", "MudBlazor.MudButtonGroup" },
        { "card", "MudBlazor.MudCard" },
        { "cardactions", "MudBlazor.MudCardActions" },
        { "cardcontent", "MudBlazor.MudCardContent" },
        { "cardheader", "MudBlazor.MudCardHeader" },
        { "cardmedia", "MudBlazor.MudCardMedia" },
        { "carousel", "MudBlazor.MudCarousel`1" },
        { "carouselitem", "MudBlazor.MudCarouselItem" },
        { "checkbox", "MudBlazor.MudCheckBox`1" },
        { "chips", "MudBlazor.MudChip`1" },
        { "chipset", "MudBlazor.MudChipSet`1" },
        { "collapse", "MudBlazor.MudCollapse" },
        { "colorpicker", "MudBlazor.MudColorPicker" },
        { "container", "MudBlazor.MudContainer" },
        { "datagrid", "MudBlazor.MudDataGrid`1" },
        { "datepicker", "MudBlazor.MudDatePicker" },
        { "daterangepicker", "MudBlazor.MudDateRangePicker" },
        { "dialog", "MudBlazor.MudDialog" },
        { "dialoginstance", "MudBlazor.MudDialogInstance" },
        { "dialogprovider", "MudBlazor.MudDialogProvider" },
        { "divider", "MudBlazor.MudDivider" },
        { "donut", "MudBlazor.Charts.Donut" },
        { "donutchart", "MudBlazor.Charts.Donut" },
        { "drawer", "MudBlazor.MudDrawer" },
        { "drawercontainer", "MudBlazor.MudDrawerContainer" },
        { "drawerheader", "MudBlazor.MudDrawerHeader" },
        { "dynamictabs", "MudBlazor.MudDynamicTabs" },
        { "element", "MudBlazor.MudElement" },
        { "expansionpanel", "MudBlazor.MudExpansionPanel" },
        { "expansionpanels", "MudBlazor.MudExpansionPanels" },
        { "field", "MudBlazor.MudField" },
        { "fileuploader", "MudBlazor.MudFileUpload`1" },
        { "focustrap", "MudBlazor.MudFocusTrap" },
        { "form", "MudBlazor.MudForm" },
        { "grid", "MudBlazor.MudGrid" },
        { "hidden", "MudBlazor.MudHidden" },
        { "highlighter", "MudBlazor.MudHighlighter" },
        { "iconbutton", "MudBlazor.MudIconButton" },
        { "icons", "MudBlazor.MudIcon" },
        { "input", "MudBlazor.MudInput`1" },
        { "inputcontrol", "MudBlazor.MudInputControl" },
        { "inputlabel", "MudBlazor.MudInputLabel" },
        { "item", "MudBlazor.MudItem" },
        { "legend", "MudBlazor.Charts.Legend" },
        { "line", "MudBlazor.Charts.Line" },
        { "linechart", "MudBlazor.Charts.Line" },
        { "link", "MudBlazor.MudLink" },
        { "list", "MudBlazor.MudList`1" },
        { "listitem", "MudBlazor.MudListItem`1" },
        { "listsubheader", "MudBlazor.MudListSubheader" },
        { "maincontent", "MudBlazor.MudMainContent" },
        { "menu", "MudBlazor.MudMenu" },
        { "menuitem", "MudBlazor.MudMenuItem" },
        { "messagebox", "MudBlazor.MudMessageBox" },
        { "navgroup", "MudBlazor.MudNavGroup" },
        { "navlink", "MudBlazor.MudNavLink" },
        { "navmenu", "MudBlazor.MudNavMenu" },
        { "numericfield", "MudBlazor.MudNumericField`1" },
        { "overlay", "MudBlazor.MudOverlay" },
        { "pagecontentnavigation", "MudBlazor.MudPageContentNavigation" },
        { "pagination", "MudBlazor.MudPagination" },
        { "paper", "MudBlazor.MudPaper" },
        { "pie", "MudBlazor.Charts.Pie" },
        { "piechart", "MudBlazor.Charts.Pie" },
        { "popover", "MudBlazor.MudPopover" },
        { "progress", "MudBlazor.MudProgressLinear" },
        { "radio", "MudBlazor.MudRadio`1" },
        { "radiogroup", "MudBlazor.MudRadioGroup`1" },
        { "rangeinput", "MudBlazor.MudRangeInput`1" },
        { "rating", "MudBlazor.MudRating" },
        { "ratingitem", "MudBlazor.MudRatingItem" },
        { "rtlprovider", "MudBlazor.MudRTLProvider" },
        { "scrolltotop", "MudBlazor.MudScrollToTop" },
        { "select", "MudBlazor.MudSelect`1" },
        { "selectitem", "MudBlazor.MudSelectItem`1" },
        { "simpletable", "MudBlazor.MudSimpleTable" },
        { "skeleton", "MudBlazor.MudSkeleton" },
        { "slider", "MudBlazor.MudSlider`1" },
        { "snackbar", "MudBlazor.MudSnackbarProvider" },
        { "snackbarelement", "MudBlazor.MudSnackbarElement" },
        { "sparkline", "MudBlazor.Charts.Line" },
        { "stackedbar", "MudBlazor.Charts.StackedBar" },
        { "swipearea", "MudBlazor.MudSwipeArea" },
        { "switch", "MudBlazor.MudSwitch`1" },
        { "table", "MudBlazor.MudTable`1" },
        { "tablegrouprow", "MudBlazor.MudTableGroupRow`1" },
        { "tablepager", "MudBlazor.MudTablePager" },
        { "tablesortlabel", "MudBlazor.MudTableSortLabel`1" },
        { "tabs", "MudBlazor.MudTabs" },
        { "td", "MudBlazor.MudTd" },
        { "textfield", "MudBlazor.MudTextField`1" },
        { "tfootrow", "MudBlazor.MudTFootRow" },
        { "th", "MudBlazor.MudTh" },
        { "theadrow", "MudBlazor.MudTHeadRow" },
        { "timeline", "MudBlazor.MudTimeline" },
        { "timelineitem", "MudBlazor.MudTimelineItem" },
        { "timepicker", "MudBlazor.MudTimePicker" },
        { "timeseries", "MudBlazor.Charts.TimeSeries" },
        { "toggleiconbutton", "MudBlazor.MudToggleIconButton" },
        { "toolbar", "MudBlazor.MudToolBar" },
        { "tooltip", "MudBlazor.MudTooltip" },
        { "tr", "MudBlazor.MudTr" },
        { "treeview", "MudBlazor.MudTreeView`1" },
        { "treeviewitem", "MudBlazor.MudTreeViewItem`1" },
        { "treeviewitemtogglebutton", "MudBlazor.MudTreeViewItemToggleButton" },
        { "typography", "MudBlazor.Typography" },
    };
}
