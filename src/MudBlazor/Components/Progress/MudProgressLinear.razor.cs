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
    /// A line-shaped indicator of progress for an ongoing operation.
    /// </summary>
    /// <seealso cref="MudProgressCircular"/>
    public partial class MudProgressLinear : MudComponentBase
    {
        private readonly ParameterState<double> _minState;
        private readonly ParameterState<double> _maxState;
        private readonly ParameterState<double> _valueState;
        private readonly ParameterState<double> _bufferValueState;

        protected string Classname =>
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
        /// The color of this component.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The size of this component.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Appearance)]
        public Size Size { get; set; } = Size.Small;

        /// <summary>
        /// Displays a constant animation without any value.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the <see cref="Value"/> will be ignored.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Behavior)]
        public bool Indeterminate { get; set; } = false;

        /// <summary>
        /// Displays an additional value ahead of <see cref="Value" />.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the value of <see cref="BufferValue"/> is displayed.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Behavior)]
        public bool Buffer { get; set; } = false;

        /// <summary>
        /// Displays a rounded border.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the CSS <c>border-radius</c> is set to the theme's default value.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Appearance)]
        public bool Rounded { get; set; } = false;

        /// <summary>
        /// Displays animated stripes for the value portion of this progress bar.
        /// </summary>
        /// <remarks>
        /// Default to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Appearance)]
        public bool Striped { get; set; } = false;

        /// <summary>
        /// Displays this progress bar vertically.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Appearance)]
        public bool Vertical { get; set; } = false;

        /// <summary>
        /// The content within this progress bar.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The lowest possible value.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0.0</c>.  Usually a percentage.  Should be lower than <see cref="Max"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Behavior)]
        public double Min { get; set; } = 0.0;

        /// <summary>
        /// The highest possible value.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>100.0</c>.  Usually a percentage.  Should be higher than <see cref="Min"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Behavior)]
        public double Max { get; set; } = 100.0;

        /// <summary>
        /// The current progress amount.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.  Only applies when <see cref="Indeterminate"/> is <c>false</c>.  Should be between <see cref="Min"/> and <see cref="Max"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Behavior)]
        public double Value { get; set; }

        /// <summary>
        /// The amount to display ahead of the value.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>0</c>.  Only shows when <see cref="Buffer"/> is <c>true</c> and <see cref="Indeterminate"/> is <c>false</c>.  Typically a value greater than <see cref="Value"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.ProgressLinear.Behavior)]
        public double BufferValue { get; set; }

        public MudProgressLinear()
        {
            using var registerScope = CreateRegisterScope();
            _valueState = registerScope.RegisterParameter<double>(nameof(Value))
                .WithParameter(() => Value)
                .WithChangeHandler(OnParameterChangedShared)
                .WithComparer(DoubleEpsilonEqualityComparer.Default);
            _minState = registerScope.RegisterParameter<double>(nameof(Min))
                .WithParameter(() => Min)
                .WithChangeHandler(OnParameterChangedShared);
            _maxState = registerScope.RegisterParameter<double>(nameof(Max))
                .WithParameter(() => Max)
                .WithChangeHandler(OnParameterChangedShared);
            _bufferValueState = registerScope.RegisterParameter<double>(nameof(BufferValue))
                .WithParameter(() => BufferValue)
                .WithChangeHandler(OnParameterChangedShared);
        }

        private void OnParameterChangedShared() => UpdatePercentages();

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
            var total = Math.Abs(_maxState.Value - _minState.Value);
            if (DoubleEpsilonEqualityComparer.Default.Equals(0, total))
            {
                // numeric instability!
                return 0.0;
            }

            var value = Math.Max(0, Math.Min(total, input - _minState.Value));

            return value / total * 100.0;
        }

        /// <summary>
        /// The calculated value percentage based on <see cref="Min"/>, <see cref="Max"/>, and <see cref="Value"/>.
        /// </summary>
        /// <returns>A value between <c>0.0</c> and <c>100.0</c>.</returns>
        public double GetValuePercent() => GetPercentage(_valueState.Value);

        /// <summary>
        /// The calculated buffer value percentage based on <see cref="Min"/>, <see cref="Max"/>, and <see cref="BufferValue"/>.
        /// </summary>
        /// <returns>A value between <c>0.0</c> and <c>100.0</c>.</returns>
        public double GetBufferPercent() => GetPercentage(_bufferValueState.Value);

        private string GetStyleBarTransform(double input) =>
            Vertical ? $"transform: translateY({(int)Math.Round(100 - input)}%);" : $"transform: translateX(-{(int)Math.Round(100 - input)}%);";

        /// <summary>
        /// Gets the CSS transform to apply based on the current value percentage.
        /// </summary>
        /// <returns>A CSS transform.</returns>
        public string GetStyledBar1Transform() => GetStyleBarTransform(ValuePercent);

        /// <summary>
        /// Gets the CSS transform to apply based on the current buffer value percentage.
        /// </summary>
        /// <returns>A CSS transform.</returns>
        public string GetStyledBar2Transform() => GetStyleBarTransform(BufferPercent);
    }
}
