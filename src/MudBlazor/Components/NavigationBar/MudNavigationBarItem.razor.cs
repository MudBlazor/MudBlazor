// Copyright (c) MudBlazor 2025
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// Represents a navigation item within a navigation bar.
    /// </summary>
    public partial class MudNavigationBarItem : MudComponentBase, IAsyncDisposable
    {
        protected internal bool _isSelected;
        private ElementReference _elementReference;

        protected bool IsLink => !string.IsNullOrEmpty(_hrefState.Value);
        protected bool IsCustomContent => !IsLink && ChildContent is not null;
        protected string HtmlTag => IsLink ? "a" : IsCustomContent ? "div" : "button";

        protected string Classname =>
            new CssBuilder("mud-nav-bar-item")
                .AddClass("mud-ripple", Parent?.GetState(x => x.Ripple) == true && !_disabledState.Value)
                .AddClass("mud-disabled", _disabledState.Value)
                .AddClass("mud-hoverable", Parent?.GetState(x => x.Hover) == true)
                .AddClass("mud-nav-bar-item-selected", _isSelected)
                .AddClass("mud-nav-bar-item-underline", Parent?.GetState(x => x.Underline) == true && _isSelected)
                .AddClass($"mud-nav-bar-item-color-{Parent?.GetState(x => x.Color).ToStringFast()}", Parent?.GetState(x => x.Underline) == true && _isSelected)
                .AddClass(SelectedClass ?? Parent?.GetState(x => x.SelectedClass), _isSelected)
                .AddClass(Class)
                .Build();

        protected string BadgeClassname => "mud-nav-bar-item-badge";

        private readonly ParameterState<string?> _hrefState;
        private readonly ParameterState<bool> _disabledState;

        public MudNavigationBarItem()
        {
            using var registerScope = CreateRegisterScope();

            _hrefState = registerScope.RegisterParameter<string?>(nameof(Href))
                .WithParameter(() => Href)
                .WithChangeHandler(OnNavigationParameterChangedAsync);

            _disabledState = registerScope.RegisterParameter<bool>(nameof(Disabled))
                .WithParameter(() => Disabled)
                .WithChangeHandler(OnNavigationParameterChangedAsync);
        }

        private Task OnNavigationParameterChangedAsync()
        {
            return Parent?.RefreshSelectionAsync() ?? Task.CompletedTask;
        }


        /// <summary>
        /// The grouped parameter that contains the badge parameters.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public NavigationBarBadgeParameters BadgeParameters { get; set; } = new();

        /// <summary>
        /// The navigation bar containing this item.
        /// </summary>
        [CascadingParameter]
        public MudNavigationBar? Parent { get; set; }

        /// <summary>
        /// The CSS classes that applies on selected items if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? SelectedClass { get; set; }

        /// <summary>
        /// The custom content displayed inside the navigation item.
        /// </summary>
        /// <remarks>
        /// When <see cref="Href"/> is not set, custom content is rendered inside a non-interactive container, allowing interactive child components.
        /// When <see cref="Href"/> is set, the item renders as a native link; avoid nesting buttons, links, or other interactive elements.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// If true, the navigation item will be disabled and not clickable or selectable.
        /// </summary>
        [Parameter, ParameterState]
        [Category(CategoryTypes.List.Appearance)]
        public bool Disabled { get; set; }

        /// <summary>
        /// The icon to display in the navigation item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? Icon { get; set; }

        /// <summary>
        /// The text displayed for the navigation item.
        /// </summary>
        /// <remarks>
        /// For icon-only items, provide an accessible name using <c>aria-label</c> or <c>aria-labelledby</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? Text { get; set; }

        /// <summary>
        /// The id of the navigation item. Used for identify to the selected item. If not set, the Text property will be used instead.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string? Id { get; set; }

        /// <summary>
        /// The navigation item href. If set, the navigation item will be treated as a link and will navigate to the specified href when clicked.
        /// </summary>
        [Parameter, ParameterState]
        [Category(CategoryTypes.List.Behavior)]
        public string? Href { get; set; }

        /// <summary>
        /// Fires when the navigation item is clicked.
        /// </summary>
        [Category(CategoryTypes.List.Behavior)]
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Parent?.Register(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Parent?.Unregister(this);
            }
        }

        /// <summary>
        /// Releases resources used by this component.
        /// </summary>
        public ValueTask DisposeAsync()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Obtains focus for component.
        /// </summary>
        public ValueTask FocusAsync() => _elementReference.FocusAsync();

        /// <summary>
        /// Obtains blur for component.
        /// </summary>
        public ValueTask BlurAsync() => _elementReference.MudBlurAsync();

        protected internal Task SetSelectedAsync(bool selected)
        {
            if (_isSelected == selected)
                return Task.CompletedTask;

            _isSelected = selected;
            StateHasChanged();

            return Task.CompletedTask;
        }

        protected async Task HandleClickAsync(MouseEventArgs args)
        {
            if (_disabledState.Value || Parent is null)
                return;

            if (IsLink)
            {
                if (args.Button != 0 || args.CtrlKey || args.MetaKey || args.ShiftKey || args.AltKey)
                    return;

                // The browser handles Href navigation.
                await OnClick.InvokeAsync(args);
                return;
            }

            await Parent.ActivateItemAsync(this);
            await OnClick.InvokeAsync(args);
        }

        protected Color GetColor()
        {
            if (_disabledState.Value || !_isSelected)
                return Color.Inherit;

            if (string.IsNullOrEmpty(SelectedClass) &&
                string.IsNullOrEmpty(Parent?.GetState(x => x.SelectedClass)))
            {
                return Parent?.GetState(x => x.Color) ?? Color.Primary;
            }

            return Color.Inherit;
        }
    }
}
