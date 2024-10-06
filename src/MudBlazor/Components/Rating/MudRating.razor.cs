using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudRating : MudComponentBase
    {
        private readonly ParameterState<int> _selectedValueState;
        private int? _hoveredValue = null;

        public MudRating()
        {
            using var registerScope = CreateRegisterScope();
            _selectedValueState = registerScope.RegisterParameter<int>(nameof(SelectedValue))
                .WithParameter(() => SelectedValue)
                .WithEventCallback(() => SelectedValueChanged);
        }

        /// <summary>
        /// Space separated class names.
        /// </summary>
        protected string ClassName =>
            new CssBuilder("mud-rating-root")
                .AddClass("mud-disabled", Disabled)
                .AddClass(Class)
                .Build();

        /// <summary>
        /// User class names for RatingItems, separated by space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public string? RatingItemsClass { get; set; }

        /// <summary>
        /// User styles for RatingItems.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public string? RatingItemsStyle { get; set; }

        /// <summary>
        /// Input name. If not initialized, name will be a random GUID.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Rating.Behavior)]
        public string Name { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Max value and how many elements to click will be generated. Default: 5.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Rating.Behavior)]
        public int MaxValue { get; set; } = 5;

        /// <summary>
        /// Selected or hovered icon. Default @Icons.Material.Star.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public string FullIcon { get; set; } = Icons.Material.Filled.Star;

        /// <summary>
        /// Non-selected item icon. Default @Icons.Material.StarBorder.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public string EmptyIcon { get; set; } = Icons.Material.Filled.StarBorder;

        /// <summary>
        /// Selected or hovered icon color.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public Color? FullIconColor { get; set; }

        /// <summary>
        /// Non-selected item icon color.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public Color? EmptyIconColor { get; set; }

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The size of the icons.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// Gets or sets whether to show a ripple effect when the user clicks the button. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// If true, the controls will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Rating.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// If true, the ratings will show without interactions.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Rating.Behavior)]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Fires when SelectedValue changes.
        /// </summary>
        [Parameter]
        public EventCallback<int> SelectedValueChanged { get; set; }

        /// <summary>
        /// Selected value. This property is two-way bindable.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Rating.Data)]
        public int SelectedValue { get; set; } = 0;

        /// <summary>
        /// Fires when hovered value changes. Value will be null if no rating item is hovered.
        /// </summary>
        [Parameter]
        public EventCallback<int?> HoveredValueChanged { get; set; }

        internal int? HoveredValue => _hoveredValue;

        internal Task SetHoveredValueAsync(int? hoveredValue)
        {
            if (_hoveredValue == hoveredValue)
            {
                return Task.CompletedTask;
            }

            _hoveredValue = hoveredValue;
            return HoveredValueChanged.InvokeAsync(hoveredValue);
        }

        internal bool IsRatingHover => HoveredValue.HasValue;

        private async Task HandleItemClickedAsync(int itemValue)
        {
            await _selectedValueState.SetValueAsync(itemValue);

            if (itemValue == 0)
            {
                await SetHoveredValueAsync(null);
            }
        }

        internal Task HandleItemHoveredAsync(int? itemValue) => SetHoveredValueAsync(itemValue);

        private async Task IncreaseValueAsync(int val)
        {
            if ((_selectedValueState.Value != MaxValue || val <= 0) && (_selectedValueState.Value != 0 || val >= 0))
            {
                var value = _selectedValueState.Value + val;
                await _selectedValueState.SetValueAsync(value);
            }
        }

        protected internal async Task HandleKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
        {
            if (Disabled || ReadOnly)
            {
                return;
            }

            switch (keyboardEventArgs.Key)
            {
                case "ArrowRight" when keyboardEventArgs.ShiftKey:
                    await IncreaseValueAsync(MaxValue - _selectedValueState.Value);
                    break;
                case "ArrowRight":
                    await IncreaseValueAsync(1);
                    break;
                case "ArrowLeft" when keyboardEventArgs.ShiftKey:
                    await IncreaseValueAsync(-_selectedValueState.Value);
                    break;
                case "ArrowLeft":
                    await IncreaseValueAsync(-1);
                    break;
            }
        }
    }
}
