using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Text;
using MudBlazor.Extensions;
using MudBlazor.Utilities;
using System.ComponentModel.DataAnnotations;

namespace MudBlazor
{
    public partial class MudRating : MudComponentBase
    {
        /// <summary>
        /// Space separated class names
        /// </summary>
        protected string ClassName =>
        new CssBuilder("")
          .AddClass($"mud-rating-root")
          .AddClass(Class)
        .Build();

        /// <summary>
        /// User class names for RatingItems, separated by space
        /// </summary>
        [Parameter] public string RatingItemsClass { get; set; }

        /// <summary>
        /// User styles for RatingItems.
        /// </summary>
        [Parameter] public string RatingItemsStyle { get; set; }

        /// <summary>
        /// Input name. If not initialized, name will be random guid.
        /// </summary>
        [Parameter] public string Name { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Max value and how many elements to click will be generated. Default: 5
        /// </summary>
        [Parameter] public int MaxValue { get; set; } = 5;

        /// <summary>
        /// Selected or hovered icon. Default @Icons.Material.Star
        /// </summary>
        [Parameter] public string FullIcon { get; set; } = Icons.Material.Star;

        /// <summary>
        /// Non selected item icon. Default @Icons.Material.StarBorder
        /// </summary>
        [Parameter] public string EmptyIcon { get; set; } = Icons.Material.StarBorder;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;
        /// <summary>
        /// The Size of the icons.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;
        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }
        /// <summary>
        /// If true, the controls will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// Fires when SelectedValue changes.
        /// </summary>
        [Parameter] public EventCallback<int> SelectedValueChanged { get; set; }

        /// <summary>
        /// Selected value. This property is two-way bindable.
        /// </summary>
        [Parameter]
        public int SelectedValue
        {
            get => _selectedValue;
            set
            {
                if (_selectedValue == value)
                    return;
   
                _selectedValue = value;

                SelectedValueChanged.InvokeAsync(_selectedValue);
            }
        }

        private int _selectedValue = 0;

        /// <summary>
        /// Fires when hovered value change. Value will be null if no rating item is hovered.
        /// </summary>
        [Parameter] public EventCallback<int?> HoveredValueChanged { get; set; }

        internal int? HoveredValue
        {
            get => _hoveredValue;
            set
            {
                if (_hoveredValue == value)
                    return;

                _hoveredValue = value;
                HoveredValueChanged.InvokeAsync(value);
            }
        }

        private int? _hoveredValue = null;

        internal bool IsRatingHover => HoveredValue.HasValue;

        private void HandleItemClicked(int itemValue) {
            SelectedValue = itemValue;

            if (itemValue == 0)
            {
                HoveredValue = null;
            }
        }

        private void HandleItemHovered(int? itemValue) => HoveredValue = itemValue;
    }
}
