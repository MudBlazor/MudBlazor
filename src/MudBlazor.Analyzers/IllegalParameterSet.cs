// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Analyzers
{
    internal class IllegalParameterSet
    {
        internal Dictionary<INamedTypeSymbol, string[]> Parameters { get; set; }

        internal StringComparer Comparer { get; set; }
        internal IllegalParameters IllegalParameters { get; set; }

        private Compilation _compilation;

        internal IllegalParameterSet(Compilation compilation, IllegalParameters illegalParameters)
        {
            _compilation = compilation;
            Parameters = [];
            IllegalParameters = illegalParameters;
            Comparer = illegalParameters == IllegalParameters.V7IgnoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;

            if (illegalParameters != IllegalParameters.Disabled)
            {
                AddIllegalParameterSet("MudBlazor.MudBadge", "Bottom", "Left", "Start");

                AddIllegalParameterSet("MudBlazor.MudProgressCircular", "Minimum", "Maximum");
                AddIllegalParameterSet("MudBlazor.MudProgressLinear", "Minimum", "Maximum");

                AddIllegalParameterSet("MudBlazor.MudRadio`1", "Option");
                AddIllegalParameterSet("MudBlazor.MudFab", "Icon");

                AddIllegalParameterSet("MudBlazor.MudCheckBox`1", "Checked");
                AddIllegalParameterSet("MudBlazor.MudSwitch`1", "Checked");

                AddIllegalParameterSet("MudBlazor.MudPopover`1", "Direction", "OffsetX", "OffsetY", "SelectOnClick");
                AddIllegalParameterSet("MudBlazor.MudAutocomplete`1", "Direction", "OffsetX", "OffsetY", "SelectOnClick");
                AddIllegalParameterSet("MudBlazor.MudSelect`1", "Direction", "OffsetX", "OffsetY", "SelectOnClick");

                AddIllegalParameterSet("MudBlazor.MudToggleGroup`1", "Outline", "Dense");

                AddIllegalParameterSet("MudBlazor.MudAvatar", "Image", "Alt");

                AddIllegalParameterSet("MudBlazor.MudSlider`1", "Text");

                AddIllegalParameterSet("MudBlazor.MudRadioGroup`1", "SelectedOption", "SelectedOptionChanged");

                AddIllegalParameterSet("MudBlazor.MudSwipeArea", "OnSwipe");

                AddIllegalParameterSet("MudBlazor.MudChip`1", "Avatar", "AvatarClass");

                AddIllegalParameterSet("MudBlazor.MudChipSet`1", "Filter", "MultiSelection", "Multiselection", "Mandatory",
                    "SelectedChip", "SelectedChipChanged", "SelectedChips", "SelectedChipsChanged");

                AddIllegalParameterSet("MudBlazor.MudList`1", "SelectedItem", "SelectedItemChanged", "Clickable", "Avatar",
                    "AvatarClass", "AdornmentColor", "OnClickHandlerPreventDefault");

                AddIllegalParameterSet("MudBlazor.MudListItem`1", "SelectedItem", "SelectedItemChanged", "Clickable", "Avatar",
                    "AvatarClass", "AdornmentColor", "OnClickHandlerPreventDefault");

                AddIllegalParameterSet("MudBlazor.MudTreeView`1", "CanActivate", "CanHover", "CanSelect", "ActivatedValue",
                    "ActivatedValueChanged", "Multiselection", "MultiSelection", "Activated", "ExpandedIcon", "SelectedItem");

                AddIllegalParameterSet("MudBlazor.MudTreeViewItem`1", "CanActivate", "CanHover", "CanSelect", "ActivatedValue",
                    "ActivatedValueChanged", "Multiselection", "MultiSelection", "Activated", "ExpandedIcon", "SelectedItem");

                AddIllegalParameterSet("MudBlazor.MudMenu", "Link", "Target", "HtmlTag", "ButtonType");
                AddIllegalParameterSet("MudBlazor.MudMenuItem", "Link", "Target", "HtmlTag", "ButtonType");

                AddIllegalParameterSet("MudBlazor.MudFileUpload`1", "ButtonTemplate");

                AddIllegalParameterSet("MudBlazor.MudButtonGroup", "VerticalAlign");

                AddIllegalParameterSet("MudBlazor.MudTable`1", "QuickColumns");

                AddIllegalParameterSet("MudBlazor.MudComponentBase", "UnCheckedColor", "Command", "CommandParameter", "IsEnabled",
                    "ClassAction", "InputIcon", "InputVariant", "AllowKeyboardInput", "ClassActions", "DisableRipple", "DisableGutters",
                    "DisablePadding", "DisableElevation", "DisableUnderLine", "DisableRowsPerPage", "Link", "Delayed", "AlertTextPosition",
                    "ShowDelimiters", "DelimitersColor", "DrawerWidth", "DrawerHeightTop", "DrawerHeightBottom", "AppbarMinHeight",
                    "ClassBackground", "Cancelled", "ClassContent", "IsExpanded", "IsExpandedChanged", "IsInitiallyExpanded", "InitiallyExpanded",
                    "RightAlignSmall", "IsExpandable", "ToolBarClass", "DisableToolbar", "DisableLegend", "DisableSliders", "DisablePreview",
                    "DisableModeSwitch", "DisableInputs", "DisableDragEffect", "DisableColorField", "DisableAlpha", "DisableSidePadding",
                    "DisableOverlay", "DisableSliderAnimation", "DisableModifiers", "IsChecked", "IsCheckable", "IsCheckedChanged",
                    "IsVisible", "IsVisibleChanged", "IsOpen", "IsOpened", "IsOpenChanged", "IsActive", "ItemIsDisabled", "IsSelected",
                    "IsSelectedChanged", "IsEditable", "IsEditing", "IsEditSwitchBlocked", "IsHidden", "IsHiddenChanged");
            }
        }

        private void AddIllegalParameterSet(string typeName, params string[] parameterSet)
        {
            var symbol = _compilation.GetBestTypeByMetadataName(typeName);

            if (symbol is not null)
            {
                if (!Parameters.ContainsKey(symbol))
                    Parameters.Add(symbol, parameterSet);
            }
        }
    }


}
