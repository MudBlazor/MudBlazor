// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Components.EnchancedChart;
using MudBlazor.Utilities;

namespace MudBlazor
{
    record BarChartSeriesSnapshot(String Name, String Color, Boolean IsEnabled);

    [DoNotGenerateAutomaticTest]
    public class BarChartSeries : ComponentBase, ISnapshot<BarChartSeriesSnapshot>, IDisposable, IDataSeries
    {
        [CascadingParameter] public BarDataSet Dataset { get; set; }
        [CascadingParameter] public MudEnchancedBarChart Chart { get; set; }

        [Parameter] public String Name { get; set; } = String.Empty;
        [Parameter] public IList<Double> Points { get; set; } = new List<Double>();
        [Parameter] public CssColor Color { get; set; } = RandomColorSelector.GetRandomColor();
        [Parameter] public Boolean IsEnabled { get; set; } = true;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (Dataset == null)
            {
                throw new ArgumentException("A chart series need to be placed inside a dataset");
            }

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

        BarChartSeriesSnapshot ISnapshot<BarChartSeriesSnapshot>.OldSnapshotValue { get; set; }


        BarChartSeriesSnapshot ISnapshot<BarChartSeriesSnapshot>.CreateSnapShot() => new(Name, (String)Color, IsEnabled);

        public void Dispose()
        {
            Dataset?.Remove(this);
        }

        public void ToggleEnabledState()
        {
            IsEnabled = !IsEnabled;
            ISnapshot<BarChartSeriesSnapshot> _this = this;
            _this.CreateSnapshot();
            Dataset.SeriesUpdated(this);
        }

        public Boolean IsActive { get; private set; } = true;

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
    }
}
