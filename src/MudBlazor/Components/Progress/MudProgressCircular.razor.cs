using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudProgressCircular : MudComponentBase
    {
        private const int _magicNumber = 126; // weird, but required for the SVG to work

        protected string DivClassname =>
            new CssBuilder("mud-progress-circular")
                .AddClass($"mud-{Color.ToDescriptionString()}-text")
                .AddClass($"mud-progress-{Size.ToDescriptionString()}")
                .AddClass($"mud-progress-indeterminate", Indeterminate)
                .AddClass($"mud-progress-static", !Indeterminate)
                .AddClass(Class)
                .Build();

        protected string SvgClassname =>
            new CssBuilder("mud-progress-circular-circle")
                .AddClass($"mud-progress-indeterminate", Indeterminate)
                .AddClass($"mud-progress-static", !Indeterminate)
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

        private int _svgValue;
        private double _value;

        [Parameter]
        [Category(CategoryTypes.ProgressCircular.Behavior)]
        public double Value
        {
            get => _value;
            set
            {
                if (!NumericConverter<double>.AreEqual(_value, value))
                {
                    _value = value;
                    _svgValue = ToSvgValue(_value);
                    StateHasChanged();
                }
            }
        }

        private int ToSvgValue(double value)
        {
            var minValue = Math.Min(Math.Max(Min, value), Max);
            // calculate fraction, which is a value between 0 and 1
            var fraction = (minValue - Min) / (Max - Min);
            // now project into the range of the SVG value (126 .. 0)
            return (int)Math.Round(_magicNumber - _magicNumber * fraction);
        }

        [Parameter]
        [Category(CategoryTypes.ProgressCircular.Appearance)]
        public int StrokeWidth { get; set; } = 3;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _svgValue = ToSvgValue(_value);
        }

        #region --> Obsolete Forwarders for Backwards-Compatiblilty

        [ExcludeFromCodeCoverage]
        [Obsolete("Use Min instead.", true)]
        [Parameter]
        public double Minimum { get => Min; set => Min = value; }

        [ExcludeFromCodeCoverage]
        [Obsolete("Use Max instead.", true)]
        [Parameter]
        public double Maximum { get => Max; set => Max = value; }

        #endregion
    }
}
