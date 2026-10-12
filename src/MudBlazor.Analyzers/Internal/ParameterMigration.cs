// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;

namespace MudBlazor.Analyzers.Internal;

/// <summary>
/// A component parameter that an earlier MudBlazor major version removed, together with the guidance <c>MUD0002</c> appends when the old name still appears on the component that used to declare it.
/// </summary>
/// <remarks>
/// This is a lookup table, not a migration tool.
/// The analyzer never reads, evaluates, or rewrites the attribute's value; it only explains what the replacement parameter is so the reader does not have to search the migration guides.
/// </remarks>
internal sealed class ParameterMigration
{
    private const string V7Guide = "https://github.com/MudBlazor/MudBlazor/discussions/12658";
    private const string V8Guide = "https://github.com/MudBlazor/MudBlazor/discussions/12659";
    private const string V9Guide = "https://github.com/MudBlazor/MudBlazor/issues/12666";

    // Used when the guide for that release does not cover the removal, or describes it wrongly.
    private const string Docs = "https://mudblazor.com/features/analyzers#migration-hints";

    private ParameterMigration(string componentMetadataName, string parameterName, string replacementParameterName, ParameterMigrationKind kind, LocalizableString hint)
    {
        ComponentMetadataName = componentMetadataName;
        ParameterName = parameterName;
        ReplacementParameterName = replacementParameterName;
        Kind = kind;
        Hint = hint;
    }

    /// <summary>
    /// The fully qualified metadata name of the component that declared the removed parameter.
    /// </summary>
    /// <remarks>
    /// Resolved through the compilation so the hint follows type identity rather than the display tag name, which several unrelated components share.
    /// A base type is only used when every current component deriving from it has the replacement, so the advice holds for all of them.
    /// Some of those components were added after the removal and never had the old parameter, such as MudFabMenu, MudContextualActionBar, and SelectColumn.
    /// </remarks>
    internal string ComponentMetadataName { get; }

    /// <summary>
    /// The removed parameter's name, matched case-insensitively like the rest of MUD0002.
    /// </summary>
    internal string ParameterName { get; }

    /// <summary>
    /// The current parameter the hint points at.
    /// </summary>
    internal string ReplacementParameterName { get; }

    /// <summary>
    /// How the value carries over, which the catalog tests use to check the hint against the real replacement.
    /// </summary>
    internal ParameterMigrationKind Kind { get; }

    /// <summary>
    /// The replacement guidance appended to the diagnostic message.
    /// </summary>
    internal LocalizableString Hint { get; }

