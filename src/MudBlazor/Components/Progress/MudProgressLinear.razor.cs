using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudProgressLinear : MudComponentBase
    {
        protected string DivClassname =>
            new CssBuilder("mud-progress-linear")
                .AddClass($"mud-progress-linear-color-{Color.ToDescriptionString()}", !Buffer)
                .AddClass("mud-flip-x-rtl")
                .AddClass(Class)
                .Build();

        protected string LinearClassname =>
            new CssBuilder("mud-progress-linear-bar")
                .AddClass($"mud-{Color.ToDescriptionString()}")
                .AddClass($"mud-progress-indeterminate", Indeterminate)
                .AddClass($"mud-progress-linear-bar-1-determinate", !Indeterminate)
                .Build();

        protected string BufferClassname =>
            new CssBuilder("mud-progress-linear-dashed")
                .AddClass($"mud-progress-linear-dashed-color-{Color.ToDescriptionString()}")
                .Build();

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;
        [Parameter] public bool Indeterminate { get; set; }
        [Parameter] public bool Buffer { get; set; }
        [Parameter] public bool Static { get; set; }
        [Parameter] public int StrokeWidth { get; set; } = 3;

        /// <summary>
        /// The minimum allowed value of the slider. Should not be equal to max.
        /// </summary>
        [Parameter]
        public double Min
        {
            get => _min;
            set
            {
                _min = value;
                UpdatePercentages();
            }
        }

        private double _min = 0.0;
        private double _max = 100.0;

        /// <summary>
        /// The maximum allowed value of the slider. Should not be equal to min.
        /// </summary>
        /// 
        [Parameter]
        public double Max
        {
            get => _max;
            set
            {
                _max = value;
                UpdatePercentages();
            }
        }

        private double _value;
        private double _bufferValue;

        [Parameter]
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

        public double GetValuePercent()
        {
            var total = Math.Abs(_max - _min);
            if (NumericConverter<double>.AreEqual(0, total)) // numeric instability!
                return 0;
            var value = Math.Max(0, Math.Min(total, _value - _min));
            return value / total * 100.0;
        }

        public double GetBufferPercent()
        {
            var total = Math.Abs(_max - _min);
            if (NumericConverter<double>.AreEqual(0, total)) // numeric instability!
                return 0;
            var value = Math.Max(0, Math.Min(total, _bufferValue - _min));
            return value / total * 100.0;
        }

        #region --> Obsolete Forwarders for Backwards-Compatiblilty

        [Obsolete("This property is obsolete. Use Min instead.")] [Parameter] public double Minimum { get => Min; set => Min = value; }

        [Obsolete("This property is obsolete. Use Max instead.")] [Parameter] public double Maximum { get => Max; set => Max = value; }

        #endregion
    }
}
