// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.UnitTests.Analyzers.Internal;

#nullable enable
/// <summary>
/// A removed parameter written on a concrete component, with the guidance MUD0002 has to append for it.
/// </summary>
/// <param name="ComponentMetadataName">The component a consumer writes, which may derive from the type that declared the removed parameter.</param>
/// <param name="Removed">The removed parameter as written in markup.</param>
/// <param name="Replacement">The current parameter the hint must point at.</param>
/// <param name="ExpectedHint">Fragments the appended hint must contain.</param>
/// <param name="TypeArguments">Type arguments for a generic component, defaulting to <c>string</c> for each.</param>
internal sealed record MigrationHintCase(string ComponentMetadataName, string Removed, string Replacement, IReadOnlyList<string> ExpectedHint, string? TypeArguments = null)
{
    /// <summary>
    /// The display name MUD0002 prints for the component.
    /// </summary>
    internal string TagName
    {
        get
        {
            var name = ComponentMetadataName[(ComponentMetadataName.LastIndexOf('.') + 1)..];
            var tick = name.IndexOf('`');

            return tick < 0 ? name : name[..tick];
        }
    }

    /// <summary>
    /// The component as C# type syntax, closed over <see cref="TypeArguments"/> when it is generic.
    /// </summary>
    internal string TypeSyntax
    {
        get
        {
            var tick = ComponentMetadataName.IndexOf('`');
            if (tick < 0)
                return $"global::{ComponentMetadataName}";

            var arity = int.Parse(ComponentMetadataName[(tick + 1)..]);

            return $"global::{ComponentMetadataName[..tick]}<{TypeArguments ?? string.Join(", ", Enumerable.Repeat("string", arity))}>";
        }
    }

    public override string ToString() => $"{TagName}.{Removed}";
}

/// <summary>
/// Every removed parameter MUD0002 explains, written the way a consumer meets it: on the concrete component, with the wording the reader sees.
/// </summary>
/// <remarks>
/// This list is written from the evidence for each removal rather than generated from the analyzer's catalog, so a catalog entry that points at the wrong replacement, the wrong component, or the wrong direction of an inversion fails here.
/// Base-type registrations are exercised through several concrete descendants.
/// </remarks>
internal static class MigrationHintCases
{
    internal const string V7Guide = "https://github.com/MudBlazor/MudBlazor/discussions/12658";
    internal const string V8Guide = "https://github.com/MudBlazor/MudBlazor/discussions/12659";
    internal const string V9Guide = "https://github.com/MudBlazor/MudBlazor/issues/12666";
    internal const string Docs = "https://mudblazor.com/features/analyzers#migration-hints";

