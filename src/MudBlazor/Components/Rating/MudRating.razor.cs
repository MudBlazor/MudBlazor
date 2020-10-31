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
        protected string Classname =>
        new CssBuilder("")
          .AddClass($"mud-rating-root")
          .AddClass(Class)
        .Build();


        /// <summary>
        /// User class names, separated by space
        /// </summary>
        [Parameter] public string ItemClass { get; set; }

        [Parameter] public string Name { get; set; } = Guid.NewGuid().ToString();
        [Parameter] public int MaxValue { get; set; } = 5;

        [Parameter] public string FullIcon { get; set; } = Icons.Material.Star;
        [Parameter] public string EmptyIcon { get; set; } = Icons.Material.StarBorder;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color? Color { get; set; } = null;
        /// <summary>
        /// The Size of the icon.
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


        [Parameter] public EventCallback<int> SelectedValueChanged { get; set; }

        [Parameter]
        public int SelectedValue
        {
            get => _selectedValue;
            set
            {
                Console.WriteLine($"selectedValue = {value}");
                if (_selectedValue == value)
                {
                    _selectedValue = 0;
                    HoveredValue = null;
                }
                else
                {
                    _selectedValue = value;
                }

                SelectedValueChanged.InvokeAsync(_selectedValue);
            }
        }

        private int _selectedValue = 0;

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

        private void HandleItemClicked(int itemValue)
        {
            SelectedValue = itemValue;
        }

        private void HandleItemHovered(int? itemValue)
        {
            HoveredValue = itemValue;
        }
    }
}
