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
    /// <summary>
    /// This component can show a partially filled star. This is normally only used in ReadOnly mode because a
    /// mouse click will set an integer value. When set to a non integer value, it uses the icons as follows:<br/>
    /// 1.0 - 1.25 = 1 star<br/>
    /// 1.26 - 1.75 = 1.5 stars<br/>
    /// 1.76 - 2.25 = 2 stars
    /// </summary>
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
        /// This rating item value. This is the index of this item in the list of items displayed by the
        /// parent Rating component.
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

        private bool Checked
        {
            get
            {
                if (Rating is null)
                {
                    return false;
                }
                var value = Rating.GetState<decimal>(nameof(Rating.Value));
                return ItemValue == (int)Math.Round(value);
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            ItemIcon = SelectIcon();
            ItemColor = SelectIconColor();
        }

        /// <summary>
        /// The icon to show for this icon based on the RatingValue.
        /// </summary>
        private enum IconState
        {
            /// <summary>
            /// The full icon. Use if Rating.Value >= ItemValue - 0.25
            /// </summary>
            Full,
            /// <summary>
            /// The half icon. Use if Rating.Value > ItemValue - 0.75 &amp; Rating.Value &lt; ItemValue - 0.25
            /// </summary>
            Half,
            /// <summary>
            /// The empty icon. Use if Rating.Value &lt;= ItemValue - 0.75
            /// </summary>
            Empty
        };

        private IconState GetIconState()
        {
            if (Rating is null)
            {
                return IconState.Empty;
            }
            var ratingValue = Rating.GetState<decimal>(nameof(Rating.Value));
            if (ratingValue >= ItemValue - 0.25m)
            {
                return IconState.Full;
            }
            if (ratingValue > ItemValue - 0.75m && ratingValue < ItemValue - 0.25m)
            {
                return IconState.Half;
            }
            return IconState.Empty;
        }

        internal string? SelectIcon()
        {
            if (Rating is null)
            {
                return null;
            }

            // hover is full/empty icons
            if (Rating.HoveredValue.HasValue)
            {
                // full icon when RatingItem hovered
                if (Rating.HoveredValue.Value >= ItemValue)
                    return Rating.FullIcon;
                // empty icon when equal or higher RatingItem value clicked, but less value hovered
                return Rating.EmptyIcon;
            }

            return GetIconState() switch
            {
                IconState.Full => Rating.FullIcon,
                IconState.Half => Rating.HalfIcon,
                IconState.Empty => Rating.EmptyIcon,
                _ => Rating.EmptyIcon
            };
        }

        internal Color SelectIconColor()
        {
            if (Rating is null)
            {
                return Color.Inherit;
            }

            // hover is full/empty icons
            if (Rating.HoveredValue.HasValue)
            {
                if (Rating.HoveredValue.Value >= ItemValue)
                {
                    // full icon color when RatingItem hovered
                    return Rating.FullIconColor ?? Color.Inherit;
                }

                // empty icon color when equal or higher RatingItem value clicked, but less value hovered
                return Rating.EmptyIconColor ?? Color.Inherit;
            }

            return GetIconState() switch
            {
                IconState.Full => Rating.FullIconColor ?? Color.Inherit,
                IconState.Half => Rating.HalfIconColor ?? Color.Inherit,
                IconState.Empty => Rating.EmptyIconColor ?? Color.Inherit,
                _ => Color.Inherit
            };
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

            // this is a click so we use the int value.
            Active = false;
            var ratingValue = Rating?.GetState<decimal>(nameof(Rating.Value));
            if (ratingValue is null)
                return ItemClicked.InvokeAsync(ItemValue);

            return ItemClicked.InvokeAsync((int)Math.Round(ratingValue.Value) == ItemValue ? 0 : ItemValue);
        }
    }
}