    internal static IReadOnlyList<MigrationHintCase> All { get; } =
    [
        // Buttons
        Renamed("MudBlazor.MudButton", "Link", "Href", "v7", V7Guide),
        Renamed("MudBlazor.MudIconButton", "Link", "Href", "v7", V7Guide),
        Renamed("MudBlazor.MudFab", "Link", "Href", "v7", V7Guide),
        Inverted("MudBlazor.MudButton", "DisableRipple", "Ripple", "v7", V7Guide),
        Inverted("MudBlazor.MudIconButton", "DisableRipple", "Ripple", "v7", V7Guide),
        Inverted("MudBlazor.MudFab", "DisableRipple", "Ripple", "v7", V7Guide),
        Inverted("MudBlazor.MudButton", "DisableElevation", "DropShadow", "v7", V7Guide),
        Inverted("MudBlazor.MudIconButton", "DisableElevation", "DropShadow", "v7", V7Guide),
        Inverted("MudBlazor.MudFab", "DisableElevation", "DropShadow", "v7", V7Guide),
        Renamed("MudBlazor.MudFab", "Icon", "StartIcon", "v7", V7Guide),
        Inverted("MudBlazor.MudToggleIconButton", "DisableRipple", "Ripple", "v7", V7Guide),
        Inverted("MudBlazor.MudToggleIconButton", "DisableElevation", "DropShadow", "v7", V7Guide),
        Inverted("MudBlazor.MudButtonGroup", "DisableElevation", "DropShadow", "v7", V7Guide),
        Renamed("MudBlazor.MudButtonGroup", "VerticalAlign", "Vertical", "v7", V7Guide),

        // Selection controls
        BoundValue("MudBlazor.MudCheckBox`1", "Checked", "Value", "v7", V7Guide),
        BoundCallback("MudBlazor.MudCheckBox`1", "Checked", "Value", "v7", V7Guide),
        Inverted("MudBlazor.MudCheckBox`1", "DisableRipple", "Ripple", "v7", V7Guide),
        LabelPosition("MudBlazor.MudCheckBox`1"),
        BoundValue("MudBlazor.MudSwitch`1", "Checked", "Value", "v7", V7Guide),
        BoundCallback("MudBlazor.MudSwitch`1", "Checked", "Value", "v7", V7Guide),
        Inverted("MudBlazor.MudSwitch`1", "DisableRipple", "Ripple", "v7", V7Guide),
        LabelPosition("MudBlazor.MudSwitch`1"),
        Renamed("MudBlazor.MudRadio`1", "Option", "Value", "v7", V7Guide),
        Inverted("MudBlazor.MudRadio`1", "DisableRipple", "Ripple", "v7", V7Guide),
        Manual("MudBlazor.MudRadio`1", "Placement", "LabelPlacement",
            "'Placement' was removed in v8 and replaced by 'LabelPlacement', which takes the same Placement value.",
            "Placement.Left and Placement.Right no longer swap sides in right-to-left layouts, so use Placement.Start or Placement.End there.",
            $"See {V8Guide}"),
        BoundValue("MudBlazor.MudRadioGroup`1", "SelectedOption", "Value", "v7", V7Guide),
        BoundCallback("MudBlazor.MudRadioGroup`1", "SelectedOption", "Value", "v7", V7Guide),
        Inverted("MudBlazor.MudToggleGroup`1", "DisableRipple", "Ripple", "v7", V7Guide),
        Renamed("MudBlazor.MudToggleGroup`1", "Outline", "Outlined", "v7", V7Guide),
        InvertedWithNewDefault("MudBlazor.MudRating", "DisableRipple", "Ripple", "MudRating", "v7", Docs),
        InvertedWithNewDefault("MudBlazor.MudRatingItem", "DisableRipple", "Ripple", "MudRatingItem", "v7", Docs),

        // Chips, lists, and menus
        Renamed("MudBlazor.MudChip`1", "Link", "Href", "v7", V7Guide),
        InvertedFollowingParent("MudBlazor.MudChip`1", "DisableRipple", "Ripple", "MudChipSet", "v7", V7Guide),
        Renamed("MudBlazor.MudChipSet`1", "Filter", "CheckMark", "v7", V7Guide),
        InvertedWithNewDefault("MudBlazor.MudList`1", "DisablePadding", "Padding", "MudList", "v7", V7Guide),
        InvertedWithNewDefault("MudBlazor.MudList`1", "Clickable", "ReadOnly", "MudList", "v7", V7Guide),
        Inverted("MudBlazor.MudList`1", "DisableGutters", "Gutters", "v7", V7Guide),
        Inverted("MudBlazor.MudListItem`1", "DisableRipple", "Ripple", "v7", V7Guide),
        InvertedFollowingParent("MudBlazor.MudListItem`1", "DisableGutters", "Gutters", "MudList", "v7", V7Guide),
        Renamed("MudBlazor.MudListItem`1", "AdornmentColor", "ExpandIconColor", "v7", V7Guide),
        Renamed("MudBlazor.MudListItem`1", "OnClickHandlerPreventDefault", "OnClickPreventDefault", "v7", V7Guide),
        InitialState("MudBlazor.MudListItem`1", "InitiallyExpanded", "Expanded", "v7", V7Guide),
        Inverted("MudBlazor.MudListSubheader", "DisableGutters", "Gutters", "v7", V7Guide),
        Inverted("MudBlazor.MudMenu", "DisableRipple", "Ripple", "v7", V7Guide),
        Inverted("MudBlazor.MudMenu", "DisableElevation", "DropShadow", "v7", V7Guide),
        Renamed("MudBlazor.MudMenu", "IsOpenChanged", "OpenChanged", "v7", V7Guide),
        Renamed("MudBlazor.MudMenu", "PositionAtCurser", "PositionAtCursor", "v7", Docs),
        Renamed("MudBlazor.MudMenuItem", "Link", "Href", "v7", V7Guide),

        // Alerts, dialogs, tooltips, and layout
        Manual("MudBlazor.MudAlert", "AlertTextPosition", "ContentAlignment",
            "'AlertTextPosition' was removed in v7 and replaced by 'ContentAlignment', which takes a HorizontalAlignment with the same member names instead of an AlertTextPosition:",
            "AlertTextPosition=\"AlertTextPosition.Center\" becomes ContentAlignment=\"HorizontalAlignment.Center\"",
            $"See {V7Guide}"),
        BoundValue("MudBlazor.MudDialog", "IsVisible", "Visible", "v7", V7Guide),
        BoundCallback("MudBlazor.MudDialog", "IsVisible", "Visible", "v7", V7Guide),
        Inverted("MudBlazor.MudDialog", "DisableSidePadding", "Gutters", "v7", V7Guide),
        Renamed("MudBlazor.MudDialog", "ClassActions", "ActionsClass", "v7", Docs),
        Renamed("MudBlazor.MudDialog", "ClassContent", "ContentClass", "v7", V7Guide),
        BoundValue("MudBlazor.MudMessageBox", "IsVisible", "Visible", "v7", V7Guide),
        BoundCallback("MudBlazor.MudMessageBox", "IsVisible", "Visible", "v7", V7Guide),
        BoundValue("MudBlazor.MudTooltip", "IsVisible", "Visible", "v7", V7Guide),
        BoundCallback("MudBlazor.MudTooltip", "IsVisible", "Visible", "v7", V7Guide),
        Manual("MudBlazor.MudTooltip", "Delayed", "Delay",
            "'Delayed' was removed in v7 and replaced by 'Delay', which is in milliseconds instead of seconds:",
            "Delayed=\"0.5\" becomes Delay=\"500\"",
            "Delayed=\"@seconds\" becomes Delay=\"@(seconds * 1000)\"",
            $"See {Docs}"),
        BoundValue("MudBlazor.MudHidden", "IsHidden", "Hidden", "v7", V7Guide),
        BoundCallback("MudBlazor.MudHidden", "IsHidden", "Hidden", "v7", V7Guide),
        BoundValue("MudBlazor.MudExpansionPanel", "IsExpanded", "Expanded", "v7", V7Guide),
        BoundCallback("MudBlazor.MudExpansionPanel", "IsExpanded", "Expanded", "v7", V7Guide),
        Inverted("MudBlazor.MudExpansionPanel", "DisableGutters", "Gutters", "v7", V7Guide),
        InitialState("MudBlazor.MudExpansionPanel", "IsInitiallyExpanded", "Expanded", "v7", V7Guide),
        Inverted("MudBlazor.MudExpansionPanels", "DisableBorders", "Outlined", "v7", V7Guide),
        Inverted("MudBlazor.MudExpansionPanels", "DisableGutters", "Gutters", "v7", V7Guide),
        Inverted("MudBlazor.MudAppBar", "DisableGutters", "Gutters", "v7", V7Guide),
        Inverted("MudBlazor.MudToolBar", "DisableGutters", "Gutters", "v7", V7Guide),
        Inverted("MudBlazor.MudDrawer", "DisableOverlay", "Overlay", "v7", Docs),

        // Inputs and pickers
        Inverted("MudBlazor.MudTextField`1", "DisableUnderLine", "Underline", "v7", V7Guide),
        Inverted("MudBlazor.MudNumericField`1", "DisableUnderLine", "Underline", "v7", V7Guide),
        Inverted("MudBlazor.MudSelect`1", "DisableUnderLine", "Underline", "v7", V7Guide),
        Inverted("MudBlazor.MudAutocomplete`1", "DisableUnderLine", "Underline", "v7", V7Guide),
        OnKeyPress("MudBlazor.MudTextField`1"),
        OnKeyPress("MudBlazor.MudNumericField`1"),
        Inverted("MudBlazor.MudField", "DisableUnderLine", "Underline", "v7", V7Guide),
        Manual("MudBlazor.MudTextField`1", "AutoGrow", "Sizing", MigrationHintExpectations.AutoGrow),
        Manual("MudBlazor.MudInput`1", "AutoGrow", "Sizing",
            "'AutoGrow' was removed in v9 and replaced by 'Sizing':",
            "AutoGrow=\"true\" becomes Sizing=\"InputSizing.Auto\"",
            "AutoGrow=\"false\" becomes Sizing=\"InputSizing.Fixed\"",
            "AutoGrow=\"@expression\" becomes Sizing=\"@(expression ? InputSizing.Auto : InputSizing.Fixed)\"",
            "Lines and MaxLines keep their meaning.",
            $"See {Docs}"),
        Renamed("MudBlazor.MudAutocomplete`1", "IsOpenChanged", "OpenChanged", "v7", V7Guide),
        Renamed("MudBlazor.MudAutocomplete`1", "SelectOnClick", "SelectOnActivation", "v7", V7Guide),
        Manual("MudBlazor.MudAutocomplete`1", "SearchFuncWithCancel", "SearchFunc",
            "'SearchFuncWithCancel' was removed in v7 and replaced by 'SearchFunc', which now takes the same cancellable delegate:",
            "SearchFuncWithCancel=\"Search\" becomes SearchFunc=\"Search\"",
            "Remove any old SearchFunc that has no CancellationToken parameter",
            $"See {V7Guide}"),
        Manual("MudBlazor.MudSelect`1", "OnOpen", "OpenChanged",
            "'OnOpen' was removed in v9. Use 'OpenChanged', which receives true when the list opens and false when it closes:",
            "OnOpen=\"Handler\" becomes OpenChanged=\"@(open => { if (open) Handler(); })\"",
            "or OpenChanged=\"@(async open => { if (open) await Handler(); })\" when Handler returns a Task",
            "raised only when the open state actually changes",
            $"See {Docs}"),
        Manual("MudBlazor.MudSelect`1", "OnClose", "OpenChanged",
            "'OnClose' was removed in v9. Use 'OpenChanged', which receives true when the list opens and false when it closes:",
            "OnClose=\"Handler\" becomes OpenChanged=\"@(open => { if (!open) Handler(); })\"",
            "or OpenChanged=\"@(async open => { if (!open) await Handler(); })\" when Handler returns a Task",
            "a single OpenChanged handler for both",
            $"See {Docs}"),
        Inverted("MudBlazor.MudDatePicker", "DisableToolbar", "ShowToolbar", "v7", V7Guide),
        Inverted("MudBlazor.MudDateRangePicker", "DisableToolbar", "ShowToolbar", "v7", V7Guide),
        Inverted("MudBlazor.MudTimePicker", "DisableToolbar", "ShowToolbar", "v7", V7Guide),
        Inverted("MudBlazor.MudColorPicker", "DisableToolbar", "ShowToolbar", "v7", V7Guide),
        Inverted("MudBlazor.MudDatePicker", "DisableUnderLine", "Underline", "v7", V7Guide),
        Renamed("MudBlazor.MudDatePicker", "InputIcon", "AdornmentIcon", "v7", V7Guide),
        Renamed("MudBlazor.MudDatePicker", "InputVariant", "Variant", "v7", V7Guide),
        Renamed("MudBlazor.MudTimePicker", "ClassActions", "ActionsClass", "v7", Docs),
        Inverted("MudBlazor.MudPickerToolbar", "DisableToolbar", "ShowToolbar", "v7", V7Guide),
        Inverted("MudBlazor.MudColorPicker", "DisableSliders", "ShowSliders", "v7", V7Guide),
        Inverted("MudBlazor.MudColorPicker", "DisablePreview", "ShowPreview", "v7", V7Guide),
        Inverted("MudBlazor.MudColorPicker", "DisableModeSwitch", "ShowModeSwitch", "v7", V7Guide),
        Inverted("MudBlazor.MudColorPicker", "DisableInputs", "ShowInputs", "v7", V7Guide),
        Inverted("MudBlazor.MudColorPicker", "DisableDragEffect", "DragEffect", "v7", V7Guide),
        Inverted("MudBlazor.MudColorPicker", "DisableColorField", "ShowColorField", "v7", V7Guide),
        Inverted("MudBlazor.MudColorPicker", "DisableAlpha", "ShowAlpha", "v7", V7Guide),

        // Navigation, tabs, and progress
        Inverted("MudBlazor.MudNavGroup", "DisableRipple", "Ripple", "v7", V7Guide),
        Inverted("MudBlazor.MudNavLink", "DisableRipple", "Ripple", "v7", V7Guide),
        Inverted("MudBlazor.MudTabs", "DisableRipple", "Ripple", "v7", V7Guide),
        Inverted("MudBlazor.MudTabs", "DisableSliderAnimation", "SliderAnimation", "v7", V7Guide),
        Manual("MudBlazor.MudTabs", "TabPanelClass", "TabButtonsClass",
            "'TabPanelClass' was removed in v9 and replaced by 'TabButtonsClass', which takes the same value and still styles each tab button.",
            "TabPanelsClass and MudTabPanel's PanelClass style the panels instead.",
            $"See {V9Guide}"),
        Manual("MudBlazor.MudTabs", "PanelClass", "TabPanelsClass",
            "'PanelClass' was removed in v9 and replaced by 'TabPanelsClass', which takes the same value and still styles the wrapper around all panels.",
            "MudTabPanel's own PanelClass is a different parameter that styles one panel.",
            $"See {V9Guide}"),
        Manual("MudBlazor.MudDynamicTabs", "PanelClass", "TabPanelsClass",
            "'PanelClass' was removed in v9 and replaced by 'TabPanelsClass'",
            $"See {V9Guide}"),
        Inverted("MudBlazor.MudTimeline", "DisableModifiers", "Modifiers", "v7", V7Guide),
        Renamed("MudBlazor.MudProgressCircular", "Minimum", "Min", "v7", V7Guide),
        Renamed("MudBlazor.MudProgressCircular", "Maximum", "Max", "v7", V7Guide),
        Renamed("MudBlazor.MudProgressLinear", "Minimum", "Min", "v7", V7Guide),
        Renamed("MudBlazor.MudProgressLinear", "Maximum", "Max", "v7", V7Guide),
        Renamed("MudBlazor.MudCarousel`1", "ShowDelimiters", "ShowBullets", "v7", V7Guide),
        Renamed("MudBlazor.MudCarousel`1", "DelimitersColor", "BulletsColor", "v7", V7Guide),
        Renamed("MudBlazor.MudCarousel`1", "DelimitersClass", "BulletsClass", "v7", Docs),
        Renamed("MudBlazor.MudCarousel`1", "DelimiterTemplate", "BulletTemplate", "v7", Docs),
        Inverted("MudBlazor.MudPagination", "DisableElevation", "DropShadow", "v7", V7Guide),

        // Tables, data grids, trees, drag and drop, and charts
        Manual("MudBlazor.MudTablePager", "DisableRowsPerPage", "HideRowsPerPage",
            "'DisableRowsPerPage' was removed in v7 and replaced by 'HideRowsPerPage', which keeps the same value:",
            "DisableRowsPerPage=\"true\" becomes HideRowsPerPage=\"true\"",
            "Do not invert it",
            $"See {Docs}"),
        Inverted("MudBlazor.MudDataGridPager`1", "DisableRowsPerPage", "PageSizeSelector", "v7", V7Guide),
        Renamed("MudBlazor.MudDataGrid`1", "CancelledEditingItem", "CanceledEditingItem", "v8", V8Guide),
        Renamed("MudBlazor.PropertyColumn`2", "IsEditable", "Editable", "v7", V7Guide),
        Renamed("MudBlazor.TemplateColumn`1", "IsEditable", "Editable", "v7", V7Guide),
        Renamed("MudBlazor.MudTr", "IsCheckable", "Checkable", "v7", V7Guide),
        BoundValue("MudBlazor.MudTr", "IsChecked", "Checked", "v7", V7Guide),
        BoundCallback("MudBlazor.MudTr", "IsChecked", "Checked", "v7", V7Guide),
        Renamed("MudBlazor.MudTr", "IsExpandable", "Expandable", "v7", V7Guide),
        Renamed("MudBlazor.MudTr", "IsEditable", "Editable", "v7", V7Guide),
        Renamed("MudBlazor.MudTHeadRow", "IsCheckable", "Checkable", "v7", V7Guide),
        Renamed("MudBlazor.MudTHeadRow", "IsExpandable", "Expandable", "v7", V7Guide),
        Renamed("MudBlazor.MudTFootRow", "IsCheckable", "Checkable", "v7", V7Guide),
        Renamed("MudBlazor.MudTFootRow", "IsExpandable", "Expandable", "v7", V7Guide),
        Renamed("MudBlazor.MudTableGroupRow`1", "IsCheckable", "Checkable", "v7", V7Guide),
        Renamed("MudBlazor.MudTreeView`1", "CanHover", "Hover", "v7", V7Guide),
        Renamed("MudBlazor.MudTreeView`1", "ActivatedValueChanged", "SelectedValueChanged", "v7", V7Guide),
        Manual("MudBlazor.MudTreeViewItem`1", "ExpandedIcon", "ExpandButtonIcon",
            "'ExpandedIcon' was removed in v7 and replaced by 'ExpandButtonIcon', which takes the same value and still sets the expand button's icon.",
            "It is not IconExpanded, which replaces the item's own icon while the item is expanded.",
            $"See {V7Guide}"),
        Renamed("MudBlazor.MudTreeViewItem`1", "ExpandedIconColor", "ExpandButtonIconColor", "v7", Docs),
        Renamed("MudBlazor.MudDropZone`1", "ItemIsDisabled", "ItemDisabled", "v7", V7Guide),
        Renamed("MudBlazor.MudDropContainer`1", "ItemIsDisabled", "ItemDisabled", "v7", V7Guide),
        Renamed("MudBlazor.MudChart`1", "XAxisLabels", "ChartLabels", "v9", V9Guide) with { TypeArguments = "double" },
        Renamed("MudBlazor.MudChart`1", "InputLabels", "ChartLabels", "v9", V9Guide) with { TypeArguments = "double" },
    ];

