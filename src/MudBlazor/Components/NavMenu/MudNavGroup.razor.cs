// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A deeper level of navigation links as part of a <see cref="MudNavMenu"/>.
    /// </summary>
    /// <seealso cref="MudNavLink"/>
    /// <seealso cref="MudNavMenu"/>
    public partial class MudNavGroup : MudComponentBase
    {
        private readonly ParameterState<bool> _expandedState;
        private readonly ParameterState<bool> _disabledState;
        private readonly ParameterState<NavigationContext?> _parentNavigationContextState;
        private NavigationContext _navigationContext = new(false, true);

        public MudNavGroup()
        {
            using var registerScope = CreateRegisterScope();
            _disabledState = registerScope.RegisterParameter<bool>(nameof(Disabled))
                .WithParameter(() => Disabled)
                .WithChangeHandler(UpdateNavigationContext);
            _parentNavigationContextState = registerScope.RegisterParameter<NavigationContext?>(nameof(ParentNavigationContext))
                .WithParameter(() => ParentNavigationContext)
                .WithChangeHandler(UpdateNavigationContext);
            _expandedState = registerScope.RegisterParameter<bool>(nameof(Expanded))
                .WithParameter(() => Expanded)
                .WithEventCallback(() => ExpandedChanged)
                .WithChangeHandler(UpdateNavigationContext);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            UpdateNavigationContext();
        }

        protected string Classname =>
            new CssBuilder("mud-nav-group")
                .AddClass(Class)
                .AddClass("mud-nav-group-disabled", _disabledState.Value)
                .Build();

        protected string ButtonClassname =>
            new CssBuilder("mud-nav-link")
                .AddClass($"mud-ripple", Ripple)
                .AddClass("mud-expanded", _expandedState.Value)
                .AddClass(HeaderClass)
                .Build();

        protected string IconClassname =>
            new CssBuilder("mud-nav-link-icon")
                .AddClass("mud-nav-link-icon-default", IconColor == Color.Default)
                .Build();

        protected string ExpandIconClassname =>
            new CssBuilder("mud-nav-link-expand-icon")
                .AddClass("mud-transform", _expandedState.Value && _disabledState.Value is false)
                .AddClass("mud-transform-disabled", _expandedState.Value && _disabledState.Value)
                .Build();

        protected int ButtonTabIndex => _disabledState.Value || _parentNavigationContextState.Value is { Disabled: true } or { Expanded: false } ? -1 : 0;

        [CascadingParameter]
        private NavigationContext? ParentNavigationContext { get; set; }

        /// <summary>
        /// The CSS classes applied to this nav group title.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  You can use spaces to separate multiple classes.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public string? HeaderClass { get; set; }

        /// <summary>
        /// The content within the title area.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When set, overrides the <see cref="Title"/> property.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public RenderFragment? TitleContent { get; set; }

        /// <summary>
        /// The text shown for this group.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public string? Title { get; set; }

        /// <summary>
        /// The icon displayed next to the <see cref="Title"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// The color of the icon when <see cref="Icon"/> is set.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public Color IconColor { get; set; } = Color.Default;

        /// <summary>
        /// Prevents the user from interacting with this group.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Shows a ripple effect when the user clicks this group.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// Displays the items within this group.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When this value changes, <see cref="ExpandedChanged"/> occurs.  Can be bound via <c>@bind-Expanded</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public bool Expanded { get; set; }

        /// <summary>
        /// Hides the expand/collapse icon.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public bool HideExpandIcon { get; set; }

        /// <summary>
        /// The maximum height, in pixels, of this group.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When set, it will override the CSS default.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public int? MaxHeight { get; set; }

        /// <summary>
        /// The icon for expanding and collapsing this group.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ArrowDropDown"/>.  Only shows when <see cref="HideExpandIcon"/> is <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Appearance)]
        public string ExpandIcon { get; set; } = Icons.Material.Filled.ArrowDropDown;

        /// <summary>
        /// The content within this group.
        /// </summary>
        /// <remarks>
        /// Typically contains <see cref="MudNavGroup"/> and <see cref="MudNavLink"/> components.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.NavMenu.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Occurs when <see cref="Expanded"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> ExpandedChanged { get; set; }

        private async Task ExpandedToggleAsync()
        {
            await _expandedState.SetValueAsync(!_expandedState.Value);
            UpdateNavigationContext();
        }

        private void UpdateNavigationContext()
            => _navigationContext = _navigationContext with
            {
                Disabled = _disabledState.Value || _parentNavigationContextState.Value is { Disabled: true },
                Expanded = _expandedState.Value
                           && _parentNavigationContextState.Value is null or { Expanded: true }
            };
    }
}
