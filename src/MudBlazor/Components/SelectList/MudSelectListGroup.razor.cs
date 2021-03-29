using System;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudSelectListGroup<T> : MudComponentBase, IDisposable
    {
        protected string Classname =>
        new CssBuilder("mud-list-item mud-list-item-clickable")
          .AddClass("mud-list-item-dense", Dense || ParentList?.Dense == true)
          .AddClass("mud-list-item-gutters", !DisableGutters && !(ParentList?.DisableGutters == true))
          .AddClass("mud-ripple", !DisableRipple && !Disabled)
          .AddClass("mud-list-item-disabled", Disabled)
          .AddClass(Class)
        .Build();

        [Inject] protected NavigationManager UriHelper { get; set; }

        [CascadingParameter] protected MudSelectList<T> ParentList { get; set; }

        /// <summary>
        /// The text to display
        /// </summary>
        [Parameter] public string Text { get; set; }

        /// <summary>
        /// Avatar to use if set.
        /// </summary>
        [Parameter] public string Avatar { get; set; }

        /// <summary>
        /// Link to a URL when clicked.
        /// </summary>
        [Parameter] public string Href { get; set; }

        /// <summary>
        /// Avatar CSS Class to applie if Avtar is set.
        /// </summary>
        [Parameter] public string AvatarClass { get; set; }

        /// <summary>
        /// If true, will disable the list item if it has onclick.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

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

            Expanded = !Expanded;
        }

        protected override void OnInitialized()
        {
            _expanded = InitiallyExpanded;
            if (ParentList != null)
            {
                OnListParametersChanged();
                ParentList.ParametersChanged += OnListParametersChanged;
            }
        }

        private Typo _textTypo;
        private void OnListParametersChanged()
        {
            if (Dense || ParentList?.Dense == true)
            {
                _textTypo = Typo.body2;
            }
            else if (!Dense || !ParentList?.Dense == true)
            {
                _textTypo = Typo.body1;
            }
            StateHasChanged();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && ParentList != null)
            {
                try
                {
                    ParentList.ParametersChanged -= OnListParametersChanged;
                }
                catch (Exception) { /*ignore*/ }
            }
        }

    }
}
