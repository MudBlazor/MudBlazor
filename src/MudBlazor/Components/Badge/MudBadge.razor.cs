﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents an overlay added to an icon or button to add information such as a number of new items.
    /// </summary>
    public partial class MudBadge : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-badge-root")
            .AddClass(Class)
            .Build();

        protected string WrapperClass => new CssBuilder("mud-badge-wrapper")
            .AddClass($"mud-badge-{Origin.ToDescriptionString().Replace("-", " ")}")
            .Build();

        protected string BadgeClassName => new CssBuilder("mud-badge")
            .AddClass("mud-badge-dot", Dot)
            .AddClass("mud-badge-bordered", Bordered)
            .AddClass("mud-badge-icon", !string.IsNullOrEmpty(Icon) && !Dot)
            .AddClass($"mud-badge-{Origin.ToDescriptionString().Replace("-", " ")}")
            .AddClass($"mud-elevation-{Elevation.ToString()}")
            .AddClass("mud-theme-" + Color.ToDescriptionString(), Color != Color.Default)
            .AddClass("mud-badge-default", Color == Color.Default)
            .AddClass("mud-badge-overlap", Overlap)
            .AddClass(BadgeClass)
            .Build();

        /// <summary>
        /// Gets or sets the location of the badge.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Origin.TopRight" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Badge.Appearance)]
        public Origin Origin { get; set; } = Origin.TopRight;

        /// <summary>
        /// Gets or sets the size of the drop shadow.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.  A higher number creates a heavier drop shadow.  Use a value of <c>0</c> for no shadow.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Badge.Appearance)]
        public int Elevation { set; get; } = 0;

        /// <summary>
        /// Gets or sets whether the badge can be seen.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Badge.Behavior)]
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Gets or sets the color of the badge.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.  Theme colors are supported.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Badge.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// Gets or sets whether a dot is displayed instead of any content.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Badge.Behavior)]
        public bool Dot { get; set; }

        /// <summary>
        /// Gets or sets whether to display <see cref="ChildContent"/> over the main badge content.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Badge.Appearance)]
        public bool Overlap { get; set; }

        /// <summary>
        /// Gets or sets whether a border is displayed around the badge.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Badge.Appearance)]
        public bool Bordered { get; set; }

        /// <summary>
        /// Gets or sets the icon to display in the badge.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Badge.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// Gets or sets the maximum number allowed in the <see cref="Content"/> property.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>99</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Badge.Behavior)]
        public int Max { get; set; } = 99;

        /// <summary>
        /// Gets or sets the <c>string</c> or <c>int</c> value to display inside the badge.
        /// </summary>
        /// <remarks>
        /// Supported types are <c>string</c> and <c>int</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Badge.Behavior)]
        public object? Content { get; set; }

        /// <summary>
        /// Gets or sets any CSS classes applied to the badge.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  You can use spaces to separate multiple classes.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Badge.Appearance)]
        public string? BadgeClass { get; set; }

        /// <summary>
        /// Gets or sets any child content for the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Badge.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Occurs when the badge has been clicked.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

        private string? _content;

        internal Task HandleBadgeClick(MouseEventArgs e)
        {
            if (OnClick.HasDelegate)
                return OnClick.InvokeAsync(e);

            return Task.CompletedTask;
        }

        protected override void OnParametersSet()
        {
            if (Content is string stringContent)
            {
                _content = stringContent;
            }
            else if (Content is int numberContent)
            {
                if (numberContent > Max)
                {
                    _content = Max + "+";
                }
                else
                {
                    _content = numberContent.ToString();
                }
            }
            else
            {
                _content = null;
            }
        }
    }
}
