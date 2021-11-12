﻿using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudNavGroup : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-nav-group")
          .AddClass(Class)
          .AddClass($"mud-nav-group-disabled", Disabled)
          .Build();

        protected string ButtonClassname =>
        new CssBuilder("mud-nav-link")
          .AddClass($"mud-ripple", !DisableRipple)
          .AddClass("mud-expanded", Expanded)
          .Build();

        protected string IconClassname =>
        new CssBuilder("mud-nav-link-icon")
          .AddClass($"mud-nav-link-icon-default", IconColor == Color.Default)
          .Build();

        protected string ExpandIconClassname =>
        new CssBuilder("mud-nav-link-expand-icon")
          .AddClass($"mud-transform", Expanded && !Disabled)
          .AddClass($"mud-transform-disabled", Expanded && Disabled)
          .Build();

        [Parameter] public string Title { get; set; }

        /// <summary>
        /// Icon to use if set.
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors, default value uses the themes drawer icon color.
        /// </summary>
        [Parameter] public Color IconColor { get; set; } = Color.Default;

        /// <summary>
        /// If true, the button will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

        private bool _expanded;
        /// <summary>
        /// If true, expands the nav group, otherwise collapse it. 
        /// Two-way bindable
        /// </summary>
        [Parameter]
        public bool Expanded
        {
            get => _expanded;
            set
            {
                if (_expanded == value)
                    return;

                _expanded = value;
                ExpandedChanged.InvokeAsync(_expanded);
            }
        }

        [Parameter] public EventCallback<bool> ExpandedChanged { get; set; }

        /// <summary>
        /// If true, hides expand-icon at the end of the NavGroup.
        /// </summary>
        [Parameter] public bool HideExpandIcon { get; set; }

        /// <summary>
        /// Explicitly sets the height for the Collapse element to override the css default.
        /// </summary>
        [Parameter] public int? MaxHeight { get; set; }

        /// <summary>
        /// If set, overrides the default expand icon.
        /// </summary>
        [Parameter] public string ExpandIcon { get; set; } = @Icons.Filled.ArrowDropDown;
        [Parameter] public RenderFragment ChildContent { get; set; }

        protected void ExpandedToggle()
        {
            _expanded = !Expanded;
            ExpandedChanged.InvokeAsync(_expanded);
        }
    }
}