    /// <summary>
    /// Every removed parameter MUD0002 can explain.
    /// </summary>
    /// <remarks>
    /// Each entry is checked against the source of the release that removed the parameter, not only against the migration guide, because the guides name the wrong component or conversion in several places.
    /// Removals that need more than an in-place change, such as a parameter that moved to a parent component or a set of booleans that became one enum, are left to the guides.
    /// </remarks>
    internal static ImmutableArray<ParameterMigration> All { get; } = ImmutableArray.Create(
        // Buttons
        Rename("MudBlazor.MudBaseButton", "Link", "Href", "v7", V7Guide),
        Inversion("MudBlazor.MudBaseButton", "DisableRipple", "Ripple", "v7", V7Guide),
        Inversion("MudBlazor.MudBaseButton", "DisableElevation", "DropShadow", "v7", V7Guide),
        Rename("MudBlazor.MudFab", "Icon", "StartIcon", "v7", V7Guide),
        Inversion("MudBlazor.MudToggleIconButton", "DisableRipple", "Ripple", "v7", V7Guide),
        Inversion("MudBlazor.MudToggleIconButton", "DisableElevation", "DropShadow", "v7", V7Guide),
        Inversion("MudBlazor.MudButtonGroup", "DisableElevation", "DropShadow", "v7", V7Guide),
        Rename("MudBlazor.MudButtonGroup", "VerticalAlign", "Vertical", "v7", V7Guide),

        // Selection controls
        // Checked is registered per component because MudRadio shares their base today but never had a public Checked.
        BindingValue("MudBlazor.MudCheckBox`1", "Checked", "Value", "v7", V7Guide),
        BindingCallback("MudBlazor.MudCheckBox`1", "Checked", "Value", "v7", V7Guide),
        Inversion("MudBlazor.MudCheckBox`1", "DisableRipple", "Ripple", "v7", V7Guide),
        Manual("MudBlazor.MudCheckBox`1", "LabelPosition", "LabelPlacement", nameof(Resources.MUD0002MigrationHintLabelPosition)),
        BindingValue("MudBlazor.MudSwitch`1", "Checked", "Value", "v7", V7Guide),
        BindingCallback("MudBlazor.MudSwitch`1", "Checked", "Value", "v7", V7Guide),
        Inversion("MudBlazor.MudSwitch`1", "DisableRipple", "Ripple", "v7", V7Guide),
        Manual("MudBlazor.MudSwitch`1", "LabelPosition", "LabelPlacement", nameof(Resources.MUD0002MigrationHintLabelPosition)),
        Rename("MudBlazor.MudRadio`1", "Option", "Value", "v7", V7Guide),
        Inversion("MudBlazor.MudRadio`1", "DisableRipple", "Ripple", "v7", V7Guide),
        Manual("MudBlazor.MudRadio`1", "Placement", "LabelPlacement", nameof(Resources.MUD0002MigrationHintMudRadioPlacement)),
        BindingValue("MudBlazor.MudRadioGroup`1", "SelectedOption", "Value", "v7", V7Guide),
        BindingCallback("MudBlazor.MudRadioGroup`1", "SelectedOption", "Value", "v7", V7Guide),
        Inversion("MudBlazor.MudToggleGroup`1", "DisableRipple", "Ripple", "v7", V7Guide),
        Rename("MudBlazor.MudToggleGroup`1", "Outline", "Outlined", "v7", V7Guide),
        InversionWithNewDefault("MudBlazor.MudRating", "DisableRipple", "Ripple", "MudRating", "v7", Docs),
        InversionWithNewDefault("MudBlazor.MudRatingItem", "DisableRipple", "Ripple", "MudRatingItem", "v7", Docs),

        // Chips, lists, and menus
        Rename("MudBlazor.MudChip`1", "Link", "Href", "v7", V7Guide),
        InheritedInversion("MudBlazor.MudChip`1", "DisableRipple", "Ripple", "MudChipSet", "v7", V7Guide),
        Rename("MudBlazor.MudChipSet`1", "Filter", "CheckMark", "v7", V7Guide),
        InversionWithNewDefault("MudBlazor.MudList`1", "DisablePadding", "Padding", "MudList", "v7", V7Guide),
        InversionWithNewDefault("MudBlazor.MudList`1", "Clickable", "ReadOnly", "MudList", "v7", V7Guide),
        Inversion("MudBlazor.MudList`1", "DisableGutters", "Gutters", "v7", V7Guide),
        Inversion("MudBlazor.MudListItem`1", "DisableRipple", "Ripple", "v7", V7Guide),
        InheritedInversion("MudBlazor.MudListItem`1", "DisableGutters", "Gutters", "MudList", "v7", V7Guide),
        Rename("MudBlazor.MudListItem`1", "AdornmentColor", "ExpandIconColor", "v7", V7Guide),
        Rename("MudBlazor.MudListItem`1", "OnClickHandlerPreventDefault", "OnClickPreventDefault", "v7", V7Guide),
        InitialState("MudBlazor.MudListItem`1", "InitiallyExpanded", "Expanded", "v7", V7Guide),
        Inversion("MudBlazor.MudListSubheader", "DisableGutters", "Gutters", "v7", V7Guide),
        Inversion("MudBlazor.MudMenu", "DisableRipple", "Ripple", "v7", V7Guide),
        Inversion("MudBlazor.MudMenu", "DisableElevation", "DropShadow", "v7", V7Guide),
        // IsOpen was never a parameter, so only the callback is a migration.
        Rename("MudBlazor.MudMenu", "IsOpenChanged", "OpenChanged", "v7", V7Guide),
        Rename("MudBlazor.MudMenu", "PositionAtCurser", "PositionAtCursor", "v7", Docs),
        Rename("MudBlazor.MudMenuItem", "Link", "Href", "v7", V7Guide),

        // Alerts, dialogs, tooltips, and layout
        Manual("MudBlazor.MudAlert", "AlertTextPosition", "ContentAlignment", nameof(Resources.MUD0002MigrationHintMudAlertAlertTextPosition)),
        BindingValue("MudBlazor.MudDialog", "IsVisible", "Visible", "v7", V7Guide),
        BindingCallback("MudBlazor.MudDialog", "IsVisible", "Visible", "v7", V7Guide),
        Inversion("MudBlazor.MudDialog", "DisableSidePadding", "Gutters", "v7", V7Guide),
        Rename("MudBlazor.MudDialog", "ClassActions", "ActionsClass", "v7", Docs),
        Rename("MudBlazor.MudDialog", "ClassContent", "ContentClass", "v7", V7Guide),
        BindingValue("MudBlazor.MudMessageBox", "IsVisible", "Visible", "v7", V7Guide),
        BindingCallback("MudBlazor.MudMessageBox", "IsVisible", "Visible", "v7", V7Guide),
        BindingValue("MudBlazor.MudTooltip", "IsVisible", "Visible", "v7", V7Guide),
        BindingCallback("MudBlazor.MudTooltip", "IsVisible", "Visible", "v7", V7Guide),
        Manual("MudBlazor.MudTooltip", "Delayed", "Delay", nameof(Resources.MUD0002MigrationHintMudTooltipDelayed)),
        BindingValue("MudBlazor.MudHidden", "IsHidden", "Hidden", "v7", V7Guide),
        BindingCallback("MudBlazor.MudHidden", "IsHidden", "Hidden", "v7", V7Guide),
        BindingValue("MudBlazor.MudExpansionPanel", "IsExpanded", "Expanded", "v7", V7Guide),
        BindingCallback("MudBlazor.MudExpansionPanel", "IsExpanded", "Expanded", "v7", V7Guide),
        Inversion("MudBlazor.MudExpansionPanel", "DisableGutters", "Gutters", "v7", V7Guide),
        InitialState("MudBlazor.MudExpansionPanel", "IsInitiallyExpanded", "Expanded", "v7", V7Guide),
        Inversion("MudBlazor.MudExpansionPanels", "DisableBorders", "Outlined", "v7", V7Guide),
        Inversion("MudBlazor.MudExpansionPanels", "DisableGutters", "Gutters", "v7", V7Guide),
        Inversion("MudBlazor.MudAppBar", "DisableGutters", "Gutters", "v7", V7Guide),
        Inversion("MudBlazor.MudToolBar", "DisableGutters", "Gutters", "v7", V7Guide),
        // The v7 guide lists this under MudDialog, but only MudDrawer ever had it.
        Inversion("MudBlazor.MudDrawer", "DisableOverlay", "Overlay", "v7", Docs),

        // Inputs and pickers
        Inversion("MudBlazor.MudBaseInput`1", "DisableUnderLine", "Underline", "v7", V7Guide),
        Manual("MudBlazor.MudBaseInput`1", "OnKeyPress", "OnKeyDown", nameof(Resources.MUD0002MigrationHintMudBaseInputOnKeyPress)),
        Inversion("MudBlazor.MudField", "DisableUnderLine", "Underline", "v7", V7Guide),
        Manual("MudBlazor.MudTextField`1", "AutoGrow", "Sizing", nameof(Resources.MUD0002MigrationHintMudTextFieldAutoGrow)),
        Manual("MudBlazor.MudInput`1", "AutoGrow", "Sizing", nameof(Resources.MUD0002MigrationHintMudInputAutoGrow)),
        Rename("MudBlazor.MudAutocomplete`1", "IsOpenChanged", "OpenChanged", "v7", V7Guide),
        Rename("MudBlazor.MudAutocomplete`1", "SelectOnClick", "SelectOnActivation", "v7", V7Guide),
        Manual("MudBlazor.MudAutocomplete`1", "SearchFuncWithCancel", "SearchFunc", nameof(Resources.MUD0002MigrationHintMudAutocompleteSearchFuncWithCancel)),
        Manual("MudBlazor.MudSelect`1", "OnOpen", "OpenChanged", nameof(Resources.MUD0002MigrationHintMudSelectOnOpen)),
        Manual("MudBlazor.MudSelect`1", "OnClose", "OpenChanged", nameof(Resources.MUD0002MigrationHintMudSelectOnClose)),
        Inversion("MudBlazor.MudPicker`1", "DisableToolbar", "ShowToolbar", "v7", V7Guide),
        Inversion("MudBlazor.MudPicker`1", "DisableUnderLine", "Underline", "v7", V7Guide),
        Rename("MudBlazor.MudPicker`1", "InputIcon", "AdornmentIcon", "v7", V7Guide),
        Rename("MudBlazor.MudPicker`1", "InputVariant", "Variant", "v7", V7Guide),
        Rename("MudBlazor.MudPicker`1", "ClassActions", "ActionsClass", "v7", Docs),
        Inversion("MudBlazor.MudPickerToolbar", "DisableToolbar", "ShowToolbar", "v7", V7Guide),
        Inversion("MudBlazor.MudColorPicker", "DisableSliders", "ShowSliders", "v7", V7Guide),
        Inversion("MudBlazor.MudColorPicker", "DisablePreview", "ShowPreview", "v7", V7Guide),
        Inversion("MudBlazor.MudColorPicker", "DisableModeSwitch", "ShowModeSwitch", "v7", V7Guide),
        Inversion("MudBlazor.MudColorPicker", "DisableInputs", "ShowInputs", "v7", V7Guide),
        Inversion("MudBlazor.MudColorPicker", "DisableDragEffect", "DragEffect", "v7", V7Guide),
        Inversion("MudBlazor.MudColorPicker", "DisableColorField", "ShowColorField", "v7", V7Guide),
        Inversion("MudBlazor.MudColorPicker", "DisableAlpha", "ShowAlpha", "v7", V7Guide),

        // Navigation, tabs, and progress
        Inversion("MudBlazor.MudNavGroup", "DisableRipple", "Ripple", "v7", V7Guide),
        Inversion("MudBlazor.MudNavLink", "DisableRipple", "Ripple", "v7", V7Guide),
        Inversion("MudBlazor.MudTabs", "DisableRipple", "Ripple", "v7", V7Guide),
        Inversion("MudBlazor.MudTabs", "DisableSliderAnimation", "SliderAnimation", "v7", V7Guide),
        Manual("MudBlazor.MudTabs", "TabPanelClass", "TabButtonsClass", nameof(Resources.MUD0002MigrationHintMudTabsTabPanelClass)),
        // MudTabPanel does not derive from MudTabs, so its own current PanelClass is untouched.
        Manual("MudBlazor.MudTabs", "PanelClass", "TabPanelsClass", nameof(Resources.MUD0002MigrationHintMudTabsPanelClass)),
        Inversion("MudBlazor.MudTimeline", "DisableModifiers", "Modifiers", "v7", V7Guide),
        Rename("MudBlazor.MudProgressCircular", "Minimum", "Min", "v7", V7Guide),
        Rename("MudBlazor.MudProgressCircular", "Maximum", "Max", "v7", V7Guide),
        Rename("MudBlazor.MudProgressLinear", "Minimum", "Min", "v7", V7Guide),
        Rename("MudBlazor.MudProgressLinear", "Maximum", "Max", "v7", V7Guide),
        Rename("MudBlazor.MudCarousel`1", "ShowDelimiters", "ShowBullets", "v7", V7Guide),
        Rename("MudBlazor.MudCarousel`1", "DelimitersColor", "BulletsColor", "v7", V7Guide),
        Rename("MudBlazor.MudCarousel`1", "DelimitersClass", "BulletsClass", "v7", Docs),
        Rename("MudBlazor.MudCarousel`1", "DelimiterTemplate", "BulletTemplate", "v7", Docs),
        Inversion("MudBlazor.MudPagination", "DisableElevation", "DropShadow", "v7", V7Guide),

        // Tables, data grids, trees, drag and drop, and charts
        // Unlike MudDataGridPager, this pager keeps the value.
        // The v7 guide's generic inversion bullet for this name only applies to MudDataGridPager, so the hint links the docs, which show both pagers side by side.
        Manual("MudBlazor.MudTablePager", "DisableRowsPerPage", "HideRowsPerPage", nameof(Resources.MUD0002MigrationHintMudTablePagerDisableRowsPerPage)),
        Inversion("MudBlazor.MudDataGridPager`1", "DisableRowsPerPage", "PageSizeSelector", "v7", V7Guide),
        Rename("MudBlazor.MudDataGrid`1", "CancelledEditingItem", "CanceledEditingItem", "v8", V8Guide),
        Rename("MudBlazor.PropertyColumn`2", "IsEditable", "Editable", "v7", V7Guide),
        Rename("MudBlazor.TemplateColumn`1", "IsEditable", "Editable", "v7", V7Guide),
        Rename("MudBlazor.MudTr", "IsCheckable", "Checkable", "v7", V7Guide),
        BindingValue("MudBlazor.MudTr", "IsChecked", "Checked", "v7", V7Guide),
        BindingCallback("MudBlazor.MudTr", "IsChecked", "Checked", "v7", V7Guide),
        Rename("MudBlazor.MudTr", "IsExpandable", "Expandable", "v7", V7Guide),
        Rename("MudBlazor.MudTr", "IsEditable", "Editable", "v7", V7Guide),
        Rename("MudBlazor.MudTHeadRow", "IsCheckable", "Checkable", "v7", V7Guide),
        Rename("MudBlazor.MudTHeadRow", "IsExpandable", "Expandable", "v7", V7Guide),
        Rename("MudBlazor.MudTFootRow", "IsCheckable", "Checkable", "v7", V7Guide),
        Rename("MudBlazor.MudTFootRow", "IsExpandable", "Expandable", "v7", V7Guide),
        Rename("MudBlazor.MudTableGroupRow`1", "IsCheckable", "Checkable", "v7", V7Guide),
        Rename("MudBlazor.MudTreeView`1", "CanHover", "Hover", "v7", V7Guide),
        // ActivatedValue never existed, so this is a callback rename rather than a binding pair.
        Rename("MudBlazor.MudTreeView`1", "ActivatedValueChanged", "SelectedValueChanged", "v7", V7Guide),
        Manual("MudBlazor.MudTreeViewItem`1", "ExpandedIcon", "ExpandButtonIcon", nameof(Resources.MUD0002MigrationHintMudTreeViewItemExpandedIcon)),
        Rename("MudBlazor.MudTreeViewItem`1", "ExpandedIconColor", "ExpandButtonIconColor", "v7", Docs),
        Rename("MudBlazor.MudDropZone`1", "ItemIsDisabled", "ItemDisabled", "v7", V7Guide),
        Rename("MudBlazor.MudDropContainer`1", "ItemIsDisabled", "ItemDisabled", "v7", V7Guide),
        Rename("MudBlazor.MudChart`1", "XAxisLabels", "ChartLabels", "v9", V9Guide),
        Rename("MudBlazor.MudChart`1", "InputLabels", "ChartLabels", "v9", V9Guide));

