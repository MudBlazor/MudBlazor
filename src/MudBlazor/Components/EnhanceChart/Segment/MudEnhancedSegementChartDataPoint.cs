// Not Used

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.EnhanceChart.Internal;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart
{
    record SegementChartDataPointSnapshot(Double Value, String Label, MudColor FillColor, Boolean IsEnabled);

    public abstract class MudEnhancedSegementChartDataPoint<TChart, TPoint, TSegementRepresentation> : ComponentBase, IDataPoint, ISnapshot<SegementChartDataPointSnapshot>, IDisposable
        where TPoint : MudEnhancedSegementChartDataPoint<TChart,TPoint, TSegementRepresentation>
        where TSegementRepresentation : SvgSegementRepresentation
        where TChart : MudEnhancedSegmentChart<TChart, TPoint, TSegementRepresentation>
    {
        [Parameter] public Double Value { get; set; } = 0.0;
        [Parameter] public String Label { get; set; } = String.Empty;
        [Parameter] public MudColor FillColor { get; set; } = RandomColorSelector.GetRandomColor();
        [Parameter] public Boolean IsEnabled { get; set; } = true;
        [Parameter] public String AddtionalClass { get; set; } = String.Empty;

        public void SetAsActive()
        {
            //show tooltip
            IsActive = true;
        }

        public void SetAsInactive()
        {
            //hide tooltip
            IsActive = false;
        }

        private Boolean _isActive = true;

        public Boolean IsActive
        {
            get => _isActive;
            set
            {
                if (value != _isActive)
                {
                    _isActive = value;
                    ActiveChanged.InvokeAsync(value);
                }
            }
        }

        [Parameter] public EventCallback<Boolean> ActiveChanged { get; set; }

        [Parameter] public EventCallback<Boolean> IsEnabledChanged { get; set; }

        private Guid _id = Guid.NewGuid();

        [Parameter]
        public Guid Id
        {
            get => _id;
            set
            {
                if (value != _id)
                {
                    _id = value;
                }
            }
        }

        [CascadingParameter] public MudEnhancedSegmentChart<TChart, TPoint, TSegementRepresentation> Chart { get; set; }

        #region life cycle hooks

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            //if (Chart == null)
            //{
            //    throw new ArgumentException("A datapoint needs to be placed inside a dataset");
            //}

            if (Chart == null)
            {
                return;
            }

            if (Chart.Contains((TPoint)this) == false)
            {
                Chart.Add((TPoint)this);
                ISnapshot<SegementChartDataPointSnapshot> _this = this;
                _this.CreateSnapshot();
            }
            else
            {
                ISnapshot<SegementChartDataPointSnapshot> _this = this;
                var changeResult = _this.SnapshotHasChanged(true, (old, current) => (old.Value != current.Value) || (old.IsEnabled != current.IsEnabled));
                if (changeResult.Item1 == true)
                {
                    Chart.UpdatedPoint((TPoint)this, changeResult.Item2);
                }
            }
        }

        public void Dispose()
        {
            Chart?.Remove((TPoint)this);
        }

        SegementChartDataPointSnapshot ISnapshot<SegementChartDataPointSnapshot>.OldSnapshotValue { get; set; }

        SegementChartDataPointSnapshot ISnapshot<SegementChartDataPointSnapshot>.CreateSnapShot() => new(Value, Label, (String)FillColor, IsEnabled);

        public void ToggleEnabledState()
        {
            IsEnabled = !IsEnabled;
            ISnapshot<SegementChartDataPointSnapshot> _this = this;
            _this.CreateSnapshot();
            IsEnabledChanged.InvokeAsync(IsEnabled);
            Chart.UpdatedPoint((TPoint)this, true);
        }

        public void SentRequestToBecomeActiveAlone()
        {
            if (IsEnabled == false) { return; }

            Chart.SetSeriesAsExclusivelyActive(this);
        }

        public void RevokeExclusiveActiveState()
        {
            Chart.SetAllSeriesAsActive();
        }

        protected abstract SegementChartToolTipInfo GenerateToolTip(TSegementRepresentation reprensentation);

        protected internal void SentRequestForTooltip(TSegementRepresentation reprensentation)
        {
            Chart?.AddTooltip(GenerateToolTip(reprensentation));
        }

        protected internal void SentRevokationForTooltip(TSegementRepresentation svgBarRepresentation)
        {
            Chart?.RemoveTooltip();
        }

        #endregion
    }
}
