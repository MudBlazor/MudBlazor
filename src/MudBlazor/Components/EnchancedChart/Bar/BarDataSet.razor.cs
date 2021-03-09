using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Components.EnchancedChart;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public record BarDataSetSnapShot(String name, Boolean IsStacked, Guid? axisId);

    public partial class BarDataSet : ComponentBase, ICollection<BarChartSeries>, IDisposable, ISnapshot<BarDataSetSnapShot>
    {
        public BarDataSet()
        {

        }

        private List<BarChartSeries> _series = new();

        [CascadingParameter] public MudEnchancedBarChart Chart { get; set; }

        [Parameter] public String Name { get; set; }
        [Parameter] public Boolean IsStacked { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        public int Count => _series.Count;

        public bool IsReadOnly => throw new NotImplementedException();

        [Parameter] public IYAxis Axis { get; set; }

        BarDataSetSnapShot ISnapshot<BarDataSetSnapShot>.OldSnapshotValue { get; set; }
        BarDataSetSnapShot ISnapshot<BarDataSetSnapShot>.CreateSnapShot() => new BarDataSetSnapShot(Name, IsStacked, Axis != null ?  Axis.Id : null);

        public void Add(BarChartSeries item)
        {
            if (_series.Contains(item) == false)
            {
                _series.Add(item);
                Chart.SeriesAdded(item);
            }
        }

        public void Clear()
        {
            _series.Clear();
            Chart.DataSetCleared(this);
        }

        public bool Remove(BarChartSeries item)
        {
            if (_series.Contains(item) == true)
            {
                _series.Remove(item);
                Chart.DataSeriesRemoved(item);
                return true;
            }

            return false;
        }

        public bool Contains(BarChartSeries item) => _series.Contains(item);
        public void CopyTo(BarChartSeries[] array, int arrayIndex) => _series.CopyTo(array, arrayIndex);
        public IEnumerator<BarChartSeries> GetEnumerator() => _series.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _series.GetEnumerator();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (Chart == null)
            {
                throw new ArgumentException("A dataset need to be placed inside a char");
            }

            if (Chart.Contains(this) == false)
            {
                Chart.Add(this);
                ISnapshot<BarDataSetSnapShot> _this = this;
                _this.CreateSnapshot();
            }
            else
            {
                ISnapshot<BarDataSetSnapShot> _this = this;

                if (_this.SnapshotHasChanged(true) == true)
                {
                    Chart.DataSetUpdated(this);
                }
            }
        }

        public void Dispose()
        {
            Chart?.Remove(this);
        }


        protected internal void SeriesUpdated(BarChartSeries barChartSeries) => Chart.SeriesUpdated(this, barChartSeries);

    }
}
