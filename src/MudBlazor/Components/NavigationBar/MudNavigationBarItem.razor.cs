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


        internal bool _isSelected;

        [CascadingParameter]
        public MudNavigationBar? Parent { get; set; }

        protected string Classname =>
            new CssBuilder("mud-nav-bar-item")
                .AddClass("mud-ripple", Parent?.Ripple == true && !Disabled)
                .AddClass($"border-solid border-b-2 mud-border-{Parent?.Color.ToDescriptionString() ?? "primary"}", Parent?.Underline == true && _isSelected)
                .AddClass("mud-disabled", Disabled)
                .AddClass($"mud-nav-bar-item-selected {SelectedClass}", _isSelected)
                .AddClass(Class)
                .Build();

        /// <summary>
        /// The icon to display in the navigation item.
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
        /// The custom content of the navigation item.
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

        protected override async Task OnInitializedAsync()
        {
            base.OnInitialized();
            Parent?.Register(this);
            await HandleHrefSelected(Parent?._currentLocation);
        }

        protected internal async Task HandleHrefSelected(string? location)
        {
            if (Parent == null)
            {
                await SetSelected(false);
            }

            if (!string.IsNullOrEmpty(Href))
            {
                await SetSelected(CompareHrefRoute(Href, location));
            }
        }

        private bool CompareHrefRoute(string href, string? location)
        {
            if (href.Contains('#'))
            {
                href = href.Substring(0, href.IndexOf('#'));
            }
            return location?.StartsWith(href) ?? false;
        }

        protected internal async Task SetSelected(bool selected)
        {
            bool callChange = _isSelected != selected;
            _isSelected = selected;
            if (callChange && Parent != null)
            {
                await Parent.OnSelectionChange.InvokeAsync(Id ?? Text);
            }
            StateHasChanged();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Parent?.Unregister(this);
            }
        }

        protected async Task HandleClick(MouseEventArgs args)
        {
            if (Disabled)
                return;

            if (!string.IsNullOrEmpty(Href))
            {
                Parent?.DeselectAll();
                await HandleHrefSelected(Href);
                Parent?.Navigate(Href);
            }
            else
            {
                Parent?.DeselectAll();
                await SetSelected(true);
            }
            await OnClick.InvokeAsync(args);
        }

        protected Color GetColor()
        {
            if (Disabled)
            {
                return Color.Inherit;
            }
            else if (_isSelected)
            {
                if (string.IsNullOrEmpty(SelectedClass))
                {
                    return Parent?.Color ?? Color.Primary;
                }
                else
                {
                    return Color.Inherit;
                }
            }
            else
            {
                return Color.Inherit;
            }
        }

        public ValueTask DisposeAsync()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }
    }
}
