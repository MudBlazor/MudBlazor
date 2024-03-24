using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudProgressCircular : MudComponentBase
    {
        private int _svgValue;
        private IParameterState<double> _valueState;
        private const int _magicNumber = 126; // weird, but required for the SVG to work

        protected string DivClassname =>
            new CssBuilder("mud-progress-circular")
                .AddClass($"mud-{Color.ToDescriptionString()}-text")
                .AddClass($"mud-progress-{Size.ToDescriptionString()}")
                .AddClass("mud-progress-indeterminate", Indeterminate)
                .AddClass("mud-progress-static", !Indeterminate)
                .AddClass(Class)
                .Build();

        protected string SvgClassname =>
            new CssBuilder("mud-progress-circular-circle")
                .AddClass("mud-progress-indeterminate", Indeterminate)
                .AddClass("mud-progress-static", !Indeterminate)
                .Build();

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ProgressCircular.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The size of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ProgressCircular.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// Constantly animates, does not follow any value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ProgressCircular.Behavior)]
        public bool Indeterminate { get; set; }

        [Parameter]
        [Category(CategoryTypes.ProgressCircular.Behavior)]
        public double Min { get; set; } = 0.0;

        [Parameter]
        [Category(CategoryTypes.ProgressCircular.Behavior)]
        public double Max { get; set; } = 100.0;

        [Parameter]
        [Category(CategoryTypes.ProgressCircular.Behavior)]
        public double Value { get; set; }

        [Parameter]
        [Category(CategoryTypes.ProgressCircular.Appearance)]
        public int StrokeWidth { get; set; } = 3;

        [ExcludeFromCodeCoverage]
        [Obsolete("Use Min instead.", true)]
        [Parameter]
        public double Minimum { get => Min; set => Min = value; }

        [ExcludeFromCodeCoverage]
        [Obsolete("Use Max instead.", true)]
        [Parameter]
        public double Maximum { get => Max; set => Max = value; }

        public MudProgressCircular()
        {
            _valueState = RegisterParameter(nameof(Value), () => Value, OnValueParameterChanged, DoubleEpsilonEqualityComparer.Default);
        }

        private void OnValueParameterChanged(ParameterChangedEventArgs<double> args)
        {
            _svgValue = ToSvgValue(args.Value);
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _svgValue = ToSvgValue(_valueState.Value);
        }

        private int ToSvgValue(double value)
        {
            var minValue = Math.Min(Math.Max(Min, value), Max);
            // calculate fraction, which is a value between 0 and 1
            var fraction = (minValue - Min) / (Max - Min);
            // now project into the range of the SVG value (126 .. 0)
            return (int)Math.Round(_magicNumber - _magicNumber * fraction);
        }
    }
}
