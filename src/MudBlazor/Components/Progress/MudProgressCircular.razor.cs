// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A circle-shaped indicator of progress for an ongoing operation.
    /// </summary>
    /// <seealso cref="MudProgressLinear"/>
    public partial class MudProgressCircular : MudComponentBase
    {
        private int _svgValue;
        private readonly ParameterState<double> _valueState;
        private const int MagicNumber = 126; // weird, but required for the SVG to work

        protected string Classname =>
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
        /// The color of this component.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ProgressCircular.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The size of this component.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ProgressCircular.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// Displays a constant animation without any value.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the <see cref="Value"/> will be ignored.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ProgressCircular.Behavior)]
        public bool Indeterminate { get; set; }

        /// <summary>
        /// The lowest possible value.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0.0</c>.  Usually a percentage.  Should be lower than <see cref="Max"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ProgressCircular.Behavior)]
        public double Min { get; set; } = 0.0;

        /// <summary>
        /// The highest possible value.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>100.0</c>.  Usually a percentage.  Should be higher than <see cref="Min"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ProgressCircular.Behavior)]
        public double Max { get; set; } = 100.0;

        /// <summary>
        /// The current progress amount.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.  Only applies when <see cref="Indeterminate"/> is <c>False</c>.  Should be between <see cref="Min"/> and <see cref="Max"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ProgressCircular.Behavior)]
        public double Value { get; set; }

        /// <summary>
        /// The thickness of the circle.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>3</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ProgressCircular.Appearance)]
        public int StrokeWidth { get; set; } = 3;

        public MudProgressCircular()
        {
            using var registerScope = CreateRegisterScope();
            _valueState = registerScope.RegisterParameter<double>(nameof(Value))
                .WithParameter(() => Value)
                .WithChangeHandler(OnValueParameterChanged)
                .WithComparer(DoubleEpsilonEqualityComparer.Default);
        }

        private void OnValueParameterChanged(ParameterChangedEventArgs<double> args)
        {
            _svgValue = ToSvgValue(args.Value);
            StateHasChanged();
        }

        /// <inheritdoc />
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
            return (int)Math.Round(MagicNumber - (MagicNumber * fraction));
        }
    }
}
