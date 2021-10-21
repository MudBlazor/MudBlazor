using System;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudListItem : MudComponentBase, IDisposable
    {
        protected string Classname =>
        new CssBuilder("mud-list-item")
          .AddClass("mud-list-item-dense", Dense || MudList?.Dense == true)
          .AddClass("mud-list-item-gutters", !DisableGutters && !(MudList?.DisableGutters == true))
          .AddClass("mud-list-item-clickable", MudList?.Clickable)
          .AddClass("mud-ripple", MudList?.Clickable == true && !DisableRipple && !Disabled)
          .AddClass("mud-selected-item", _selected && !Disabled)
          .AddClass("mud-list-item-disabled", Disabled)
          .AddClass(Class)
        .Build();

        [Inject] protected NavigationManager UriHelper { get; set; }

        [CascadingParameter] protected MudList MudList { get; set; }

        /// <summary>
        /// The text to display
        /// </summary>
        [Parameter] public string Text { get; set; }

        [Parameter] public object Value { get; set; }

        /// <summary>
        /// Avatar to use if set.
        /// </summary>
        [Parameter] public string Avatar { get; set; }

        /// <summary>
        /// Link to a URL when clicked.
        /// </summary>
        [Parameter] public string Href { get; set; }

        /// <summary>
        /// If true, force browser to redirect outside component router-space.
        /// </summary>
        [Parameter] public bool ForceLoad { get; set; }

        /// <summary>
        /// Avatar CSS Class to apply if Avatar is set.
        /// </summary>
        [Parameter] public string AvatarClass { get; set; }

        private bool _disabled;
        /// <summary>
        /// If true, will disable the list item if it has onclick.
        /// The value can be overridden by the parent list.
        /// </summary>
        [Parameter]
        public bool Disabled
        {
            get => _disabled || (MudList?.Disabled ?? false);
            set => _disabled = value;
        }

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

        /// <summary>
        /// Icon to use if set.
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// The color of the icon.
        /// </summary>
        [Parameter] public Color IconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// Sets the Icon Size.
        /// </summary>
        [Parameter] public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// The color of the adornment if used. It supports the theme colors.
        /// </summary>
        [Parameter] public Color AdornmentColor { get; set; } = Color.Default;

        /// <summary>
        /// Custom expand less icon.
        /// </summary>
        [Parameter] public string ExpandLessIcon { get; set; } = Icons.Material.Filled.ExpandLess;

        /// <summary>
        /// Custom expand more icon.
        /// </summary>
        [Parameter] public string ExpandMoreIcon { get; set; } = Icons.Material.Filled.ExpandMore;

        /// <summary>
        /// If true, the List Subheader will be indented.
        /// </summary>
        [Parameter] public bool Inset { get; set; }

        /// <summary>
        /// If true, compact vertical padding will be used.
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// If true, the left and right padding is removed.
        /// </summary>
        [Parameter] public bool DisableGutters { get; set; }

        /// <summary>
        /// Expand or collapse nested list. Two-way bindable. Note: if you directly set this to
        /// true or false (instead of using two-way binding) it will force the nested list's expansion state.
        /// </summary>
        [Parameter]
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

        private bool _expanded;

        [Parameter]
        public EventCallback<bool> ExpandedChanged { get; set; }

        /// <summary>
        /// If true, expands the nested list on first display
        /// </summary>
        [Parameter] public bool InitiallyExpanded { get; set; }

        /// <summary>
        /// Command parameter.
        /// </summary>
        [Parameter] public object CommandParameter { get; set; }

        /// <summary>
        /// Command executed when the user clicks on an element.
        /// </summary>
        [Parameter] public ICommand Command { get; set; }

        /// <summary>
        /// Display content of this list item. If set, this overrides Text
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Add child list items here to create a nested list.
        /// </summary>
        [Parameter] public RenderFragment NestedList { get; set; }

        /// <summary>
        /// List click event.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected void OnClickHandler(MouseEventArgs ev)
        {
            if (Disabled)
                return;
            if (NestedList != null)
            {
                Expanded = !Expanded;
            }
            else if (Href != null)
            {
                MudList?.SetSelectedValue(this.Value);
                OnClick.InvokeAsync(ev);
                UriHelper.NavigateTo(Href, ForceLoad);
            }
            else
            {
                MudList?.SetSelectedValue(this.Value);
                OnClick.InvokeAsync(ev);
                if (Command?.CanExecute(CommandParameter) ?? false)
                {
                    Command.Execute(CommandParameter);
                }
            }
        }

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

        private bool _selected;

        internal void SetSelected(bool selected)
        {
            if (Disabled)
                return;
            if (_selected == selected)
                return;
            _selected = selected;
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