    private static ParameterMigration Rename(string component, string parameter, string replacement, string release, string url) =>
        Create(component, parameter, replacement, ParameterMigrationKind.Rename, nameof(Resources.MUD0002MigrationHintRename), parameter, release, replacement, url);

    private static ParameterMigration Inversion(string component, string parameter, string replacement, string release, string url) =>
        Create(component, parameter, replacement, ParameterMigrationKind.Inversion, nameof(Resources.MUD0002MigrationHintInversion), parameter, release, replacement, url);

    private static ParameterMigration InversionWithNewDefault(string component, string parameter, string replacement, string componentName, string release, string url) =>
        Create(component, parameter, replacement, ParameterMigrationKind.InversionWithNewDefault, nameof(Resources.MUD0002MigrationHintInversionWithNewDefault), parameter, release, replacement, componentName, url);

    private static ParameterMigration InheritedInversion(string component, string parameter, string replacement, string parentName, string release, string url) =>
        Create(component, parameter, replacement, ParameterMigrationKind.InheritedInversion, nameof(Resources.MUD0002MigrationHintInheritedInversion), parameter, release, replacement, parentName, url);

    private static ParameterMigration BindingValue(string component, string parameter, string replacement, string release, string url) =>
        Create(component, parameter, replacement, ParameterMigrationKind.Binding, nameof(Resources.MUD0002MigrationHintBinding),
            parameter, release, replacement, $"{parameter}Changed", $"{replacement}Changed", parameter, replacement, url);

