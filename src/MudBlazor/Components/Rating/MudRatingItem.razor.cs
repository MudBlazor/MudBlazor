// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudRatingItem : MudComponentBase
    {
        /// <summary>
        /// Space separated class names.
        /// </summary>
        protected string ClassName =>
            new CssBuilder("mud-rating-item")
                .AddClass($"mud-ripple mud-ripple-icon", Ripple)
                .AddClass($"yellow-text.text-darken-3", Color == Color.Default)
                .AddClass($"mud-{Color.ToDescriptionString()}-text", Color != Color.Default)
                .AddClass($"mud-rating-item-active", Active)
                .AddClass($"mud-disabled", Disabled)
                .AddClass($"mud-readonly", ReadOnly)
                .AddClass(Class)
                .Build();

        [CascadingParameter]
        private MudRating? Rating { get; set; }

        /// <summary>
        /// This rating item value.
        /// </summary>
        [Parameter]
        public int ItemValue { get; set; }

        /// <summary>
        /// The size of the icon.
        /// </summary>
        [Parameter]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// Gets or sets whether to show a ripple effect when the user clicks the button. Default is true.
        /// </summary>
        [Parameter]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// If true, the controls will be disabled.
        /// </summary>
        [Parameter]
        public bool Disabled { get; set; }

        /// <summary>
        /// If true, the item will be readonly.
        /// </summary>
        [Parameter]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Fires when element clicked.
        /// </summary>
        [Parameter]
        public EventCallback<int> ItemClicked { get; set; }

        /// <summary>
        /// Fires when element hovered.
        /// </summary>
        [Parameter]
        public EventCallback<int?> ItemHovered { get; set; }

        internal Color ItemColor { get; set; }

        internal string? ItemIcon { get; set; }

        internal bool Active { get; set; }

        private bool Checked => ItemValue == Rating?.GetState<int>(nameof(Rating.SelectedValue));

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            ItemIcon = SelectIcon();
            ItemColor = SelectIconColor();
        }

        internal string? SelectIcon()
        {
            if (Rating is null)
            {
                return null;
            }

            if (Rating.HoveredValue.HasValue && Rating.HoveredValue.Value >= ItemValue)
            {
                // full icon when RatingItem hovered
                return Rating.FullIcon;
            }

            var ratingSelectedValue = Rating.GetState<int>(nameof(Rating.SelectedValue));
            if (ratingSelectedValue >= ItemValue)
            {
                if (Rating.HoveredValue.HasValue && Rating.HoveredValue.Value < ItemValue)
                {
                    // empty icon when equal or higher RatingItem value clicked, but less value hovered
                    return Rating.EmptyIcon;
                }

                // full icon when equal or higher RatingItem value clicked
                return Rating.FullIcon;
            }

            // empty icon when this or higher RatingItem is not clicked and not hovered
            return Rating.EmptyIcon;
        }

        internal Color SelectIconColor()
        {
            if (Rating is null)
            {
                return Color.Inherit;
            }

            if (Rating.FullIconColor is null || Rating.EmptyIconColor is null)
            {
                return Color.Inherit;
            }

            if (Rating.HoveredValue.HasValue && Rating.HoveredValue.Value >= ItemValue)
            {
                // full icon color when RatingItem hovered
                return Rating.FullIconColor.Value;
            }

            var ratingSelectedValue = Rating.GetState<int>(nameof(Rating.SelectedValue));
            if (ratingSelectedValue >= ItemValue)
            {
                if (Rating.HoveredValue.HasValue && Rating.HoveredValue.Value < ItemValue)
                {
                    // empty icon color when equal or higher RatingItem value clicked, but less value hovered
                    return Rating.EmptyIconColor.Value;
                }

                // full icon color when equal or higher RatingItem value clicked
                return Rating.FullIconColor.Value;
            }

            // empty icon color when this or higher RatingItem is not clicked and not hovered
            return Rating.EmptyIconColor.Value;
        }

        // rating item lose hover
        internal Task HandlePointerOutAsync(PointerEventArgs e)
        {
            if (Disabled || Rating is null)
            {
                return Task.CompletedTask;
            }

            Active = false;

            return ItemHovered.InvokeAsync(null);
        }

        internal Task HandlePointerOverAsync(PointerEventArgs e)
        {
            if (Disabled)
            {
                return Task.CompletedTask;
            }

            Active = true;

            return ItemHovered.InvokeAsync(ItemValue);
        }

        private Task HandleClickAsync()
        {
            if (Disabled)
            {
                return Task.CompletedTask;
            }

            Active = false;
            var ratingSelectedValue = Rating?.GetState<int>(nameof(Rating.SelectedValue));

            return ItemClicked.InvokeAsync(ratingSelectedValue == ItemValue ? 0 : ItemValue);
        }
    }
}
