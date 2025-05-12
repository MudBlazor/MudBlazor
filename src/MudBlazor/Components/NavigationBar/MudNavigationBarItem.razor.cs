// Copyright (c) MudBlazor 2025
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// Represents a navigation item within a navigation bar.
    /// </summary>
    public partial class MudNavigationBarItem : MudComponentBase, IAsyncDisposable
    {
        #region Fields
        protected internal bool _isSelected;
        private ElementReference _elementReference;

        protected string Classname =>
            new CssBuilder("mud-nav-bar-item")
                .AddClass("mud-ripple", Parent?.Ripple == true && !Disabled)
                .AddClass("mud-disabled", Disabled)
                .AddClass("mud-hoverable", Parent?.Hover)
                .AddClass($"mud-nav-bar-item-selected {SelectedClass ?? Parent?.SelectedClass}", _isSelected)
                .AddClass(Class)
                .Build();

        protected string BadgeClassname =>
            new CssBuilder("mud-nav-bar-item-badge")
                .AddClass($"border-solid border-b-2 mud-border-{Parent?.Color.ToDescriptionString() ?? "primary"}", Parent?.Underline == true && _isSelected)
                .AddClass("mud-disabled", Disabled)
                .Build();
        #endregion

        #region Grouped Parameters
        /// <summary>
        /// The grouped parameter that contains the badge parameters.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public NavigationBarBadgeParameters BadgeParameters { get; set; } = new();
        #endregion

        #region Parameters
        [CascadingParameter]
        public MudNavigationBar? Parent { get; set; }

        /// <summary>
        /// The CSS classes that applies on selected items if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? SelectedClass { get; set; }

        /// <summary>
        /// The custom content of the navigation item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// If true, the navigation item will be disabled and not clickable or selectable.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Disabled { get; set; }

        /// <summary>
        /// The icon to display in the navigation item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? Icon { get; set; }

        /// <summary>
        /// The text of the navigation item.
        /// </summary>
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
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public string? Href { get; set; }

        /// <summary>
        /// Fires when the navigation item is clicked.
        /// </summary>
        [Category(CategoryTypes.List.Behavior)]
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }
        #endregion

        #region Lifecycle
        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();
            Parent?.Register(this);
            await HandleHrefSelectedAsync(Parent?._currentLocation);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Parent?.Unregister(this);
            }
        }

        public ValueTask DisposeAsync()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Obtains focus for component.
        /// </summary>
        public ValueTask FocusAsync() => _elementReference.FocusAsync();

        /// <summary>
        /// Obtains blur for component.
        /// </summary>
        public ValueTask BlurAsync() => _elementReference.MudBlurAsync();
        #endregion

        #region SelectionLogic
        protected internal async Task HandleHrefSelectedAsync(string? location)
        {
            if (Parent == null)
            {
                await SetSelectedAsync(false);
            }

            if (!string.IsNullOrEmpty(Href))
            {
                await SetSelectedAsync(CompareHrefRoute(Href, location));
            }
        }

        protected bool CompareHrefRoute(string href, string? location)
        {
            if (Parent?.CustomHrefComparisonFunc != null)
            {
                return Parent.CustomHrefComparisonFunc.Invoke(location, Href);
            }

            if (href.Contains('#'))
            {
                href = href.Substring(0, href.IndexOf('#'));
            }
            return location?.StartsWith(href) ?? false;
        }

        protected internal async Task SetSelectedAsync(bool selected)
        {
            bool callChange = _isSelected != selected;
            _isSelected = selected;
            if (callChange && Parent != null)
            {
                await Parent.OnSelectionChange.InvokeAsync(Id ?? Text);
            }
            StateHasChanged();
        }
        #endregion

        #region Protected Methods
        protected async Task HandleClickAsync(MouseEventArgs args)
        {
            if (Disabled || Parent == null)
                return;

            if (!string.IsNullOrEmpty(Href))
            {
                await Parent.DeselectAllAsync();
                await HandleHrefSelectedAsync(Href);
                Parent?.Navigate(Href);
            }
            else
            {
                await Parent.DeselectAllAsync();
                await SetSelectedAsync(true);
            }
            await OnClick.InvokeAsync(args);
        }
        protected async Task HandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (Disabled)
                return;
            if (args.Key == "Enter" || args.Key == "NumpadEnter")
            {
                await HandleClickAsync(new MouseEventArgs());
            }
        }

        protected Color GetColor()
        {
            if (Disabled)
                return Color.Inherit;

            if (!_isSelected)
                return Color.Inherit;

            if (string.IsNullOrEmpty(SelectedClass) && string.IsNullOrEmpty(Parent?.SelectedClass))
                return Parent?.Color ?? Color.Primary;

            return Color.Inherit;
        }
        #endregion
    }
}
