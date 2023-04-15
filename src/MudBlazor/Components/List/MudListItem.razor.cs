using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudListItem : MudComponentBase, IDisposable
    {
        protected string Classname =>
        new CssBuilder("mud-list-item")
          .AddClass("mud-list-item-dense", (Dense ?? MudList?.Dense) ?? false)
          .AddClass("mud-list-item-gutters", !DisableGutters && !(MudList?.DisableGutters == true))
          .AddClass("mud-list-item-clickable", MudList?.Clickable)
          .AddClass("mud-ripple", MudList?.Clickable == true && !DisableRipple && !Disabled)
          .AddClass($"mud-selected-item mud-{MudList?.Color.ToDescriptionString()}-text mud-{MudList?.Color.ToDescriptionString()}-hover", _selected && !Disabled)
          .AddClass("mud-list-item-disabled", Disabled)
          .AddClass(Class)
        .Build();

        [Inject] protected NavigationManager UriHelper { get; set; }

        [CascadingParameter] protected MudList MudList { get; set; }

        private bool _onClickHandlerPreventDefault = false;

        /// <summary>
        /// The text to display
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string Text { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Selecting)]
        public object Value { get; set; }

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
        public string ExpandLessIcon { get; set; } = Icons.Material.Filled.ExpandLess;

        /// <summary>
        /// Custom expand more icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Expanding)]
        public string ExpandMoreIcon { get; set; } = Icons.Material.Filled.ExpandMore;

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
        public bool? Dense { get; set; }

        /// <summary>
        /// If true, the left and right padding is removed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool DisableGutters { get; set; }

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

        private bool _expanded;

        [Parameter]
        public EventCallback<bool> ExpandedChanged { get; set; }

        /// <summary>
        /// If true, expands the nested list on first display
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Expanding)]
        public bool InitiallyExpanded { get; set; }

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
        /// Display content of this list item. If set, this overrides Text
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool OnClickHandlerPreventDefault
        {
            get => _onClickHandlerPreventDefault;
            set => _onClickHandlerPreventDefault = value;
        }

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

        protected async Task OnClickHandlerAsync(MouseEventArgs eventArgs)
        {
            if (Disabled)
                return;
            if (!_onClickHandlerPreventDefault)
            {
                if (NestedList != null)
                {
                    Expanded = !Expanded;
                }
                else if (Href != null)
                {
                    if (MudList is not null)
                    {
                        await MudList.SetSelectedValueAsync(Value);
                    }
                    await OnClick.InvokeAsync(eventArgs);
                    UriHelper.NavigateTo(Href, ForceLoad);
                }
                else
                {
                    if (MudList is not null)
                    {
                        await MudList.SetSelectedValueAsync(Value);
                    }
                    await OnClick.InvokeAsync(eventArgs);
                    if (Command?.CanExecute(CommandParameter) ?? false)
                    {
                        Command.Execute(CommandParameter);
                    }
                }
            }
            else
            {
                await OnClick.InvokeAsync(eventArgs);
            }
        }

        [Obsolete($"Use {nameof(OnClickHandlerAsync)} instead. This will be removed in v7")]
        protected void OnClickHandler(MouseEventArgs ev)
        {
            if (Disabled)
                return;
            if (!_onClickHandlerPreventDefault)
            {
                if (NestedList != null)
                {
                    Expanded = !Expanded;
                }
                else if (Href != null)
                {
                    MudList?.SetSelectedValueAsync(this.Value);
                    OnClick.InvokeAsync(ev);
                    UriHelper.NavigateTo(Href, ForceLoad);
                }
                else
                {
                    MudList?.SetSelectedValueAsync(this.Value);
                    OnClick.InvokeAsync(ev);
                    if (Command?.CanExecute(CommandParameter) ?? false)
                    {
                        Command.Execute(CommandParameter);
                    }
                }
            }
            else
            {
                OnClick.InvokeAsync(ev);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            _expanded = InitiallyExpanded;
            if (MudList != null)
            {
                await MudList.RegisterAsync(this);
                OnListParametersChanged();
                MudList.ParametersChanged += OnListParametersChanged;
            }
        }

        private Typo _textTypo;
        private void OnListParametersChanged()
        {
            if ((Dense ?? MudList?.Dense) ?? false)
            {
                _textTypo = Typo.body2;
            }
            else if (!((Dense ?? MudList?.Dense) ?? false))
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
