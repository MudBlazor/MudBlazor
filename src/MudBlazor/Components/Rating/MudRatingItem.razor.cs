using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Text;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudRatingItem : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("")
          .AddClass($"mud-rating-item")
          .AddClass($"mud-svg-icon-root")
          .AddClass($"mud-ripple mud-ripple-icon")
          //.AddClass($"mud-icon-root", !String.IsNullOrEmpty(FontIcon))
          
          .AddClass($"yellow-text.text-darken-3", !Color.HasValue)
          .AddClass($"mud-color-text-{(Color.HasValue ? Color.Value.ToDescriptionString() : string.Empty)}", Color.HasValue)
          .AddClass($"mud-icon-size-{Size.ToDescriptionString()}")
          .AddClass($"mud-rating-item-active", IsActive)
          //.AddClass(FontClass, !String.IsNullOrEmpty(FontClass))
          

          .AddClass($"mud-disabled", Disabled)
          .AddClass(Class)
        .Build();

        [CascadingParameter]
        MudRating Rating { get; set; }

        [Parameter]
        public int ItemValue { get; set; }

        public string ItemIcon { get; set; }

        public bool IsActive { get; set; }

        private bool IsChecked => ItemValue == Rating.SelectedValue;

        /// <summary>
        /// The Size of the icon.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color? Color { get; set; } = null;

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

        /// <summary>
        /// If true, the controls will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        [Parameter] public int? HoveredValue { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            Console.WriteLine("OnParametersSet ItemValue:" + ItemValue + " selectedItem: " + Rating.SelectedValue);

            ItemIcon = SelectIcon();
        }

        private string SelectIcon()
        {
            if (Rating.HoveredValue.HasValue && Rating.HoveredValue.Value >= ItemValue)
            {
                return Rating.FullIcon;
            }
            else if (Rating.SelectedValue >= ItemValue)
            {
                if (Rating.HoveredValue.HasValue && Rating.HoveredValue.Value < ItemValue)
                {
                    return Rating.EmptyIcon;
                }
                else
                {
                    return Rating.FullIcon;
                }
            }
            else
            {
                return Rating.EmptyIcon;
            }
        }

        [Parameter] public EventCallback<int> ItemClicked { get; set; }

        [Parameter] public EventCallback<int?> ItemHovered { get; set; }

        // rating item lose hover
        private void HandleMouseOut(MouseEventArgs e)
        {
            IsActive = false;
            ItemHovered.InvokeAsync(null);
        }

        private void HandleMouseOver(MouseEventArgs e)
        {
            IsActive = true;
            ItemHovered.InvokeAsync(ItemValue);
        }

        private void HandleClick(MouseEventArgs e)
        {
            IsActive = false;
            ItemClicked.InvokeAsync(ItemValue);
        }
    }
}