    private static MigrationHintCase Renamed(string component, string removed, string replacement, string version, string url) =>
        new(component, removed, replacement,
        [
            $"'{removed}' was removed in {version} and replaced by '{replacement}', which takes the same value.",
            $"See {url}"
        ]);

    private static MigrationHintCase Inverted(string component, string removed, string replacement, string version, string url) =>
        new(component, removed, replacement, InversionFragments(removed, replacement, version, url));

    private static MigrationHintCase InvertedWithNewDefault(string component, string removed, string replacement, string owner, string version, string url) =>
        new(component, removed, replacement,
        [
            .. InversionFragments(removed, replacement, version, url),
            $"The default changed as well, so a {owner} that never set '{removed}' now needs {replacement}=\"true\" to keep its old behavior."
        ]);

    private static MigrationHintCase InvertedFollowingParent(string component, string removed, string replacement, string parent, string version, string url) =>
        new(component, removed, replacement,
        [
            $"'{removed}' was removed in {version} and replaced by '{replacement}', which inverts the value:",
            $"{removed}=\"true\" becomes {replacement}=\"false\"",
            $"{removed}=\"@expression\" becomes {replacement}=\"@(expression ? false : null)\"",
            $"For {removed}=\"false\", leave '{replacement}' unset so it follows the parent {parent}, because {replacement}=\"true\" now overrides the parent.",
            $"See {url}"
        ]);

