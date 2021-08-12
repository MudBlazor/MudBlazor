using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.EnhanceChart.Internal;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart
{
    record BarChartSeriesSnapshot(String Name, String Color, Boolean IsEnabled);

    [DoNotGenerateAutomaticTest]
    public class MudEnhancedBarChartSeries : ComponentBase, ISnapshot<BarChartSeriesSnapshot>, IDisposable, IDataSeries
    {
        /// <summary>
        /// The parent bar dataset
        /// </summary>
        [CascadingParameter] public MudEnhancedBarDataSet Dataset { get; set; }

        /// <summary>
        /// The parent chart
        /// </summary>
        [CascadingParameter] public MudEnhancedBarChart Chart { get; set; }

        /// <summary>
        /// Name of the series. Used in tooltips and the legend
        /// </summary>
        [Parameter] public String Name { get; set; } = String.Empty;

        /// <summary>
        /// Points for this series
        /// </summary>
        [Parameter] public IList<Double> Points { get; set; } = new List<Double>();

        /// <summary>
        /// Color used for displaying. If not set, a random color is choosen
        /// </summary>
        [Parameter] public MudColor Color { get; set; } = RandomColorSelector.GetRandomColor();

        /// <summary>
        /// Setting a value indidacting that this series is active (highlighted) in the chart. Default is true
        /// </summary>
        [Parameter] public Boolean IsActive { get; set; } = true;

        /// <summary>
        /// If IsEnabled is false, the series is not rendered and not considered for features like autoscaling. Default is true
        /// </summary>
        [Parameter] public Boolean IsEnabled { get; set; } = true;

        /// <summary>
        /// The unique Id of this series
        /// </summary>
        [Parameter] public Guid Id { get; set; } = Guid.NewGuid();

        #region legend related 

        protected internal void SentRequestForTooltip(SvgBarRepresentation svgBarRepresentation)
        {
            if (Chart == null) { return; }

            Chart.AddTooltip(new BarChartToolTipInfo(svgBarRepresentation.XLabel, svgBarRepresentation.YValue, Name, Color, Dataset.Name,
                svgBarRepresentation.P1, svgBarRepresentation.P2, svgBarRepresentation.P3, svgBarRepresentation.P4), this);
        }

        protected internal void SentRevokationForTooltip(SvgBarRepresentation svgBarRepresentation)
        {
            Chart?.RemoveTooltip(this);
        }

        public void ToggleEnabledState()
        {
            IsEnabled = !IsEnabled;
            ISnapshot<BarChartSeriesSnapshot> _this = this;
            _this.CreateSnapshot();
            Dataset.SeriesUpdated(this);
        }

        #endregion

        #region active state related

        public void SentRequestToBecomeActiveAlone()
        {
            if (IsEnabled == false) { return; }

            Chart.SetSeriesAsExclusivelyActive(this);
        }

        public void RevokeExclusiveActiveState()
        {
            Chart.SetAllSeriesAsActive();
        }

        public void SetAsActive() => IsActive = true;
        public void SetAsInactive() => IsActive = false;

        #endregion

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            //if (Dataset == null)
            //{
            //    throw new ArgumentException("A chart series need to be placed inside a dataset");
            //}

            if (Dataset == null) { return; }

            if (Dataset.Contains(this) == false)
            {
                Dataset.Add(this);
                ISnapshot<BarChartSeriesSnapshot> _this = this;
                _this.CreateSnapshot();
            }
            else
            {
                ISnapshot<BarChartSeriesSnapshot> _this = this;
                if (_this.SnapshotHasChanged(true) == true)
                {
                    Dataset.SeriesUpdated(this);
                }
            }
        }

        public void Dispose()
        {
            Dataset?.Remove(this);
        }

        BarChartSeriesSnapshot ISnapshot<BarChartSeriesSnapshot>.OldSnapshotValue { get; set; }

        BarChartSeriesSnapshot ISnapshot<BarChartSeriesSnapshot>.CreateSnapShot() => new(Name, (String)Color, IsEnabled);

    }
}
