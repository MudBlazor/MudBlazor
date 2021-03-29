using System;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.EnhanceChart
{
    public abstract class MudEnhancedChartLegendBase : MudComponentBase
    {
        [CascadingParameter] MudEnhancedChart Chart { get; set; }

        [Parameter] public ChartLegendInfo LegendInfo { get; set; }

        public async void Update(ChartLegendInfo info)
        {
            LegendInfo = info;
            try
            {
                //sometimes fires a render error, not sure why
                await InvokeAsync(StateHasChanged);
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

        protected void ToggleEnabledStateOfSeries(ChartLegendInfoSeries series)
        {
            series.Series?.ToggleEnabledState();
        }

        protected void DeactiveAllOtherSeries(ChartLegendInfoSeries active)
        {
            active.Series.SentRequestToBecomeActiveAlone();
        }

        protected void ActivedAllSeries(ChartLegendInfoSeries active)
        {
            active.Series.RevokeExclusiveActiveState();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
        }


    }
}
