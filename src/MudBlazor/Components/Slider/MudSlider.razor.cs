using System;
using System.Globalization;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a slider component, allowing users to select a value within a specified range.
    /// </summary>
    /// <typeparam name="T">The type of the value the slider represents.</typeparam>
    public partial class MudSlider<T> : MudComponentBase where T : struct, INumber<T>
    {
        private int _tickMarkCount = 0;
        private bool _nullableValueResetToDefault = false;
        private readonly ParameterState<T> _valueState;
        private readonly ParameterState<T?> _nullableValueState;

        public MudSlider()
        {
            using var registerScope = CreateRegisterScope();
            _valueState = registerScope.RegisterParameter<T>(nameof(Value))
                .WithParameter(() => Value)
                .WithEventCallback(() => ValueChanged)
                .WithChangeHandler(OnValueParameterChangedAsync);
            _nullableValueState = registerScope.RegisterParameter<T?>(nameof(NullableValue))
                .WithParameter(() => NullableValue)
                .WithEventCallback(() => NullableValueChanged)
                .WithChangeHandler(OnNullableValueParameterChangedAsync);
        }

        protected string Classname =>
            new CssBuilder("mud-slider")
                .AddClass($"mud-slider-{Size.ToDescriptionString()}")
                .AddClass($"mud-slider-{Color.ToDescriptionString()}")
                .AddClass("mud-slider-vertical", Vertical)
                .AddClass(Class)
                .Build();

        /// <summary>
        /// The minimum allowed value of the slider. Should not be equal to max.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Validation)]
        public T Min { get; set; } = T.Zero;

        /// <summary>
        /// The maximum allowed value of the slider. Should not be equal to min.
        /// </summary>
        /// 
        [Parameter]
        [Category(CategoryTypes.Slider.Validation)]
        public T Max { get; set; } = T.CreateTruncating(100);

        /// <summary>
        /// How many steps the slider should take on each move.
        /// </summary>
        /// 
        [Parameter]
        [Category(CategoryTypes.Slider.Validation)]
        public T Step { get; set; } = T.One;

        /// <summary>
        /// If true, the slider will be disabled.
        /// </summary>
        /// 
        [Parameter]
        [Category(CategoryTypes.Slider.Behavior)]
        public bool Disabled { get; set; } = false;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Event callback invoked when the value of the slider changes.
        /// </summary>
        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }

        /// <summary>
        /// Event callback invoked when the nullable value of the slider changes.
        /// </summary>
        [Parameter]
        public EventCallback<T?> NullableValueChanged { get; set; }

        /// <summary>
        /// The value of the slider.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Data)]
        public T Value { get; set; } = T.Zero;

        /// <summary>
        /// The nullable value of the slider.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Data)]
        public T? NullableValue { get; set; } = default;

        /// <summary>
        /// The color of the component. It supports the Primary, Secondary and Tertiary theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// If true, the dragging the slider will update the Value immediately.
        /// If false, the Value is updated only on releasing the handle.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Behavior)]
        public bool Immediate { get; set; } = true;

        /// <summary>
        /// If true, displays the slider vertical.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Appearance)]
        public bool Vertical { get; set; } = false;

        /// <summary>
        /// If true, displays tick marks on the track.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Appearance)]
        public bool TickMarks { get; set; } = false;

        /// <summary>
        /// Labels for tick marks, will attempt to map the labels to each step in index order.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Appearance)]
        public string[]? TickMarkLabels { get; set; }

        /// <summary>
        /// Labels for tick marks, will attempt to map the labels to each step in index order.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Appearance)]
        public Size Size { get; set; } = Size.Small;

        /// <summary>
        /// The variant to use.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// Displays the value over the slider thumb.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public bool ValueLabel { get; set; }

        /// <summary>
        /// Sets the culture information used for ValueLabel. Default is <see cref="CultureInfo.InvariantCulture"/>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

        /// <summary>
        /// Sets the formatting information used for ValueLabel. Default is no formatting.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public string? ValueLabelFormat { get; set; }

        /// <summary>
        /// Sets custom RenderFragment for ValueLabel.
        /// </summary>
        /// <remarks>
        /// Keep in mind that for this RenderFragment to show the <see cref="ValueLabel"/> needs to be <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public RenderFragment<SliderContext<T>>? ValueLabelContent { get; set; }

        /// <inheritdoc />
        protected override void OnParametersSet()
        {
            if (TickMarks)
            {
                var min = Convert.ToDecimal(Min);
                var max = Convert.ToDecimal(Max);
                var step = Convert.ToDecimal(Step);

                _tickMarkCount = 1 + (int)((max - min) / step);
            }
            base.OnParametersSet();
        }

        private double CalculatePosition()
        {
            var min = Convert.ToDouble(Min);
            var max = Convert.ToDouble(Max);
            var value = Convert.ToDouble(_valueState.Value);
            var result = 100.0 * (value - min) / (max - min);

            result = Math.Min(Math.Max(0, result), 100);

            return Math.Round(result, 2);
        }

        private string GetValueText => _valueState.Value.ToString(null, CultureInfo.InvariantCulture);

        private async Task SetValueTextAsync(string? text)
        {
            if (T.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                await _valueState.SetValueAsync(result);
                await _nullableValueState.SetValueAsync(result);
            }
        }

        private Task OnValueParameterChangedAsync(ParameterChangedEventArgs<T> arg)
        {
            if (_nullableValueResetToDefault)
            {
                _nullableValueResetToDefault = false;

                return Task.CompletedTask;
            }

            return _nullableValueState.SetValueAsync(arg.Value);
        }

        private Task OnNullableValueParameterChangedAsync(ParameterChangedEventArgs<T?> arg)
        {
            if (arg.Value is null)
            {
                // if Value and NullableValue will be two-way bind at same time they will sync each other.
                // When attempting to reset NullableValue back to null, Value to zero,
                // and subsequently, Value will update NullableValue to zero.
                // This check prevents this.
                _nullableValueResetToDefault = true;
            }

            return _valueState.SetValueAsync(arg.Value.GetValueOrDefault(T.Zero));
        }

        private string Width => CalculatePosition().ToString(CultureInfo.InvariantCulture);
    }
}
