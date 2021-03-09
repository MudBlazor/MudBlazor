using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor.Components.EnchancedChart;
using MudBlazor.Components.EnchancedChart.Svg;
using MudBlazor.Utilities;

namespace MudBlazor
{
    record MudEnchancedBarChartSnapShot(Double Margin, Double Padding);

    public partial class MudEnchancedBarChart : ICollection<BarDataSet>, ICollection<IYAxis>, ISnapshot<MudEnchancedBarChartSnapShot>
    {
        private List<SvgLine> _lines = new();
        private List<SvgText> _labels = new();
        private List<SvgPolygonBasedRectangle> _bars = new();

        private List<BarDataSet> _dataSets = new();
        private List<IYAxis> _yaxes = new();
        private BarChartXAxis _xAxis;

        [Parameter] public RenderFragment DataSets { get; set; }
        [Parameter] public RenderFragment YAxes { get; set; } = DefaultYAxesFragment;
        [Parameter] public RenderFragment XAxis { get; set; } = DefaultXAxisFragment;

        [Parameter] public Action<MudEnchancedBarChart> BeforeCreatingInstructionCallBack { get; set; }

        [Parameter] public Double Margin { get; set; } = 2.0;
        [Parameter] public Double Padding { get; set; } = 3.0;

        #region Updates from children

        public void DataSetUpdated(BarDataSet barChartSeries)
        {
            if (_dataSets.Contains(barChartSeries) == false)
            {
                _dataSets.Add(barChartSeries);
            }

            CreateDrawingInstruction();
        }

        protected internal void SeriesAdded(BarChartSeries _)
        {
            CreateDrawingInstruction();
        }

        protected internal void DataSetCleared(BarDataSet _)
        {
            CreateDrawingInstruction();
        }

        protected internal void DataSeriesRemoved(BarChartSeries _)
        {
            CreateDrawingInstruction();
        }

        protected internal void SeriesUpdated(BarDataSet _, BarChartSeries __)
        {
            CreateDrawingInstruction();
        }

        #region From Axes

        internal void AxesUpdated(NumericLinearAxis _)
        {
            CreateDrawingInstruction();
        }

        protected internal void MajorTickChanged(IYAxis axe, Tick _)
        {
            CreateDrawingInstruction();
        }

        protected internal void MinorTickChanged(IYAxis axe, Tick _)
        {
            CreateDrawingInstruction();
        }

        internal void XAxesUpdated(BarChartXAxis barChartXAxes)
        {
            if (_xAxis == null)
            {
                _xAxis = barChartXAxes;
            }
            else if (_xAxis != barChartXAxes)
            {
                _xAxis = barChartXAxes;
            }

            CreateDrawingInstruction();
        }

        #endregion

        #endregion

        #region Drawing

        public void ForceRedraw()
        {
            CreateDrawingInstruction();
        }

        private void CreateDrawingInstruction()
        {
            BeforeCreatingInstructionCallBack?.Invoke(this);

            if (_dataSets.Count == 0 || _dataSets.Sum(x => x.Count) == 0 || _yaxes.Count == 0)
            {
                return;
            }

            _bars.Clear();
            _labels.Clear();

            Dictionary<BarDataSet, AxisHelper> dataSetAxisMapper = new();
            Dictionary<IYAxis, AxisHelper> axisMapper = new();

            foreach (var set in _dataSets)
            {
                IYAxis axis = set.Axis;
                if (set.Axis == null)
                {
                    axis = _yaxes[0];
                }

                if (axisMapper.ContainsKey(axis) == false)
                {
                    axisMapper[axis] = new AxisHelper(axis);
                }

                AxisHelper helper = axisMapper[axis];

                dataSetAxisMapper[set] = helper;

                helper.ProcessDataSet(set);
            }

            Int32 amountOfSeries = _dataSets.Sum(x => x.Count);

            Double xPerLabel = 100.0 / _xAxis.Labels.Count;
            Double labelThickness = xPerLabel - Padding;
            Double subSpacePerSeriesPerLabel = labelThickness / amountOfSeries;
            Double barThickness = subSpacePerSeriesPerLabel - Margin;

            Double currentX = 0.0;

            TransformMatrix2D matrix = null;
            switch (_xAxis.Placement)
            {
                case XAxisPlacement.Bottom:
                    matrix = TransformMatrix2D.Translate(0, _xAxis.Margin + _xAxis.Height) * TransformMatrix2D.Scaling(1, (100 - _xAxis.Margin - _xAxis.Height) / 100);
                    break;
                case XAxisPlacement.Top:
                    matrix = TransformMatrix2D.Scaling(1, (100 - _xAxis.Margin - _xAxis.Height) / 100);
                    break;
                case XAxisPlacement.None:
                    matrix = TransformMatrix2D.Identity;
                    break;
                default:
                    break;
            }

            matrix = TransformMatrix2D.MirrorYAxis * TransformMatrix2D.Translate(0, -100) * matrix;

            for (int labelIndex = 0; labelIndex < _xAxis.Labels.Count; labelIndex++)
            {
                Double subX = currentX + Padding / 2.0;
                foreach (var set in _dataSets)
                {
                    var axisHelper = dataSetAxisMapper[set];

                    foreach (var series in set)
                    {
                        Double value = 0.0;
                        if (labelIndex < series.Points.Count)
                        {
                            value = series.Points[labelIndex];
                        }

                        Double height = (value / axisHelper.Max) * 100.0;
                        if (height > 100.0)
                        {
                            height = 100.0;
                        }

                        SvgPolygonBasedRectangle bar = new SvgPolygonBasedRectangle
                        {
                            P1 = matrix * new Point2D(subX, 0),
                            P2 = matrix * new Point2D(subX, height),
                            P3 = matrix * new Point2D(subX + barThickness, height),
                            P4 = matrix * new Point2D(subX + barThickness, 0),
                            Fill = series.Color,
                        };

                        _bars.Add(bar);

                        subX += subSpacePerSeriesPerLabel;
                    }
                }

                currentX += xPerLabel;
            }

            if (_xAxis.Placement != XAxisPlacement.None)
            {
                Double spacePerXAxisLabel = 100.00 / _xAxis.Labels.Count;

                Double x = spacePerXAxisLabel / 2;

                foreach (var item in _xAxis.Labels)
                {
                    Point2D labelLeftCorner = new Point2D(x,
                        (_xAxis.Placement == XAxisPlacement.Bottom) ? (100.00 - _xAxis.Height / 2) : _xAxis.Height / 2);
                    _labels.Add(new SvgText
                    {
                        Class = _xAxis.LabelCssClass,
                        Length = spacePerXAxisLabel,
                        Value = item,
                        Height = _xAxis.Height,
                        X = labelLeftCorner.X,
                        Y = labelLeftCorner.Y
                    });

                    x += spacePerXAxisLabel;
                }

            }

            StateHasChanged();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            //if (_isRendered == false)
            //{
            //    StateHasChanged();
            //    _isRendered = true;
            //}
        }

