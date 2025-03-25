using System.Globalization;
using System.Numerics;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// A component which allows users to select a value within a specified range.
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
        /// The minimum allowed value.
        /// </summary>
        /// <remarks>
        /// Defauls to <c>0</c>.  Must be less than <see cref="Max"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Slider.Validation)]
        public T Min { get; set; } = T.Zero;

        /// <summary>
        /// The maximum allowed value.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>100</c>.  Must be greater than <see cref="Min"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Slider.Validation)]
        public T Max { get; set; } = T.CreateTruncating(100);

        /// <summary>
        /// How much the value changes on each move.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>1</c>.  
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Slider.Validation)]
        public T Step { get; set; } = T.One;

        /// <summary>
        /// Prevents the user from interacting with this slider.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Slider.Behavior)]
        public bool Disabled { get; set; } = false;

        /// <summary>
        /// The option content rendered above the slider.
        /// </summary>
        /// <remarks>
        /// Typically used for displaying text.   When the slider is vertical, content is displayed to the left of the slider.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Slider.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Occurs when <see cref="Value"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }

        /// <summary>
        /// Occurs when <see cref="NullableValue" /> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<T?> NullableValueChanged { get; set; }

        /// <summary>
        /// The value of this slider.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.  When this value changes, <see cref="ValueChanged"/> occurs.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Slider.Data)]
        public T Value { get; set; } = T.Zero;

        /// <summary>
        /// The nullable value of this slider.
        /// </summary>
        /// <remarks>
        /// When this value changes, <see cref="NullableValueChanged"/> occurs.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Slider.Data)]
        public T? NullableValue { get; set; } = default;

        /// <summary>
        /// The color of this slider.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Primary"/>.  <c>Primary</c>, <c>Secondary</c> and <c>Tertiary</c> colors are supported.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Slider.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// Controls when the value is updated.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.<br />
        /// When <c>true</c>, dragging the slider changes <see cref="Value"/> (or <see cref="NullableValue"/>) immediately.<br />
        /// When <c>false</c>, <see cref="Value"/> (or <see cref="NullableValue"/>) changes when releasing the handle.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Slider.Behavior)]
        public bool Immediate { get; set; } = true;

        /// <summary>
        /// Displays this slider vertically.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the slider is displayed like a horizontal slider, but rotated 90° counterclockwise.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Slider.Appearance)]
        public bool Vertical { get; set; } = false;

        /// <summary>
        /// Displays tick marks along the track.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Slider.Appearance)]
        public bool TickMarks { get; set; } = false;

        /// <summary>
        /// The tick mark labels for each step.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Only applies when <see cref="TickMarks"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Slider.Appearance)]
        public string[]? TickMarkLabels { get; set; }

        /// <summary>
        /// The size of this slider.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Small"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Slider.Appearance)]
        public Size Size { get; set; } = Size.Small;

        /// <summary>
        /// The display variant to use.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Text"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// Displays the value over the slider thumb.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public bool ValueLabel { get; set; }

        /// <summary>
        /// The culture used to format the value label.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="CultureInfo.InvariantCulture"/>.  Only applied when <see cref="ValueLabel"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

        /// <summary>
        /// The format of the value label.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  
        /// Only applies when <see cref="ValueLabelContent"/> is not set.<br />
        /// See: <see href="https://learn.microsoft.com/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</see>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public string? ValueLabelFormat { get; set; }

        /// <summary>
        /// The custom content for value labels.
        /// </summary>
        /// <remarks>
        /// Use the supplied context to access the current value.<br />
        /// Only applies when <see cref="ValueLabel"/> is <c>true</c> and <see cref="ValueLabelFormat"/> is not set.
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
