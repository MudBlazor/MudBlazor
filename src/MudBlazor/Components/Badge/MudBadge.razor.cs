using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudBadge : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-badge")
          .AddClass(Class)
        .Build();

        protected string BadgeClass =>
        new CssBuilder("mud-badge-badge")
            .AddClass("mud-badge-dot", Dot)
            .AddClass("mud-badge-bordered", Bordered)
            .AddClass("mud-badge-icon", !string.IsNullOrEmpty(Icon) && !Dot)
            .AddClass("mud-theme-" + Color.ToDescriptionString())
            .AddClass("mud-badge-top", !Bottom)
            .AddClass("mud-badge-bottom", Bottom)
            .AddClass("mud-badge-right", Start == RightToLeft)
            .AddClass("mud-badge-left", Start != RightToLeft)
            .AddClass("mud-badge-overlap", Overlap)
        .Build();

        [CascadingParameter] public bool RightToLeft { get; set; }

        /// <summary>
        /// The color of the badge.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// Aligns the badge to bottom.
        /// </summary>
        [Parameter] public bool Bottom { get; set; }

        /// <summary>
        /// Aligns the badge to left.
        /// </summary>
        [ObsoleteAttribute("Left is obsolete. Use Start instead!", false)]
        [Parameter] public bool Left { get => Start; set { Start = value; } }

        /// <summary>
        /// Aligns the badge to the start (Left in LTR and right in RTL).
        /// </summary>
        [Parameter] public bool Start { get; set; }

        /// <summary>
        /// Reduces the size of the badge and hide any of its content.
        /// </summary>
        [Parameter] public bool Dot { get; set; }

        /// <summary>
        /// Overlaps the childcontent on top of the content.
        /// </summary>
        [Parameter] public bool Overlap { get; set; }

        /// <summary>
        /// Applies a border around the badge.
        /// </summary>
        [Parameter] public bool Bordered { get; set; }

        /// <summary>
        /// Sets the Icon to use in the badge.
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// Max value to show when content is integer type.
        /// </summary>
        [Parameter] public int Max { get; set; } = 99;

        /// <summary>
        /// Content you want inside the badge. Supported types are string and integer.
        /// </summary>
        [Parameter] public object Content { get; set; }

        /// <summary>
        /// Child content of component, the content that the badge will apply to.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Button click event if set.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

        private string _content;

        private async Task HandleBadgeClick(MouseEventArgs e)
        {
            if (OnClick.HasDelegate)
                await OnClick.InvokeAsync(e);
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
