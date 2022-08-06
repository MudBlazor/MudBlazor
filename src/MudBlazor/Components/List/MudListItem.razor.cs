using System;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudListItem<T> : MudComponentBase, IDisposable
    {

        #region Parameters, Fields, Injected Services

        [Inject] protected NavigationManager UriHelper { get; set; }

        [CascadingParameter] protected MudList<T> MudList { get; set; }
        [CascadingParameter] internal MudListItem<T> ParentListItem { get; set; }

        protected string Classname =>
        new CssBuilder("mud-list-item")
          .AddClass("mud-list-item-dense", Dense || MudList?.Dense == true)
          .AddClass("mud-list-item-gutters", !DisableGutters && !(MudList?.DisableGutters == true))
          .AddClass("mud-list-item-clickable", MudList?.Clickable)
          .AddClass("mud-ripple", MudList?.Clickable == true && !DisableRipple && !Disabled)
          .AddClass($"mud-selected-item mud-{MudList?.Color.ToDescriptionString()}-text mud-{MudList?.Color.ToDescriptionString()}-hover", _selected && !Disabled && NestedList == null && !MudList.DisableSelectedBackground)
          .AddClass("mud-list-item-hilight", _active && !Disabled && NestedList == null && Exceptional == false)
          .AddClass("mud-list-item-disabled", Disabled)
          .AddClass("mud-list-item-nested-background", MudList != null && MudList.SecondaryBackgroundForNestedItemHeader && NestedList != null)
          .AddClass(Class)
        .Build();

        protected string MultiSelectClassName =>
        new CssBuilder()
            .AddClass("mud-list-item-multiselect")
            .AddClass("mud-list-item-multiselect-checkbox", MudList?.MultiSelectionComponent == MultiSelectionComponent.CheckBox || OverrideMultiSelectionComponent == MultiSelectionComponent.CheckBox)
            .Build();

        internal string ItemId { get; } = "_" + Guid.NewGuid().ToString().Substring(0, 8);

        /// <summary>
        /// Exceptional items can not count on Items list (they count on AllItems), cannot be subject of keyboard navigation and selection.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Exceptional { get; set; }

        /// <summary>
        /// The text to display
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string Text { get; set; }

        /// <summary>
        /// The secondary text to display
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string SecondaryText { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public T Value { get; set; }

        /// <summary>
        /// Avatar to use if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string Avatar { get; set; }

        /// <summary>
        /// Link to a URL when clicked.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.ClickAction)]
        public string Href { get; set; }

        /// <summary>
        /// If true, force browser to redirect outside component router-space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.ClickAction)]
        public bool ForceLoad { get; set; }

        /// <summary>
        /// Overrided component for multiselection. Keep it null to have default one that MudList has.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.ClickAction)]
        public MultiSelectionComponent? OverrideMultiSelectionComponent { get; set; } = null;

        /// <summary>
        /// Avatar CSS Class to apply if Avatar is set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string AvatarClass { get; set; }

        private bool _disabled;
        /// <summary>
        /// If true, will disable the list item if it has onclick.
        /// The value can be overridden by the parent list.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Disabled
        {
            get => _disabled || (MudList?.Disabled ?? false);
            set => _disabled = value;
        }

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool DisableRipple { get; set; }

        /// <summary>
        /// Icon to use if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string Icon { get; set; }

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
        public string ExpandLessIcon { get; set; } = Icons.Filled.ExpandLess;

        /// <summary>
        /// Custom expand more icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Expanding)]
        public string ExpandMoreIcon { get; set; } = Icons.Filled.ExpandMore;

        /// <summary>
        /// If true, the List Subheader will be indented.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Inset { get; set; }

        /// <summary>
        /// If true, compact vertical padding will be used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// If true, the left and right padding is removed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool DisableGutters { get; set; }

        /// <summary>
        /// Command parameter.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.ClickAction)]
        public object CommandParameter { get; set; }

        /// <summary>
        /// Command executed when the user clicks on an element.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.ClickAction)]
        public ICommand Command { get; set; }

        /// <summary>
        /// Display content of this list item. If set, overrides Text.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Prevent default behavior when click on MudSelectItem. Default behavior is selecting the item and style adjustments.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool OnClickHandlerPreventDefault { get; set; }

        /// <summary>
        /// Add child list items here to create a nested list.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment NestedList { get; set; }

        /// <summary>
        /// List click event.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        /// <summary>
        /// If true, expands the nested list on first display.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Expanding)]
        public bool InitiallyExpanded { get; set; }

        private bool _expanded;
        /// <summary>
        /// Expand or collapse nested list. Two-way bindable. Note: if you directly set this to
        /// true or false (instead of using two-way binding) it will force the nested list's expansion state.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Expanding)]
        public bool Expanded
        {
            get => _expanded;
            set
            {
                if (_expanded == value)
                    return;
                _expanded = value;
                _ = ExpandedChanged.InvokeAsync(value);
            }
        }

        /// <summary>
        /// Called when expanded state change.
        /// </summary>
        [Parameter]
        public EventCallback<bool> ExpandedChanged { get; set; }

        #endregion


        #region Lifecycle Methods

        protected override void OnInitialized()
        {
            _expanded = InitiallyExpanded;
            if (MudList != null)
            {
                MudList.Register(this);
                OnListParametersChanged();
                MudList.ParametersChanged += OnListParametersChanged;
            }
        }

        #endregion


        #region Select & Active

        private bool _selected;
        private bool _active;

        /// <summary>
        /// Selected state of the option. Readonly. Use SetSelected for selecting.
        /// </summary>
        public bool IsSelected
        {
            get => _selected;
        }

        internal bool IsActive
        {
            get => _active;
        }

        public void SetSelected(bool selected, bool forceRender = true)
        {
            if (Disabled)
                return;
            if (_selected == selected)
                return;
            _selected = selected;
            if (forceRender)
            {
                StateHasChanged();
            }
        }

        internal void SetActive(bool active, bool forceRender = true)
        {
            if (Disabled)
                return;
            if (_active == active)
                return;
            _active = active;
            if (forceRender)
            {
                StateHasChanged();
            }
        }

        #endregion


        public void ForceRender()
        {
            StateHasChanged();
        }

        protected void OnClickHandler(MouseEventArgs ev)
        {
            if (Disabled)
                return;
            if (!OnClickHandlerPreventDefault)
            {
                if (NestedList != null)
                {
                    Expanded = !Expanded;
                }
                else if (Href != null)
                {
                    MudList?.SetSelectedValue(this);
                    OnClick.InvokeAsync(ev);
                    UriHelper.NavigateTo(Href, ForceLoad);
                }
                else if (MudList?.Clickable == true || MudList?.MultiSelection == true)
                {
                    MudList?.SetSelectedValue(this);
                    OnClick.InvokeAsync(ev);
                }
            }
            else
            {
                OnClick.InvokeAsync(ev);
            }
        }

        protected void OnlyOnClick(MouseEventArgs ev)
        {
            OnClick.InvokeAsync(ev);
        }

        private Typo _textTypo;
        private void OnListParametersChanged()
        {
            if (Dense || MudList?.Dense == true)
            {
                _textTypo = Typo.body2;
            }
            else if (!Dense || !MudList?.Dense == true)
            {
                _textTypo = Typo.body1;
            }
            StateHasChanged();
        }

        public void Dispose()
        {
            try
            {
                if (MudList == null)
                    return;
                MudList.ParametersChanged -= OnListParametersChanged;
                MudList.Unregister(this);
            }
            catch (Exception) { /*ignore*/ }
        }

    }
}
