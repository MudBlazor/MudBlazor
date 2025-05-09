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
    public partial class MudNavigationBar : MudComponentBase
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
            _underline = registerScope.RegisterParameter<bool>(nameof(Underline))
                .WithParameter(() => Underline)
                .WithChangeHandler(OnParameterChanged);
            _density = registerScope.RegisterParameter<short>(nameof(Density))
                .WithParameter(() => Density)
                .WithChangeHandler(OnParameterChanged);
        }

        private readonly ParameterState<Color> _color;
        private readonly ParameterState<Typo> _typo;
        private readonly ParameterState<bool> _ripple;
        private readonly ParameterState<bool> _underline;
        private readonly ParameterState<short> _density;

        private readonly List<MudNavigationBarItem> _items = new();
        protected internal string? _currentLocation;

        protected string Classname =>
            new CssBuilder("mud-nav-bar mud-nav-bar-fixed")
                .AddClass($"mud-density-layout-{(0 > Density ? $"n{Math.Abs(Density)}" : Math.Abs(Density))}")
                .AddClass(Class)
                .Build();

        protected string Stylename => new StyleBuilder()
            .AddStyle("grid-template-columns", $"repeat({_items.Count}, minmax(0, 1fr))")
            .AddStyle(Style)
            .Build();
        #endregion

        #region Parameters
        /// <summary>
        /// Shows a ripple effect when the user clicks on the navigation item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// Whether to underline the selected navigation item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public bool Underline { get; set; } = true;

        /// <summary>
        /// The density of the navigation bar. A positive value increases the density, while a negative value decreases it. The range is between -5 to 5. Default height equals 4rem (64px).
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public short Density { get; set; } = 0;

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
        #endregion


        protected override void OnInitialized()
        {
            NavigationManager.LocationChanged += OnLocationChanged;
            _currentLocation = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        }

        private async void OnLocationChanged(object? sender, LocationChangedEventArgs args)
        {
            _currentLocation = NavigationManager.ToBaseRelativePath(args.Location);
            foreach (var item in _items)
            {
                await item.HandleHrefSelected(_currentLocation);
            }
        }

        protected internal void Navigate(string url, bool forceLoad = false)
        {
            NavigationManager.NavigateTo(url, forceLoad);
        }

        protected internal async Task ArrangeSelection(MudNavigationBarItem senderItem)
        {
            foreach (var item in _items)
            {
                if (item == senderItem)
                {
                    await item.SetSelected(true);
                }
                else
                {
                    await item.SetSelected(false);
                }
            }
        }

        protected internal async Task DeselectAll()
        {
            foreach (var item in _items)
            {
                await item.SetSelected(false);
            }
        }

        private void OnParameterChanged()
        {
            foreach (IMudStateHasChanged mudComponent in _items)
            {
                mudComponent.StateHasChanged();
            }

            StateHasChanged();
        }

        public string? GetSelectedItemId()
        {
            var relatedItem = _items.FirstOrDefault(x => x._isSelected);
            return relatedItem?.Id ?? relatedItem?.Text;
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
    }
}
