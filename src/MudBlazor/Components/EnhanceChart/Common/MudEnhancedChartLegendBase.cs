using System;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.EnhanceChart
{
    public abstract class MudEnhancedChartLegendBase : MudComponentBase
    {
        [CascadingParameter] MudEnhancedChart Chart { get; set; }

        [Parameter] public ChartLegendInfo LegendInfo { get; set; }

        public void Update(ChartLegendInfo info)
        {
            LegendInfo = info;
            try
            {
                //sometimes fires a render error, not sure why
                StateHasChanged();
            }
            catch (Exception)
            {

            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Chart?.SetLegend(this);
        }

        protected void ToggleEnabledStateOfSeries(ChartLegendInfoSeries series) => ToggleEnabledStateOfSeries(series.Series);
        protected void ToggleEnabledStateOfSeries(ChartLegendInfoPoint point) => ToggleEnabledStateOfSeries(point.Point);
        protected void ToggleEnabledStateOfSeries(IChartDataElement element) => element?.ToggleEnabledState();

        public void ToggleEnabledStateOfSeries(IChartDataElement element, Boolean callStateHasChanged)
        {
            element?.ToggleEnabledState();
            if (callStateHasChanged == true)
            {
                StateHasChanged();
            }
        }

        public void ToggleEnabledStateOfSeries(ChartLegendInfoSeries series, Boolean callStateHasChanged) => ToggleEnabledStateOfSeries(series.Series, callStateHasChanged);
        public void ToggleEnabledStateOfSeries(ChartLegendInfoPoint point, Boolean callStateHasChanged) => ToggleEnabledStateOfSeries(point.Point, callStateHasChanged);

        protected void DeactiveAllOtherSeries(IChartDataElement element) => element?.SentRequestToBecomeActiveAlone();
        protected void DeactiveAllOtherSeries(ChartLegendInfoSeries active) => DeactiveAllOtherSeries(active.Series);
        protected void DeactiveAllOtherSeries(ChartLegendInfoPoint point) => DeactiveAllOtherSeries(point.Point);

        public void ActivedAllSeries(IChartDataElement element) => element.RevokeExclusiveActiveState();
        public void ActivedAllSeries(ChartLegendInfoSeries active) => ActivedAllSeries(active.Series);
        public void ActivedAllSeries(ChartLegendInfoPoint active) => ActivedAllSeries(active.Point);
    }
}
