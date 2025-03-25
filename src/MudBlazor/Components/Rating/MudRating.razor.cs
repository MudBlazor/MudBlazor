using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A component for collecting and displaying ratings.
    /// </summary>
    /// <seealso cref="MudRatingItem"/>
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
        /// The CSS classes applied to this component.
        /// </summary>
        protected string ClassName =>
            new CssBuilder("mud-rating-root")
                .AddClass("mud-disabled", Disabled)
                .AddClass(Class)
                .Build();

        /// <summary>
        /// The CSS classes to apply to each <see cref="MudRatingItem"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public string? RatingItemsClass { get; set; }

        /// <summary>
        /// The CSS styles to apply to each <see cref="MudRatingItem"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public string? RatingItemsStyle { get; set; }

        /// <summary>
        /// The name of this input.
        /// </summary>
        /// <remarks>
        /// Defaults to a new <see cref="Guid"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Behavior)]
        public string Name { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The number of <see cref="MudRatingItem"/> items to display.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>5</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Behavior)]
        public int MaxValue { get; set; } = 5;

        /// <summary>
        /// The icon displayed for selected items.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Star"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public string FullIcon { get; set; } = Icons.Material.Filled.Star;

        /// <summary>
        /// The icon displayed for unselected items.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.StarBorder"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public string EmptyIcon { get; set; } = Icons.Material.Filled.StarBorder;

        /// <summary>
        /// The color of the <see cref="FullIcon"/> for selected items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public Color? FullIconColor { get; set; }

        /// <summary>
        /// The color of the <see cref="EmptyIcon"/> for unselected items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public Color? EmptyIconColor { get; set; }

        /// <summary>
        /// The color of each item.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The size of the <see cref="FullIcon"/> and <see cref="EmptyIcon"/> icons.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// Shows a ripple effect when an item is clicked.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// Prevents the user from interacting with this rating and shows a disabled color.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Prevents this rating from being changed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Behavior)]
        public bool ReadOnly { get; set; }

        /// <summary>
        /// Occurs when <see cref="SelectedValue"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<int> SelectedValueChanged { get; set; }

        /// <summary>
        /// The currently selected value.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.  Must be equal or less than <see cref="MaxValue"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Rating.Data)]
        public int SelectedValue { get; set; } = 0;

        /// <summary>
        /// Occurs when <see cref="HoveredValue"/> has changed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        public EventCallback<int?> HoveredValueChanged { get; set; }

        /// <summary>
        /// The value the user is hovering over.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When the value is selected, <see cref="SelectedValue"/> will change.
        /// </remarks>
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

        /// <summary>
        /// Occurs when a key is pressed.
        /// </summary>
        /// <param name="keyboardEventArgs">The pressed key information.</param>
        /// <remarks>
        /// Has no effect if <see cref="Disabled"/> or <see cref="ReadOnly"/> is <c>true</c>.  
        /// The supported keyboard keys are: <c>ArrowRight</c> (increase value) and <c>ArrowLeft</c> (decrease value).
        /// </remarks>
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
