// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MudBlazor.Docs.Models;

/// <summary>
/// Represents a set of XML documentation for MudBlazor types.
/// </summary>
public static partial class ApiDocumentation
{
    /// <summary>
    /// The generated documentation for events.
    /// </summary>
    public static Dictionary<string, DocumentedEvent> Events { get; private set; } = [];

    /// <summary>
    /// The generated documentation for fields.
    /// </summary>
    public static Dictionary<string, DocumentedField> Fields { get; private set; } = [];

    /// <summary>
    /// The generated documentation for types.
    /// </summary>
    public static Dictionary<string, DocumentedType> Types { get; private set; } = [];

    /// <summary>
    /// The generated documentation for properties.
    /// </summary>
    public static Dictionary<string, DocumentedProperty> Properties { get; private set; } = [];

    /// <summary>
    /// The generated documentation for methods.
    /// </summary>
    public static Dictionary<string, DocumentedMethod> Methods { get; private set; } = [];

    /// <summary>
    /// Gets an event, field, method, or property by its name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static DocumentedMember GetMember(string name)
    {
        DocumentedMember result = GetProperty(name);
        result ??= GetField(name);
        result ??= GetMethod(name);
        result ??= GetEvent(name);
        return result;
    }

    /// <summary>
    /// Gets a documented type by its name.
    /// </summary>
    /// <param name="name">The name of the type to find.</param>
    public static DocumentedType GetType(string name)
    {
        // Anything to do?
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        // First, try an exact match
        if (Types.TryGetValue(name, out var match))
        {
            return match;
        }
        // Next, try with the MudBlazor namespace
        if (Types.TryGetValue("MudBlazor." + name, out match))
        {
            return match;
        }
        // Look for legacy links like "api/bar"
        if (LegacyToModernTypeNames.TryGetValue(name.ToLowerInvariant(), out var newTypeName) && Types.TryGetValue(newTypeName, out match))
        {
            return match;
        }
        // Try to match just on the name
        var looseMatch = Types.FirstOrDefault(type => type.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase) || type.Value.NameFriendly.Equals(name, StringComparison.OrdinalIgnoreCase)).Value;
        if (looseMatch != null)
        {
            return looseMatch;
        }
        // Nothing found        
        return null;
    }

    /// <summary>
    /// Yields documented types whose friendly name matches the given text.
    /// </summary>
    /// <param name="text"></param>
    public static IEnumerable<DocumentedType> FindTypesByFriendlyName(string text)
    {
        return Types.Where(type => type.Value.NameFriendly.Contains(text))
            .Select(pair => pair.Value);
    }

    /// <summary>
    /// Gets a documented property by its name.
    /// </summary>
    /// <param name="name">The name of the property to find.</param>
    public static DocumentedProperty GetProperty(string name)
    {
        // Anything to do?
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        // First, try an exact match
        if (Properties.TryGetValue(name, out var match))
        {
            return match;
        }
        // Next, try with the MudBlazor namespace
        if (Properties.TryGetValue("MudBlazor." + name, out match))
        {
            return match;
        }
        // Find a match by name
        var byName = Properties.SingleOrDefault(type => type.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return byName.Value;
    }

    /// <summary>
    /// Gets a documented property by its name.
    /// </summary>
    /// <param name="name">The name of the field to find.</param>
    public static DocumentedField GetField(string name)
    {
        // Anything to do?
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        // First, try an exact match
        if (Fields.TryGetValue(name, out var match))
        {
            return match;
        }
        // Next, try with the MudBlazor namespace
        if (Fields.TryGetValue("MudBlazor." + name, out match))
        {
            return match;
        }
        // Find a match by name
        var byName = Fields.SingleOrDefault(type => type.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return byName.Value;
    }

    /// <summary>
    /// Gets a documented method by its name.
    /// </summary>
    /// <param name="name">The name of the method to find.</param>
    public static DocumentedMethod GetMethod(string name)
    {
        // Anything to do?
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        // First, try an exact match
        if (Methods.TryGetValue(name, out var match))
        {
            return match;
        }
        // Next, try with the MudBlazor namespace
        if (Methods.TryGetValue("MudBlazor." + name, out match))
        {
            return match;
        }
        // Find a match by name
        var byName = Methods.SingleOrDefault(type => type.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return byName.Value;
    }

    /// <summary>
    /// Gets a documented event by its name.
    /// </summary>
    /// <param name="name">The name of the event to find.</param>
    public static DocumentedEvent GetEvent(string name)
    {
        // Anything to do?
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        // First, try an exact match
        if (Events.TryGetValue(name, out var match))
        {
            return match;
        }
        // Next, try with the MudBlazor namespace
        if (Events.TryGetValue("MudBlazor." + name, out match))
        {
            return match;
        }
        // Find a match by name
        var byName = Events.SingleOrDefault(type => type.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return byName.Value;
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
