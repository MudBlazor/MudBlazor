// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{

    /// <summary>
    /// A clickable item as part of a <see cref="MudRating"/>.
    /// </summary>
    /// <seealso cref="MudRating"/>
    public partial class MudRatingItem : MudComponentBase
    {
        /// <summary>
        /// The CSS classes applied to this component.
        /// </summary>
        protected string ClassName =>
            new CssBuilder("mud-rating-item")
                .AddClass("mud-ripple", Ripple && !ReadOnly)
                .AddClass($"mud-{Color.ToStringFast(true)}-text", Color != Color.Default)
                .AddClass("mud-rating-item-active", Active)
                .AddClass("mud-disabled", Disabled)
                .AddClass("mud-readonly", ReadOnly)
                .AddClass(Class)
                .Build();

        /// <summary>
        /// The parent <see cref="MudRating"/> containing this item.
        /// </summary>
        [CascadingParameter]
        private MudRating? Rating { get; set; }

        /// <summary>
        /// The value for this item.
        /// </summary>
        /// <remarks>
        /// Defaults to the index of this item in the parent <see cref="MudRating"/>.  (e.g. The 3rd item has a value of <c>3</c>.)
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Data)]
        public int ItemValue { get; set; }

        /// <summary>
        /// The size of this item.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.  When used within a <see cref="MudRating"/>, the value of <see cref="MudRating.Size"/> is applied instead.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The color of this item.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.  When used within a <see cref="MudRating"/>, the value of <see cref="MudRating.Color"/> is applied instead.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// Show a ripple effect when the user clicks the button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When used within a <see cref="MudRating"/>, the value of <see cref="MudRating.Ripple"/> is applied instead.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public bool Ripple { get; set; } = false;

        /// <summary>
        /// Prevents the user from interacting with this item, and uses a disabled style.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Prevents this item from being changed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Behavior)]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Occurs when this item is clicked.
        /// </summary>
        /// <remarks>
        /// When clicked, the <see cref="MudRating.SelectedValue"/> is changed.
        /// </remarks>
        [Parameter]
        public EventCallback<int> ItemClicked { get; set; }

        /// <summary>
        /// Occurs when the user hovers over this item.
        /// </summary>
        /// <remarks>
        /// When hovered, the <see cref="MudRating.HoveredValue"/> is changed.
        /// </remarks>
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
            if (Disabled || ReadOnly || Rating is null)
            {
                return Task.CompletedTask;
            }

            Active = false;

            return ItemHovered.InvokeAsync(null);
        }

        internal Task HandlePointerOverAsync(PointerEventArgs e)
        {
            if (Disabled || ReadOnly)
            {
                return Task.CompletedTask;
            }

            Active = true;

            return ItemHovered.InvokeAsync(ItemValue);
        }

        private Task HandleClickAsync()
        {
            if (Disabled || ReadOnly)
            {
                return Task.CompletedTask;
            }

            Active = false;
            var ratingSelectedValue = Rating?.GetState<int>(nameof(Rating.SelectedValue));

            return ItemClicked.InvokeAsync(ratingSelectedValue == ItemValue ? 0 : ItemValue);
        }
    }
}
