using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudListItem<T> : MudComponentBase, IDisposable
    {
        private bool _selected;
        private bool MultiSelection => MudList?.SelectionMode == SelectionMode.MultiSelection;

        private ParameterState<bool> _expandedState;

        public MudListItem()
        {
            using var registerScope = CreateRegisterScope();
            _expandedState = registerScope.RegisterParameter<bool>(nameof(Expanded))
                .WithParameter(() => Expanded)
                .WithEventCallback(() => ExpandedChanged);
        }

        protected string Classname =>
            new CssBuilder("mud-list-item")
                .AddClass("mud-list-item-dense", GetDense())
                .AddClass("mud-list-item-gutters", Gutters || MudList?.Gutters == true)
                .AddClass("mud-list-item-clickable", GetClickable())
                .AddClass("mud-ripple", !Ripple && GetClickable())
                .AddClass($"mud-selected-item mud-{MudList?.Color.ToDescriptionString()}-text", !MultiSelection && _selected && !GetDisabled())
                .AddClass($"mud-{MudList?.Color.ToDescriptionString()}-hover", !MultiSelection && _selected && !GetDisabled())
                .AddClass("mud-list-item-disabled", GetDisabled())
                .AddClass(Class)
                .Build();

        [Inject]
        protected NavigationManager UriHelper { get; set; } = null!;

        [CascadingParameter]
        protected MudList<T>? MudList { get; set; }

        private MudList<T>? TopLevelList => MudList?.TopLevelList;

        /// <summary>
        /// The text to display
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string? Text { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public T? Value { get; set; }

        /// <summary>
        /// Add an Avatar or custom icon content here. When this is set, Icon will be ignored
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chip.Appearance)]
        public RenderFragment? AvatarContent { get; set; }

        /// <summary>
        /// Link to a URL when clicked.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.ClickAction)]
        public string? Href { get; set; }

        /// <summary>
        /// If true, force browser to redirect outside component router-space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.ClickAction)]
        public bool ForceLoad { get; set; }

        /// <summary>
        /// If true, will disable the list item if it has onclick.
        /// The value can be overridden by the parent list.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets whether to show a ripple effect when the user clicks the button. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// Icon to use if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// The color of the icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Color IconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// Sets the Icon Size.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// The color of the adornment if used. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Expanding)]
        public Color AdornmentColor { get; set; } = Color.Default;

        /// <summary>
        /// Custom expand less icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Expanding)]
        public string ExpandLessIcon { get; set; } = Icons.Material.Filled.ExpandLess;

        /// <summary>
        /// Custom expand more icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Expanding)]
        public string ExpandMoreIcon { get; set; } = Icons.Material.Filled.ExpandMore;

        /// <summary>
        /// If true, the List Sub-header will be indented.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Inset { get; set; }

        /// <summary>
        /// If true, compact vertical padding will be used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool? Dense { get; set; }

        /// <summary>
        /// If true, left and right padding is added. Default is true
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Gutters { get; set; } = true;

        /// <summary>
        /// Expand or collapse nested list. Two-way bindable. Note: if you directly set this to
        /// true or false (instead of using two-way binding) it will initialize the nested list's expansion state.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Expanding)]
        public bool Expanded { get; set; }

        [Parameter]
        public EventCallback<bool> ExpandedChanged { get; set; }

        /// <summary>
        /// Display content of this list item. If set, this overrides Text
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// If set, clicking the list item will only execute the OnClick handler and prevent all other
        /// functionality such as following Href or selection.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool OnClickPreventDefault { get; set; }

        /// <summary>
        /// Add child list items here to create a nested list.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment? NestedList { get; set; }

        /// <summary>
        /// List click event.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        private IEqualityComparer<T?> Comparer => MudList?.Comparer ?? EqualityComparer<T?>.Default;

        private bool ReadOnly => TopLevelList is not null && TopLevelList.ReadOnly;

        private SelectionMode SelectionMode => TopLevelList?.SelectionMode ?? SelectionMode.SingleSelection;

        private Typo TextTypo => GetDense() ? Typo.body2 : Typo.body1;

        private bool GetClickable()
        {
            if (Disabled)
                return false;
            if (NestedList != null)
                return true;
            return !GetReadOnly();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (MudList is not null)
            {
                await MudList.RegisterAsync(this);
            }
        }

        protected async Task OnClickHandlerAsync(MouseEventArgs eventArgs)
        {
            if (GetDisabled())
            {
                return;
            }
            if (OnClickPreventDefault)
            {
                await OnClick.InvokeAsync(eventArgs);
                return;
            }
            if (NestedList != null)
            {
                await _expandedState.SetValueAsync(!_expandedState.Value);
                return;
            }
            if (TopLevelList is not null && !GetReadOnly())
            {
                var value = GetValue();
                if (MultiSelection)
                {
                    await OnCheckboxChangedAsync();
                }
                else
                {
                    if (SelectionMode == SelectionMode.SingleSelection)
                        await TopLevelList.SetSelectedValueAsync(value);
                    else // ToggleSelection
                        await TopLevelList.SetSelectedValueAsync(_selected ? default : value);
                }
            }
            await OnClick.InvokeAsync(eventArgs);
            if (Href != null)
                UriHelper.NavigateTo(Href, ForceLoad);
        }

        internal void SetSelected(bool selected)
        {
            if (GetDisabled() || _selected == selected)
            {
                return;
            }
            _selected = selected;
            StateHasChanged();
        }

        internal T? GetValue()
        {
            if (typeof(T) == typeof(string) && Value is null && Text is not null)
                return (T)(object)Text;
            return Value;
        }

        private bool GetDisabled() => Disabled || (TopLevelList?.GetDisabled() ?? false);

        private bool GetReadOnly() => TopLevelList?.GetReadOnly() == true;

        private bool GetDense() => Dense ?? TopLevelList?.Dense ?? false;

        private bool? GetCheckBoxState()
        {
            return _selected;
            // TODO later, support tristate logic
            //var allChildrenChecked = GetChildItemsRecursive().All(x => x.GetState<bool>(nameof(Selected)));
            //var noChildrenChecked = GetChildItemsRecursive().All(x => !x.GetState<bool>(nameof(Selected)));
            //if (allChildrenChecked && _selectedState)
            //    return true;
            //if (noChildrenChecked && !_selectedState)
            //    return false;
            //return null;
        }

        private async Task OnCheckboxChangedAsync()
        {
            if (TopLevelList is null || ReadOnly || GetDisabled())
                return;
            if (_selected)
                await TopLevelList.DeselectValueAsync(GetValue());
            else
                await TopLevelList.SelectValueAsync(GetValue());
        }

        private string GetIndeterminateIcon()
        {
            // TODO
            //if (TriState)
            //    return IndeterminateIcon;
            // in non-tristate mode we need to fake the checked status. the actual status of the checkbox is irrelevant,
            // only _selected matters!
            return _selected ? CheckedIcon : UncheckedIcon;
        }

        private Color CheckBoxColor => TopLevelList?.CheckBoxColor ?? default;
        private string CheckedIcon => TopLevelList?.CheckedIcon ?? Icons.Material.Filled.CheckBox;
        private string UncheckedIcon => TopLevelList?.UncheckedIcon ?? Icons.Material.Filled.CheckBoxOutlineBlank;

        // TODO
        //private string IndeterminateIcon => MudList?.IndeterminateIcon ?? Icons.Material.Filled.IndeterminateCheckBox;

        public void Dispose()
        {
            if (MudList is null)
            {
                return;
            }
            try
            {
                MudList.Unregister(this);
            }
            catch (Exception) { /*ignore*/ }
        }
    }
}