    // Registers the callback half of a pair whose value half is registered with BindingValue.
    private static ParameterMigration BindingCallback(string component, string valueParameter, string valueReplacement, string release, string url) =>
        Create(component, $"{valueParameter}Changed", $"{valueReplacement}Changed", ParameterMigrationKind.Binding, nameof(Resources.MUD0002MigrationHintBinding),
            $"{valueParameter}Changed", release, $"{valueReplacement}Changed", valueParameter, valueReplacement, valueParameter, valueReplacement, url);

    private static ParameterMigration InitialState(string component, string parameter, string replacement, string release, string url) =>
        Create(component, parameter, replacement, ParameterMigrationKind.InitialState, nameof(Resources.MUD0002MigrationHintInitialState), parameter, release, replacement, url);

    // A manual hint is written out in full, with its own release and link, because its conversion does not fit a template.
    private static ParameterMigration Manual(string component, string parameter, string replacement, string resourceName) =>
        Create(component, parameter, replacement, ParameterMigrationKind.Manual, resourceName);

    private static ParameterMigration Create(string component, string parameter, string replacement, ParameterMigrationKind kind, string resourceName, params string[] formatArguments) =>
        new(component, parameter, replacement, kind, new LocalizableResourceString(resourceName, Resources.ResourceManager, typeof(Resources), formatArguments));
}
