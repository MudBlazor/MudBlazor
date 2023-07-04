using System;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudSlider<T> : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-slider")
                .AddClass($"mud-slider-{Size.ToDescriptionString()}")
                .AddClass($"mud-slider-{Color.ToDescriptionString()}")
                .AddClass("mud-slider-vertical", Vertical)
                .AddClass(Class)
                .Build();

        protected string? _value;
        protected string? _min = "0";
        protected string? _max = "100";
        protected string? _step = "1";

        /// <summary>
        /// The minimum allowed value of the slider. Should not be equal to max.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Validation)]
        public T? Min
        {
            get => Converter.Get(_min);
            set => _min = Converter.Set(value);
        }

        /// <summary>
        /// The maximum allowed value of the slider. Should not be equal to min.
        /// </summary>
        /// 
        [Parameter]
        [Category(CategoryTypes.Slider.Validation)]
        public T? Max
        {
            get => Converter.Get(_max);
            set => _max = Converter.Set(value);
        }

        /// <summary>
        /// How many steps the slider should take on each move.
        /// </summary>
        /// 
        [Parameter]
        [Category(CategoryTypes.Slider.Validation)]
        public T? Step
        {
            get => Converter.Get(_step);
            set => _step = Converter.Set(value);
        }

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

        [Parameter]
        [Category(CategoryTypes.Slider.Behavior)]
        public Converter<T> Converter { get; set; } = new DefaultConverter<T>() { Culture = CultureInfo.InvariantCulture };

        [Parameter] public EventCallback<T> ValueChanged { get; set; }

        [Parameter]
        [Category(CategoryTypes.Slider.Data)]
        public T? Value
        {
            get => Converter.Get(_value);
            set
            {
                var d = Converter.Set(value);
                if (_value == d)
                {
                    return;
                }

                _value = d;
                ValueChanged.InvokeAsync(value);
            }
        }

        /// <summary>
        /// The color of the component. It supports the Primary, Secondary and Tertiary theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Slider.Appearance)]
        public Color Color { get; set; } = Color.Primary;

        protected string? Text
        {
            get => _value;
            set
            {
                if (_value == value)
                {
                    return;
                }

                _value = value;
                ValueChanged.InvokeAsync(Value);
            }
        }

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

        private int _tickMarkCount = 0;
        protected override void OnParametersSet()
        {
            if (TickMarks)
            {
                var min = Convert.ToDouble(Min);
                var max = Convert.ToDouble(Max);
                var step = Convert.ToDouble(Step);

                _tickMarkCount = 1 + (int)((max - min) / step);
            }
        }

        private double CalculatePosition()
        {
            var min = Convert.ToDouble(Min);
            var max = Convert.ToDouble(Max);
            var value = Convert.ToDouble(Value);
            var result = 100.0 * (value - min) / (max - min);

            result = Math.Min(Math.Max(0, result), 100);

            return Math.Round(result, 2);
        }
    }
}
