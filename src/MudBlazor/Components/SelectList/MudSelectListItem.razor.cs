using System;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudSelectListItem<T> : MudComponentBase, IDisposable
    {
        #region properties
        protected string Classname =>
        new CssBuilder("mud-list-item mud-list-item-clickable")
          .AddClass("mud-list-item-dense", Dense || ParentList?.Dense == true)
          .AddClass("mud-list-item-gutters", !DisableGutters && !(ParentList?.DisableGutters == true))
          .AddClass("mud-ripple", !DisableRipple && !Disabled)
          .AddClass("mud-selected-item", _selected && !Disabled)
          .AddClass("mud-list-item-disabled", Disabled)
          .AddClass(Class)
        .Build();

        [Inject] protected NavigationManager UriHelper { get; set; }

        [CascadingParameter] protected MudSelectList<T> ParentList { get; set; }

        /// <summary>
        /// A user-defined option that can be selected
        /// </summary>
        [Parameter] public T Item { get; set; }

        /// <summary>
        /// The text to display
        /// </summary>
        [Parameter] public string Text { get; set; }

        /// <summary>
        /// Defines how values are displayed in the list
        /// </summary>
        [CascadingParameter] public Func<T, string> ToStringFunc { get; set; }

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
        /// List click event.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }
        #endregion

        protected void OnClickHandler(MouseEventArgs ev)
        {
            if (Disabled)
                return;
            else if (Href != null)
            {
                ChangeParentSelection();
                OnClick.InvokeAsync(ev);
                UriHelper.NavigateTo(Href);
            }
            else
            {
                ChangeParentSelection();
                OnClick.InvokeAsync(ev);
                if (Command?.CanExecute(CommandParameter) ?? false)
                {
                    Command.Execute(CommandParameter);
                }
            }
        }

        private void ChangeParentSelection()
        {
            ParentList?.SetSelection(Item, !_selected);
        }

        protected override void OnInitialized()
        {
            if (ParentList != null)
            {
                ParentList.Register(this);
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

        private bool _selected;

        internal void SetSelected()
        {
            if (Disabled)
                return;

            var selected = ParentList?.SelectedItems.Contains(Item) ?? false;
            if (_selected == selected)
                return;
            _selected = selected;
            StateHasChanged();
        }

        internal void SetSelected(bool selected)
        {
            if (Disabled)
                return;

            if (_selected == selected)
                return;
            _selected = selected;
            StateHasChanged();
        }

        internal void VerifySelection(HashSet<T> selected)
        {
            _selected = selected.Contains(Item);
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
                    ParentList.Unregister(this);
                }
                catch (Exception) { /*ignore*/ }
            }
        }

    }
}
