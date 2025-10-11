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

        #region Fields & Parameter State
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
                .AddClass("mud-nav-bar-dense", Dense)
                .AddClass(Class)
                .Build();

        protected string FabClassname =>
            new CssBuilder("mud-nav-bar-fab")
                .AddClass(FabClass)
                .Build();

        protected string Stylename => new StyleBuilder()
            .AddStyle("grid-template-columns", $"repeat({_items.Count}, minmax(0, 1fr))")
            .AddStyle(Style)
            .Build();
        #endregion

        #region Parameters
        /// <summary>
        /// The CSS classes that applies on selected items if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? SelectedClass { get; set; }

        /// <summary>
        /// The CSS classes that applies on selected items if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? FabClass { get; set; }

        /// <summary>
        /// Shows a ripple effect when the user clicks on the navigation item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// If true, changes background color slightly on hover.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Hover { get; set; } = true;

        /// <summary>
        /// Whether to underline the selected navigation item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Underline { get; set; } = true;

        /// <summary>
        /// If true, the navigation bar will have less height.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// The color of the selected navigation item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// The typography option of the item text.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public Typo Typo { get; set; } = Typo.subtitle2;

        /// <summary>
        /// The custom comparison function to determine if the navigation location matches with href. The given parameters are the location and href in order.
        /// </summary>
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
        #endregion

        #region Lifecycle
        protected override void OnInitialized()
        {
            NavigationManager.LocationChanged += OnLocationChangedAsync;
            _currentLocation = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        }

        protected internal void Register(MudNavigationBarItem item)
        {
            _items.Add(item);
            StateHasChanged();
        }

        protected internal void Unregister(MudNavigationBarItem item)
        {
            _items.Remove(item);
            StateHasChanged();
        }

        public ValueTask DisposeAsync()
        {
            NavigationManager.LocationChanged -= OnLocationChangedAsync;
            return ValueTask.CompletedTask;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Navigates next or previous item with defined value.
        /// </summary>
        /// <param name="adjacentValue">The integer value that </param>
        /// <returns></returns>
        public async Task NavigateAdjacentAsync(int adjacentValue)
        {
            var item = _items.FirstOrDefault(x => x._isSelected);
            if (item == null)
            {
                await DeselectAllAsync();
                await _items[0].SetSelectedAsync(true);
            }
            else if (item.Disabled)
            {
                await NavigateAdjacentAsync(adjacentValue > 0 ? 1 : -1);
                return;
            }
            else
            {
                var index = _items.IndexOf(item) + adjacentValue;
                if (index < 0 || index >= _items.Count)
                {
                    return;
                }
                var relatedItem = _items[index];
                await DeselectAllAsync();
                await relatedItem.SetSelectedAsync(true);
            }
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
        public async Task ResetAsync()
        {
            await DeselectAllAsync();
        }

        /// <summary>
        /// Obtains focus for component.
        /// </summary>
        public ValueTask FocusAsync() => _elementReference.FocusAsync();

        /// <summary>
        /// Obtains blur for component.
        /// </summary>
        public ValueTask BlurAsync() => _elementReference.MudBlurAsync();
        #endregion

        #region Protected || Internal Methods
        protected internal void Navigate(string url, bool forceLoad = false)
        {
            NavigationManager.NavigateTo(url, forceLoad);
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

        protected async void OnLocationChangedAsync(object? sender, LocationChangedEventArgs args)
        {
            _currentLocation = NavigationManager.ToBaseRelativePath(args.Location);
            foreach (var item in _items)
            {
                await item.HandleHrefSelectedAsync(_currentLocation);
            }
        }
        #endregion

    }
}
