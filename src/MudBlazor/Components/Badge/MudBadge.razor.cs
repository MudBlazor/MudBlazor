using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudBadge : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-badge-root")
          .AddClass(Class)
        .Build();
        protected string WrapperClass =>
        new CssBuilder("mud-badge-wrapper")
            .AddClass($"mud-badge-{Origin.ToDescriptionString().Replace("-", " ")}")
        .Build();

        protected string BadgeClassName =>
        new CssBuilder("mud-badge")
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
        /// The placement of the badge.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Badge.Appearance)]
        public Origin Origin { get; set; } = Origin.TopRight;

        /// <summary>
        /// The higher the number, the heavier the drop-shadow.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Badge.Appearance)]
        public int Elevation { set; get; } = 0;

        /// <summary>
        /// The visibility of the badge.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Badge.Behavior)]
        public bool Visible { get; set; } = true;

        /// <summary>
        /// The color of the badge.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Badge.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// Aligns the badge to bottom.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use Origin instead.", true)]
        [Parameter] public bool Bottom { get; set; }

        /// <summary>
        /// Aligns the badge to left.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use Origin instead.", true)]
        [Parameter] public bool Left { get => Start; set { Start = value; } }

        /// <summary>
        /// Aligns the badge to the start (Left in LTR and right in RTL).
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use Origin instead.", true)]
        [Parameter] public bool Start { get; set; }

        /// <summary>
        /// Reduces the size of the badge and hide any of its content.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Badge.Behavior)]
        public bool Dot { get; set; }

        /// <summary>
        /// Overlaps the childcontent on top of the content.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Badge.Appearance)]
        public bool Overlap { get; set; }

        /// <summary>
        /// Applies a border around the badge.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Badge.Appearance)]
        public bool Bordered { get; set; }

        /// <summary>
        /// Sets the Icon to use in the badge.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Badge.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// Max value to show when content is integer type.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Badge.Behavior)]
        public int Max { get; set; } = 99;

        /// <summary>
        /// Content you want inside the badge. Supported types are string and integer.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Badge.Behavior)]
        public object? Content { get; set; }

        /// <summary>
        /// Badge class names, separated by space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Badge.Appearance)]
        public string? BadgeClass { get; set; }

        /// <summary>
        /// Child content of component, the content that the badge will apply to.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Badge.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Button click event if set.
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
