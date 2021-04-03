﻿// Not Used

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart
{
    record NumericLinearAxisSnapshot(Double Min, Double Max, Boolean ShowMinorTick, Boolean ShowMajorTick, Double LabelSize, Double Margin, String LabelCssClass, YAxisPlacement Placement, ScalingType ScalingType);

    [DoNotGenerateAutomaticTest]
    public partial class MudEnhancedNumericLinearAxis : ComponentBase, IYAxis, IDisposable, ISnapshot<NumericLinearAxisSnapshot>
    {
        private Guid _id = new Guid();
        private TickOverview _tickInfo = new();

        public Guid Id => _id;

        private MudEnhancedTick _minorTick;
        private MudEnhancedTick _majorTick;

        [CascadingParameter] public MudEnhancedBarChart Chart { get; set; }

        [Parameter] public RenderFragment MajorTick { get; set; } = DefaultMajorTickFragment;

        [Parameter] public RenderFragment MinorTick { get; set; } = DefaultMinorTickFragment;

        [Parameter] public Double Min { get; set; }
        [Parameter] public Double Max { get; set; }

        [Parameter] public Boolean ShowMinorTicks { get; set; } = false;
        [Parameter] public Boolean ShowMajorTicks { get; set; } = true;

        [Parameter] public Double Margin { get; set; } = 2.0;
        [Parameter] public Double LabelSize { get; set; } = 5.0;

        [Parameter] public String LabelCssClass { get; set; } = String.Empty;

        [Parameter] public YAxisPlacement Placement { get; set; } = YAxisPlacement.Left;

        [Parameter] public ScalingType ScalingType { get; set; } = ScalingType.Auto;

        public Boolean ScalesAutomatically => ScalingType == ScalingType.Auto;

        public Double MajorTickValue => _majorTick?.Value ?? 0.0;
        public Double MinorTickValue => _minorTick?.Value ?? 0.0;

        private static void DefaultMinorTickFragment(RenderTreeBuilder builder)
        {
            builder.OpenComponent(1, typeof(MudEnhancedTick));
            builder.AddAttribute(2, nameof(MudEnhancedTick.Mode), TickMode.Relative);
            builder.AddAttribute(3, nameof(MudEnhancedTick.Value), 20.0);
            builder.CloseComponent();
        }

        private static void DefaultMajorTickFragment(RenderTreeBuilder builder)
        {
            builder.OpenComponent(1, typeof(MudEnhancedTick));
            builder.AddAttribute(2, nameof(MudEnhancedTick.Mode), TickMode.Relative);
            builder.AddAttribute(3, nameof(MudEnhancedTick.Value), 10.0);
            builder.CloseComponent();
        }

        public void TickUpdated(MudEnhancedTick tick)
        {
            if (tick.IsMajorTick == true)
            {
                if (_majorTick == null)
                {
                    _majorTick = tick;
                }

                Chart.MajorTickChanged(this, tick);
            }
            else
            {
                if (_minorTick == null)
                {
                    _minorTick = tick;
                }

                Chart.MinorTickChanged(this, tick);
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (Chart == null)
            {
                throw new ArgumentException("a axes needs to be placed inside a Chart");
            }

            if (Chart.Contains(this) == false)
            {
                Chart.Add(this);
                ISnapshot<NumericLinearAxisSnapshot> _this = this;
                _this.CreateSnapshot();
            }
            else
            {
                ISnapshot<NumericLinearAxisSnapshot> _this = this;
                if (_this.SnapshotHasChanged(true) == true)
                {
                    Chart.AxesUpdated(this);
                }
            }
        }

        public void Dispose()
        {
            Chart?.Remove(this);
        }

        NumericLinearAxisSnapshot ISnapshot<NumericLinearAxisSnapshot>.OldSnapshotValue { get; set; }

        public ITick MajorTickInfo => _majorTick?.GetTickInfo(ShowMajorTicks);
        public ITick MinorTickInfo => _minorTick?.GetTickInfo(ShowMinorTicks);

        NumericLinearAxisSnapshot ISnapshot<NumericLinearAxisSnapshot>.CreateSnapShot() => new NumericLinearAxisSnapshot(Min, Max, ShowMinorTicks, ShowMajorTicks, LabelSize, Margin, LabelCssClass, Placement, ScalingType);

        public void RemoveTick(bool isMajorTick)
        {
            if (isMajorTick == true)
            {
                _majorTick = null;
                Chart.MajorTickChanged(this, null);
            }
            else
            {
                _minorTick = null;
                Chart.MinorTickChanged(this, null);
            }
        }

        public void ProcessDataSet(IEnumerable<IDataSeries> set)
        {
            if (ScalesAutomatically == false) { return; }

            _tickInfo.HasValues = true;

            foreach (var series in set)
            {
                if (series.IsEnabled == false) { continue; }

                foreach (var yValue in series.Points)
                {
                    if (yValue > _tickInfo.Max)
                    {
                        _tickInfo.Max = yValue;
                    }

                    if (yValue < _tickInfo.Min)
                    {
                        _tickInfo.Min = yValue;
                    }
                }
            }
        }

        public void CalculateTicks()
        {
            if (ScalesAutomatically == false) { return; }

            if (_tickInfo.HasValues == false) { return; }

            Double initialDelta = _tickInfo.Max - _tickInfo.Min;

            if (MajorTickInfo != null)
            {
                Double firstStep = initialDelta / ((Int32)MajorTickValue - 1);

                _tickInfo.MajorTickNumericValue = GetNearestTickValue(initialDelta, firstStep);
                Int32 steps = (Int32)Math.Ceiling((_tickInfo.Max - _tickInfo.Min) / _tickInfo.MajorTickNumericValue);
                _tickInfo.MajorTickAmount = 1 + steps;
                _tickInfo.Max = steps * _tickInfo.MajorTickNumericValue;
            }
            else
            {
                _tickInfo.MajorTickNumericValue = initialDelta;
                _tickInfo.MajorTickAmount = 2;
                _tickInfo.Max = initialDelta;
            }
            if (MinorTickInfo != null)
            {
                _tickInfo.MinorTickNumericValue = GetNearestTickValue(_tickInfo.MajorTickNumericValue, _tickInfo.MajorTickNumericValue / ((Int32)MinorTickValue - 1));
                _tickInfo.MinorTickAmount = (Int32)((_tickInfo.MajorTickNumericValue / _tickInfo.MinorTickNumericValue) - 1.0);
            }
        }

        private static Double GetNearestTickValue(double initialDelta, Double firstStep)
        {
            Int32 scalingFactor = 0;
            Double valuePerTick = firstStep;

            if (initialDelta > 1)
            {
                while (valuePerTick > 1)
                {
                    valuePerTick /= 10;
                    scalingFactor++;
                }
            }
            else
            {
                while (valuePerTick < 0.1)
                {
                    valuePerTick *= 10;
                    scalingFactor++;
                }
            }

            if (valuePerTick < 0.15)
            {
                valuePerTick = 0.1;
            }
            else if (valuePerTick < 0.35)
            {
                valuePerTick = 0.2;
            }
            else
            {
                valuePerTick = 0.5;
            }

            if (initialDelta > 1)
            {
                for (int i = 0; i < scalingFactor; i++)
                {
                    valuePerTick *= 10;
                }
            }
            else
            {
                for (int i = 0; i < scalingFactor; i++)
                {
                    valuePerTick /= 10;
                }
            }

            return valuePerTick;
        }

        public TickOverview GetTickInfo() => _tickInfo;
    }
}