        #endregion

        #region Dataset ICollection Member

        public void Add(BarDataSet item)
        {
            if (_dataSets.Contains(item) == false)
            {
                _dataSets.Add(item);
                CreateDrawingInstruction();
            }
        }

        public bool Remove(BarDataSet item)
        {
            if (_dataSets.Contains(item) == true)
            {
                _dataSets.Remove(item);
                CreateDrawingInstruction();
                return true;
            }

            return false;
        }

        void ICollection<BarDataSet>.Clear()
        {
            _dataSets.Clear();
            CreateDrawingInstruction();
        }

        public void Clear()
        {
            _dataSets.Clear();
            _yaxes.Clear();
            CreateDrawingInstruction();
        }

        Int32 ICollection<BarDataSet>.Count => _dataSets.Count;
        Boolean ICollection<BarDataSet>.IsReadOnly => false;
        public bool Contains(BarDataSet item) => _dataSets.Contains(item);
        public void CopyTo(BarDataSet[] array, int arrayIndex) => _dataSets.CopyTo(array, arrayIndex);
        public IEnumerator<BarDataSet> GetEnumerator() => _dataSets.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _dataSets.GetEnumerator();

        #endregion

        #region YAxes ICollection Member

        public void Add(IYAxis item)
        {
            if (_yaxes.Contains(item) == false)
            {
                _yaxes.Add(item);
                CreateDrawingInstruction();
            }
        }

        public Boolean Remove(IYAxis item)
        {
            if (_yaxes.Contains(item) == true)
            {
                _yaxes.Remove(item);
                CreateDrawingInstruction();
                return true;
            }

            return false;
        }

        void ICollection<IYAxis>.Clear()
        {
            _yaxes.Clear();
            CreateDrawingInstruction();
        }

        Int32 ICollection<IYAxis>.Count => _yaxes.Count;
        Boolean ICollection<IYAxis>.IsReadOnly => false;

        public Boolean Contains(IYAxis item) => _yaxes.Contains(item);
        public void CopyTo(IYAxis[] array, int arrayIndex) => _yaxes.CopyTo(array, arrayIndex);
        IEnumerator<IYAxis> IEnumerable<IYAxis>.GetEnumerator() => _yaxes.GetEnumerator();

        #endregion

        #region DefaultRenderFragments

        public static void DefaultXAxisFragment(RenderTreeBuilder builder)
        {
            builder.OpenComponent(1, typeof(BarChartXAxis));
            builder.CloseComponent();
        }

        public static void DefaultYAxesFragment(RenderTreeBuilder builder)
        {
            builder.OpenComponent(1, typeof(NumericLinearAxis));
            builder.CloseComponent();
        }

        #endregion

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            ISnapshot<MudEnchancedBarChartSnapShot> _this = this;

            if (_this.SnapshotHasChanged(true) == true)
            {
                CreateDrawingInstruction();
            }
        }

        MudEnchancedBarChartSnapShot ISnapshot<MudEnchancedBarChartSnapShot>.OldSnapshotValue { get; set; }
        MudEnchancedBarChartSnapShot ISnapshot<MudEnchancedBarChartSnapShot>.CreateSnapShot() => new MudEnchancedBarChartSnapShot(Margin, Padding);

        class AxisHelper
        {
            public AxisHelper(IYAxis axis)
            {
                AXis = axis;

                if (axis.ScalesAutomatically == false)
                {
                    Min = axis.Min;
                    Max = axis.Max;
                }
            }

            public IYAxis AXis { get; private set; }
            public Double Min { get; private set; } = 0.0;
            public Double Max { get; private set; } = Double.MinValue;

            internal void ProcessDataSet(BarDataSet set)
            {
                if (AXis.ScalesAutomatically == false) { return; }

                foreach (var series in set)
                {
                    foreach (var yValue in series.Points)
                    {
                        if (yValue > Max)
                        {
                            Max = yValue;
                        }

                        if (yValue < Min)
                        {
                            Min = yValue;
                        }
                    }
                }
            }
        }
    }
}