    private static MigrationHintCase BoundValue(string component, string removed, string replacement, string version, string url) =>
        new(component, removed, replacement,
        [
            $"'{removed}' was removed in {version} and replaced by '{replacement}', which takes the same value.",
            $"Its partner '{removed}Changed' became '{replacement}Changed', so @bind-{removed} becomes @bind-{replacement}.",
            $"See {url}"
        ]);

    private static MigrationHintCase BoundCallback(string component, string removedValue, string replacementValue, string version, string url) =>
        new(component, $"{removedValue}Changed", $"{replacementValue}Changed",
        [
            $"'{removedValue}Changed' was removed in {version} and replaced by '{replacementValue}Changed', which takes the same value.",
            $"Its partner '{removedValue}' became '{replacementValue}', so @bind-{removedValue} becomes @bind-{replacementValue}.",
            $"See {url}"
        ]);

    private static MigrationHintCase InitialState(string component, string removed, string replacement, string version, string url) =>
        new(component, removed, replacement,
        [
            $"'{removed}' was removed in {version} and replaced by '{replacement}': {removed}=\"true\" becomes {replacement}=\"true\".",
            $"'{replacement}' holds the current state rather than a starting value, so merge it with any existing {replacement} or @bind-{replacement} instead of adding a second attribute.",
            $"See {url}"
        ]);

