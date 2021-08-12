using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart
{
    public record BarDataSetSnapShot(String name, Boolean IsStacked, Guid? axisId);

    [DoNotGenerateAutomaticTest]
    public partial class MudEnhancedBarDataSet : ComponentBase, IDataSet, ICollection<MudEnhancedBarChartSeries>, IDisposable, ISnapshot<BarDataSetSnapShot>
    {
        private List<MudEnhancedBarChartSeries> _series = new();

        /// <summary>
        /// The parent bar chart of this dataseries
        /// </summary>
        [CascadingParameter] public MudEnhancedBarChart Chart { get; set; }

        /// <summary>
        /// Name of the dataset. This value is displayed in the legend. Can be empty 
        /// </summary>
        [Parameter] public String Name { get; set; }

        /// <summary>
        /// If this value is set to true, the bars of this dataset are stacked horizontaly. If this value is set to false, the bars are displayed vertically, side by side. Not implemented yet
        /// </summary>
        [Parameter] public Boolean IsStacked { get; set; } = false;

        /// <summary>
        /// The dataseries that should belongs to this series
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// The y axis that should be used for this dataset
        /// </summary>
        [Parameter] public IYAxis Axis { get; set; }

        #region ICollection member

        public int Count => _series.Count;
        public bool IsReadOnly => throw new NotImplementedException();


        public void Add(MudEnhancedBarChartSeries item)
        {
            if (_series.Contains(item) == false)
            {
                _series.Add(item);
                Chart?.SeriesAdded(item);
            }
        }

        public void Clear()
        {
            _series.Clear();
            Chart.DataSetCleared(this);
        }

        public bool Remove(MudEnhancedBarChartSeries item)
        {
            if (_series.Contains(item) == true)
            {
                _series.Remove(item);
                Chart.DataSeriesRemoved(item);
                return true;
            }

            return false;
        }

        public bool Contains(MudEnhancedBarChartSeries item) => _series.Contains(item);
        public void CopyTo(MudEnhancedBarChartSeries[] array, int arrayIndex) => _series.CopyTo(array, arrayIndex);
        public IEnumerator<MudEnhancedBarChartSeries> GetEnumerator() => _series.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _series.GetEnumerator();

        #endregion

        protected internal void SeriesUpdated(MudEnhancedBarChartSeries barChartSeries) => Chart.SeriesUpdated(this, barChartSeries);

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            //if (Chart == null)
            //{
            //    throw new ArgumentException("A dataset need to be placed inside a char");
            //}

            if (Chart == null)
            {
                return;
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

        BarDataSetSnapShot ISnapshot<BarDataSetSnapShot>.OldSnapshotValue { get; set; }
        BarDataSetSnapShot ISnapshot<BarDataSetSnapShot>.CreateSnapShot() => new BarDataSetSnapShot(Name, IsStacked, Axis != null ? Axis.Id : null);

        public (Double Max, Double Min) GetMinimumAndMaximumValues()
        {
            Double min = 0;
            Double max = 0;

            if (IsStacked == false)
            {
                foreach (var item in _series)
                {
                    if (item.IsEnabled == false) { continue; }

                    foreach (var yValue in item.Points)
                    {
                        if (yValue > max)
                        {
                            max = yValue;
                        }

                        if (yValue < min)
                        {
                            min = yValue;
                        }
                    }
                }
            }
            else
            {
                Int32 maxIndex = _series.Max(x => x.Points.Count);

                for (int i = 0; i < maxIndex; i++)
                {
                    Double sliceSum = 0;

                    foreach (var item in _series)
                    {
                        if (item.IsEnabled == false) { continue; }

                        if (item.Points.Count > i)
                        {
                            sliceSum += item.Points[i];
                        }
                    }

                    if(sliceSum > max)
                    {
                        max = sliceSum;
                    }

                    if(sliceSum < min)
                    {
                        min = sliceSum;
                    }
                }
            }

            return (max, min);
        }
    }
}
