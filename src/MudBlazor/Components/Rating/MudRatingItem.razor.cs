﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudRatingItem : MudComponentBase
    {
        /// <summary>
        /// Space separated class names
        /// </summary>
        protected string ClassName =>
        new CssBuilder("")
          .AddClass($"mud-rating-item")
          .AddClass($"mud-ripple mud-ripple-icon", !DisableRipple)
          .AddClass($"yellow-text.text-darken-3", Color == Color.Default)
          .AddClass($"mud-{Color.ToDescriptionString()}-text", Color != Color.Default)
          .AddClass($"mud-rating-item-active", IsActive)
          .AddClass($"mud-disabled", Disabled)
          .AddClass($"mud-readonly", ReadOnly)
          .AddClass(Class)
        .Build();

        [CascadingParameter]
        private MudRating Rating { get; set; }

        /// <summary>
        /// This rating item value;
        /// </summary>
        [Parameter]
        public int ItemValue { get; set; }

        internal string ItemIcon { get; set; }

        internal bool IsActive { get; set; }

        private bool IsChecked => ItemValue == Rating?.SelectedValue;

        /// <summary>
        /// The Size of the icon.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

        /// <summary>
        /// If true, the controls will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// If true, the item will be readonly.
        /// </summary>
        [Parameter] public bool ReadOnly { get; set; }

        /// <summary>
        /// Fires when element clicked.
        /// </summary>
        [Parameter] public EventCallback<int> ItemClicked { get; set; }

        /// <summary>
        /// Fires when element hovered.
        /// </summary>
        [Parameter] public EventCallback<int?> ItemHovered { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            ItemIcon = SelectIcon();
        }

        private string SelectIcon()
        {
            if (Rating == null)
                return null;
            if (Rating.HoveredValue.HasValue && Rating.HoveredValue.Value >= ItemValue)
            {
                // full icon when @RatingItem hovered
                return Rating.FullIcon;
            }
            else if (Rating.SelectedValue >= ItemValue)
            {
                if (Rating.HoveredValue.HasValue && Rating.HoveredValue.Value < ItemValue)
                {
                    // empty icon when equal or higher RatingItem value clicked, but less value hovered 
                    return Rating.EmptyIcon;
                }
                else
                {
                    // full icon when equal or higher RatingItem value clicked
                    return Rating.FullIcon;
                }
            }
            else
            {
                // empty icon when this or higher RatingItem is not clicked and not hovered
                return Rating.EmptyIcon;
            }
        }

        // rating item lose hover
        private Task HandleMouseOut(MouseEventArgs e)
        {
            if (Disabled || Rating == null) 
                return Task.CompletedTask;

            IsActive = false;
            return ItemHovered.InvokeAsync(null);
        }

        private void HandleMouseOver(MouseEventArgs e)
        {
            if (Disabled) return;

            IsActive = true;
            ItemHovered.InvokeAsync(ItemValue);
        }

        private void HandleClick(MouseEventArgs e)
        {
            if (Disabled) return;
            IsActive = false;
            if (Rating?.SelectedValue == ItemValue)
            {
                ItemClicked.InvokeAsync(0);
            }
            else
            {
                ItemClicked.InvokeAsync(ItemValue);
            }
        }
    }
}
