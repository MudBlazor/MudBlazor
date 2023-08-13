using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudProgressLinear : MudComponentBase
    {
        private double _min = 0.0;
        private double _max = 100.0;
        private double _value;
        private double _bufferValue;

        protected string DivClassname =>
            new CssBuilder("mud-progress-linear")
                .AddClass("mud-progress-linear-rounded", Rounded)
                .AddClass($"mud-progress-linear-striped", Striped)
                .AddClass($"mud-progress-indeterminate", Indeterminate)
                .AddClass($"mud-progress-linear-buffer", Buffer && !Indeterminate)
                .AddClass($"mud-progress-linear-{Size.ToDescriptionString()}")
                .AddClass($"mud-progress-linear-color-{Color.ToDescriptionString()}")
                .AddClass("horizontal", !Vertical)
                .AddClass("vertical", Vertical)
                .AddClass("mud-flip-x-rtl")
                .AddClass(Class)
                .Build();

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The size of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Appearance)]
        public Size Size { get; set; } = Size.Small;

        /// <summary>
        /// Constantly animates, does not follow any value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Behavior)]
        public bool Indeterminate { get; set; } = false;

        /// <summary>
        /// If true, the buffer value will be used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Behavior)]
        public bool Buffer { get; set; } = false;

        /// <summary>
        /// If true, border-radius is set to the themes default value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Appearance)]
        public bool Rounded { get; set; } = false;

        /// <summary>
        /// Adds stripes to the filled part of the linear progress.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Appearance)]
        public bool Striped { get; set; } = false;

        /// <summary>
        /// If true, the progress bar  will be displayed vertically.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Appearance)]
        public bool Vertical { get; set; } = false;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The minimum allowed value of the linear progress. Should not be equal to max.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Behavior)]
        public double Min
        {
            get => _min;
            set
            {
                _min = value;
                UpdatePercentages();
            }
        }

        /// <summary>
        /// The maximum allowed value of the linear progress. Should not be equal to min.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Behavior)]
        public double Max
        {
            get => _max;
            set
            {
                _max = value;
                UpdatePercentages();
            }
        }

        /// <summary>
        /// The maximum allowed value of the linear progress. Should not be equal to min.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Behavior)]
        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                UpdatePercentages();
            }
        }

        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Behavior)]
        public double BufferValue
        {
            get => _bufferValue;
            set
            {
                _bufferValue = value;
                UpdatePercentages();
            }
        }

        protected double ValuePercent { get; set; }
        protected double BufferPercent { get; set; }

        protected void UpdatePercentages()
        {
            ValuePercent = GetValuePercent();
            BufferPercent = GetBufferPercent();
            StateHasChanged();
        }

        private double GetPercentage(double input)
        {
            var total = Math.Abs(_max - _min);
            if (NumericConverter<double>.AreEqual(0, total))
            {  // numeric instability!
                return 0.0;
            }
            var value = Math.Max(0, Math.Min(total, input - _min));
            return value / total * 100.0;
        }

        public double GetValuePercent() => GetPercentage(_value);
        public double GetBufferPercent() => GetPercentage(_bufferValue);

        private string GetStyleBarTransform(double input) =>
            Vertical ? $"transform: translateY({(int)Math.Round(100 - input)}%);" : $"transform: translateX(-{(int)Math.Round(100 - input)}%);";

        public string GetStyledBar1Transform() => GetStyleBarTransform(ValuePercent);
        public string GetStyledBar2Transform() => GetStyleBarTransform(BufferPercent);

        #region --> Obsolete Forwarders for Backwards-Compatiblilty

        [Obsolete("Use Min instead.", true)]
        [ExcludeFromCodeCoverage]
        [Parameter] public double Minimum { get => Min; set => Min = value; }

        [Obsolete("Use Max instead.", true)]
        [ExcludeFromCodeCoverage]
        [Parameter] public double Maximum { get => Max; set => Max = value; }

        #endregion
    }
}