    private static MigrationHintCase LabelPosition(string component) =>
        Manual(component, "LabelPosition", "LabelPlacement",
            "'LabelPosition' was removed in v8 and replaced by 'LabelPlacement', which takes a Placement instead of a LabelPosition:",
            "LabelPosition=\"LabelPosition.Start\" becomes LabelPlacement=\"Placement.Start\"",
            "LabelPosition=\"LabelPosition.End\" becomes LabelPlacement=\"Placement.End\"",
            $"See {V8Guide}");

    private static MigrationHintCase OnKeyPress(string component) =>
        Manual(component, "OnKeyPress", "OnKeyDown",
            "'OnKeyPress' was removed in v7. Use 'OnKeyDown', which receives the same KeyboardEventArgs but also fires for keys that type no character, such as Tab, Shift, and the arrow keys, so check the key in the handler.",
            $"See {V7Guide}");

    private static MigrationHintCase Manual(string component, string removed, string replacement, params string[] fragments) =>
        new(component, removed, replacement, fragments);

    private static string[] InversionFragments(string removed, string replacement, string version, string url) =>
    [
        $"'{removed}' was removed in {version} and replaced by '{replacement}', which inverts the value:",
        $"{removed}=\"true\" becomes {replacement}=\"false\"",
        $"{removed}=\"false\" becomes {replacement}=\"true\"",
        $"{removed}=\"@expression\" becomes {replacement}=\"@(!expression)\"",
        $"See {url}"
    ];
}
#nullable restore
