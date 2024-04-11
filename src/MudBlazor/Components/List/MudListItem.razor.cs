﻿using System;
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
        private Typo _textTypo;
        private bool _selected;
        private IParameterState<bool> _expandedState;

        public MudListItem()
        {
            _expandedState = RegisterParameter(nameof(Expanded), () => Expanded, () => ExpandedChanged);
        }

        protected string Classname =>
            new CssBuilder("mud-list-item")
                .AddClass("mud-list-item-dense", (Dense ?? MudList?.Dense) ?? false)
                .AddClass("mud-list-item-gutters", Gutters || MudList?.Gutters == true)
                .AddClass("mud-list-item-clickable", MudList?.GetReadOnly() != true)
                .AddClass("mud-ripple", MudList?.GetReadOnly() != true && !Ripple && !GetDisabled())
                .AddClass($"mud-selected-item mud-{MudList?.Color.ToDescriptionString()}-text", _selected && !GetDisabled())
                .AddClass($"mud-{MudList?.Color.ToDescriptionString()}-hover", _selected && !GetDisabled())
                .AddClass("mud-list-item-disabled", GetDisabled())
                .AddClass(Class)
                .Build();

        [Inject]
        protected NavigationManager UriHelper { get; set; } = null!;

        [CascadingParameter]
        protected MudList<T>? MudList { get; set; }

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
        /// Avatar to use if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string? Avatar { get; set; }

        /// <summary>
        /// Link to a URL when clicked.
        /// if not empty the list item is rendered as a html anchor with href
        /// See also <see cref="Target"/>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.ClickAction)]
        public string? Href { get; set; }

        /// <summary>
        /// The target attribute specifies where to open the link, if Href is specified.
        /// Possible values: _blank | _self | _parent | _top | <i>framename</i>
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.ClickAction)]
        public string? Target { get; set; }

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
        public string? AvatarClass { get; set; }

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

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool OnClickHandlerPreventDefault { get; set; }

        /// <summary>
        /// Add child list items here to create a nested list.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public RenderFragment? NestedList { get; set; }

        /// <summary>
        /// List click event.
        /// Also called when <see cref="Href"/> is set
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected async Task OnClickHandlerAsync(MouseEventArgs eventArgs)
        {
            if (GetDisabled() || MudList?.GetReadOnly() == true)
            {
                return;
            }

            if (!OnClickHandlerPreventDefault)
            {
                if (NestedList != null)
                {
                    await _expandedState.SetValueAsync(!_expandedState.Value);
                }
                else
                {
                    if (MudList is not null)
                    {
                        await MudList.SetSelectedValueAsync(GetValue());
                    }
                    await OnClick.InvokeAsync(eventArgs);
                    // only in case of a non default target and a force load a manual navigation is required  in all other cases this
                    // is handled by href
                    if (ForceLoad && string.IsNullOrEmpty(Href) == false && string.IsNullOrEmpty(Target))
                    {
                        UriHelper.NavigateTo(Href, forceLoad: true);
                    }
                }
            }
            else
            {
                await OnClick.InvokeAsync(eventArgs);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (MudList is not null)
            {
                await MudList.RegisterAsync(this);
                OnListParametersChanged();
                MudList.ParametersChanged += OnListParametersChanged;
            }
        }

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

        private bool GetDisabled() => Disabled || (MudList?.GetDisabled() ?? false);
        internal string HtmlTag => string.IsNullOrEmpty(Href) ? "div" : "a";
        internal bool GetPreventDefault() => GetDisabled();
        internal bool GetStopPropagation() => true;
        public void Dispose()
        {
            try
            {
                if (MudList is null)
                {
                    return;
                }

                MudList.ParametersChanged -= OnListParametersChanged;
                MudList.Unregister(this);
            }
            catch (Exception) { /*ignore*/ }
        }
    }
}
