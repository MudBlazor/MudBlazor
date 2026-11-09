// Copyright (c) MudBlazor 2025
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor.Interfaces;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// Represents a navigation bar component that allows for navigation between different sections of the application.
    /// </summary>
    public partial class MudNavigationBar : MudComponentBase, IAsyncDisposable
    {
        public MudNavigationBar()
        {
            using var registerScope = CreateRegisterScope();
            _color = registerScope.RegisterParameter<Color>(nameof(Color))
                .WithParameter(() => Color)
                .WithChangeHandler(OnParameterChanged);
            _typo = registerScope.RegisterParameter<Typo>(nameof(Typo))
                .WithParameter(() => Typo)
                .WithChangeHandler(OnParameterChanged);
            _ripple = registerScope.RegisterParameter<bool>(nameof(Ripple))
                .WithParameter(() => Ripple)
                .WithChangeHandler(OnParameterChanged);
            _hover = registerScope.RegisterParameter<bool>(nameof(Hover))
                .WithParameter(() => Hover)
                .WithChangeHandler(OnParameterChanged);
            _underline = registerScope.RegisterParameter<bool>(nameof(Underline))
                .WithParameter(() => Underline)
                .WithChangeHandler(OnParameterChanged);
            _dense = registerScope.RegisterParameter<bool>(nameof(Dense))
                .WithParameter(() => Dense)
                .WithChangeHandler(OnParameterChanged);
            _selectedClass = registerScope.RegisterParameter<string?>(nameof(SelectedClass))
                .WithParameter(() => SelectedClass)
                .WithChangeHandler(OnParameterChanged);
        }

        private readonly ParameterState<Color> _color;
        private readonly ParameterState<Typo> _typo;
        private readonly ParameterState<bool> _ripple;
        private readonly ParameterState<bool> _hover;
        private readonly ParameterState<bool> _underline;
        private readonly ParameterState<bool> _dense;
        private readonly ParameterState<string?> _selectedClass;

        private readonly List<MudNavigationBarItem> _items = new();
        protected internal string? _currentLocation;
        protected ElementReference _elementReference;

        protected string Classname =>
            new CssBuilder("mud-nav-bar mud-nav-bar-fixed")
                .AddClass("mud-nav-bar-dense", _dense.Value)
                .AddClass(Class)
                .Build();

        protected string FabClassname =>
            new CssBuilder("mud-nav-bar-fab")
                .AddClass(FabClass)
                .Build();

        protected string? Stylename => Style;

        /// <summary>
        /// The CSS classes that applies on selected items if set.
        /// </summary>
        [Parameter, ParameterState]
        [Category(CategoryTypes.List.Appearance)]
        public string? SelectedClass { get; set; }

        /// <summary>
        /// Additional CSS classes applied to the floating action button container.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? FabClass { get; set; }

        /// <summary>
        /// Shows a ripple effect when the user clicks on the navigation item.
        /// </summary>
        [Parameter, ParameterState]
        [Category(CategoryTypes.List.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// If true, changes background color slightly on hover.
        /// </summary>
        [Parameter, ParameterState]
        [Category(CategoryTypes.List.Appearance)]
        public bool Hover { get; set; } = true;

        /// <summary>
        /// Whether to underline the selected navigation item.
        /// </summary>
        [Parameter, ParameterState]
        [Category(CategoryTypes.List.Appearance)]
        public bool Underline { get; set; }

        /// <summary>
        /// If true, the navigation bar will have less height.
        /// </summary>
        [Parameter, ParameterState]
        [Category(CategoryTypes.List.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// The color of the selected navigation item.
        /// </summary>
        [Parameter, ParameterState]
        [Category(CategoryTypes.List.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// The typography option of the item text.
        /// </summary>
        [Parameter, ParameterState]
        [Category(CategoryTypes.List.Appearance)]
        public Typo Typo { get; set; } = Typo.subtitle2;

        /// <summary>
        /// Determines whether the current location matches an item's Href.
        /// </summary>
        /// <remarks>
        /// The first argument is the base-relative location, including its query string and fragment. The second argument is the item's Href.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public Func<string?, string?, bool>? CustomHrefComparisonFunc { get; set; }

        /// <summary>
        /// Fires when the navigation item changes.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public EventCallback<string?> OnSelectionChange { get; set; }

        /// <summary>
        /// Custom content to be rendered inside the navigation bar.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Custom content to be rendered as primary action right next to navigation bar.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public RenderFragment? FabContent { get; set; }

        protected override void OnInitialized()
        {
            NavigationManager.LocationChanged += OnLocationChanged;
            _currentLocation = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        }

        protected internal void Register(MudNavigationBarItem item)
        {
            _items.Add(item);

            if (!string.IsNullOrEmpty(item.Href) && MatchesHref(item, _currentLocation ?? string.Empty))
            {
                _ = InvokeAsync(() => SelectItemAsync(item));
            }

            StateHasChanged();
        }

        protected internal void Unregister(MudNavigationBarItem item)
        {
            _items.Remove(item);
            StateHasChanged();
        }

        /// <summary>
        /// Releases resources used by this component.
        /// </summary>
        public ValueTask DisposeAsync()
        {
            NavigationManager.LocationChanged -= OnLocationChanged;
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Navigates to an adjacent enabled navigation item.
        /// </summary>
        /// <param name="adjacentValue">
        /// The relative offset from the currently selected item.
        /// A positive value moves forward; a negative value moves backward.
        /// </param>
        public async Task NavigateAdjacentAsync(int adjacentValue)
        {
            if (_items.Count == 0 || adjacentValue == 0)
                return;

            var selectedIndex = _items.FindIndex(x => x._isSelected);

            if (selectedIndex < 0)
            {
                selectedIndex = adjacentValue > 0 ? -1 : _items.Count;
            }

            var direction = Math.Sign(adjacentValue);
            var remaining = Math.Abs((long)adjacentValue);
            var targetIndex = selectedIndex;

            while (remaining > 0)
            {
                do
                {
                    targetIndex += direction;

                    if (targetIndex < 0 || targetIndex >= _items.Count)
                        return;
                }
                while (_items[targetIndex].Disabled);

                remaining--;
            }

            var target = _items[targetIndex];

            await ActivateItemAsync(target);
        }

        /// <summary>
        /// Returns the Id (Text if id is null) of the selected item.
        /// </summary>
        /// <returns></returns>
        public string? GetSelectedItemId()
        {
            var relatedItem = _items.FirstOrDefault(x => x._isSelected);
            return relatedItem?.Id ?? relatedItem?.Text;
        }

        /// <summary>
        /// Clears the selection.
        /// </summary>
        /// <returns></returns>
        public Task ResetAsync()
        {
            return SelectItemAsync(null);
        }

        /// <summary>
        /// Obtains focus for component.
        /// </summary>
        public ValueTask FocusAsync() => _elementReference.FocusAsync();

        /// <summary>
        /// Obtains blur for component.
        /// </summary>
        public ValueTask BlurAsync() => _elementReference.MudBlurAsync();

        protected internal void Navigate(string url, bool forceLoad = false)
        {
            NavigationManager.NavigateTo(url, forceLoad);
        }

        protected internal async Task SelectItemAsync(MudNavigationBarItem? selectedItem)
        {
            var previous = _items.FirstOrDefault(x => x._isSelected);

            var selectionChanged = !ReferenceEquals(previous, selectedItem);

            foreach (var item in _items)
            {
                await item.SetSelectedAsync(ReferenceEquals(item, selectedItem));
            }

            if (selectionChanged)
            {
                await OnSelectionChange.InvokeAsync(selectedItem?.Id ?? selectedItem?.Text);
            }
        }

        protected internal async Task DeselectAllAsync()
        {
            foreach (var item in _items)
            {
                await item.SetSelectedAsync(false);
            }
        }

        protected void OnParameterChanged()
        {
            foreach (IMudStateHasChanged mudComponent in _items)
            {
                mudComponent.StateHasChanged();
            }

            StateHasChanged();
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs args)
        {
            _ = InvokeAsync(async () =>
            {
                _currentLocation = NavigationManager.ToBaseRelativePath(args.Location);
                await UpdateSelectionFromLocationAsync(_currentLocation);
            });
        }

        protected internal async Task ActivateItemAsync(MudNavigationBarItem item)
        {
            if (item.Disabled || !_items.Contains(item))
                return;

            if (!string.IsNullOrEmpty(item.Href))
            {
                NavigationManager.NavigateTo(item.Href);
                return;
            }

            await SelectItemAsync(item);
        }

        private async Task UpdateSelectionFromLocationAsync(string location)
        {
            if (!_items.Any(x => !string.IsNullOrEmpty(x.Href)))
                return;

            var matchedItem = _items.FirstOrDefault(item => MatchesHref(item, location));
            await SelectItemAsync(matchedItem);
        }

        private bool MatchesHref(MudNavigationBarItem item, string location)
        {
            if (string.IsNullOrEmpty(item.Href) || item.Disabled)
                return false;

            if (CustomHrefComparisonFunc is not null)
                return CustomHrefComparisonFunc(location, item.Href);

            var target = NavigationManager.ToAbsoluteUri(item.Href);
            var current = NavigationManager.ToAbsoluteUri(location);

            if (!string.Equals(target.Scheme, current.Scheme, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(target.Authority, current.Authority, StringComparison.OrdinalIgnoreCase))
                return false;

            var targetPath = target.AbsolutePath.TrimEnd('/');
            var currentPath = current.AbsolutePath.TrimEnd('/');

            if (targetPath.Length == 0)
                targetPath = "/";

            if (currentPath.Length == 0)
                currentPath = "/";

            var pathMatches = string.Equals(targetPath, currentPath, StringComparison.OrdinalIgnoreCase) ||
                (targetPath != "/" && currentPath.StartsWith(targetPath + "/", StringComparison.OrdinalIgnoreCase));

            return string.IsNullOrEmpty(target.Fragment) || string.Equals(target.Fragment, current.Fragment, StringComparison.Ordinal);
        }
    }
}
