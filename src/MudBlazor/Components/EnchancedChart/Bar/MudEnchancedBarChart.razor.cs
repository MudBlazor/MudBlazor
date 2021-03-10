﻿using System;
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

            if (_dataSets.Count == 0 || _dataSets.Sum(x => x.Count) == 0 || _dataSets.Sum(x => x.Sum(y => y.Points.Count)) == 0 || _yaxes.Count == 0)
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

            TransformMatrix2D matrix = TransformMatrix2D.Identity;
            switch (_xAxis.Placement)
            {
                case XAxisPlacement.Bottom:
                    matrix = TransformMatrix2D.TranslateY(_xAxis.Margin + _xAxis.Height) * TransformMatrix2D.ScalingY((100 - _xAxis.Margin - _xAxis.Height) / 100);
                    break;
                case XAxisPlacement.Top:
                    matrix = TransformMatrix2D.ScalingY((100 - _xAxis.Margin - _xAxis.Height) / 100);
                    break;
                case XAxisPlacement.None:
                    matrix = TransformMatrix2D.Identity;
                    break;
                default:
                    break;
            }

            foreach (var item in axisMapper.Values)
            {
                item.CalculateTicks();

                switch (item.Axis.Placement)
                {
                    case YAxisPlacement.Left:
                        matrix = TransformMatrix2D.TranslateX(item.Axis.Margin + item.Axis.LabelSize) * TransformMatrix2D.ScalingX((100 - item.Axis.Margin - item.Axis.LabelSize) / 100) * matrix;
                        break;
                    case YAxisPlacement.Rigth:
                        matrix = TransformMatrix2D.ScalingX((100 - item.Axis.Margin - item.Axis.LabelSize) / 100) * matrix;
                        break;
                    case YAxisPlacement.None:
                    default:
                        //matrix = TransformMatrix2D.Identity * matrix;
                        continue;
                }
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
                Double marginLeftFromXAxis = axisMapper.Where(x => x.Value.Axis.Placement == YAxisPlacement.Left)
                     .Select(x => x.Value.Axis.Margin + x.Value.Axis.LabelSize)
                     .Sum();

                Double marginRigthFromXAxis = axisMapper.Where(x => x.Value.Axis.Placement == YAxisPlacement.Rigth)
                     .Select(x => x.Value.Axis.Margin + x.Value.Axis.LabelSize)
                     .Sum();

                Double spacePerXAxisLabel = (100.00 - marginRigthFromXAxis - marginLeftFromXAxis) / _xAxis.Labels.Count;

                Double x = marginLeftFromXAxis + (spacePerXAxisLabel / 2);

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

            foreach (var axisHelper in axisMapper.Values)
            {
                if (axisHelper.Axis.Placement == YAxisPlacement.None)
                {
                    continue;
                }

                Double marginFromXAxis = _xAxis.Placement == XAxisPlacement.None ? 0.0 : _xAxis.Margin + _xAxis.Height;

                Double x = axisHelper.Axis.Placement == YAxisPlacement.Left ? axisHelper.Axis.LabelSize : 100 - axisHelper.Axis.LabelSize;

                Double y = 100.0;
                if (_xAxis.Placement == XAxisPlacement.Bottom)
                {
                    y -= marginFromXAxis;
                }

                Double deltaYPerTick = (100.0 - marginFromXAxis) / (axisHelper.TickAmount - 1);
                Double tickValue = axisHelper.StartTickValue;
                Int32 startIndex = _labels.Count;

                for (int i = 0; i < axisHelper.TickAmount; i++)
                {
                    Point2D labelLeftCorner = new Point2D(x, y);

                    _labels.Add(new SvgText
                    {
                        Class = axisHelper.Axis.LabelCssClass,
                        Value = Math.Round(tickValue, 6).ToString(),
                        Height = 3,
                        X = labelLeftCorner.X,
                        Y = labelLeftCorner.Y,
                        TextPlacement = axisHelper.Axis.Placement == YAxisPlacement.Left ? "end" : "start",
                    });

                    y -= deltaYPerTick;
                    tickValue += axisHelper.TickNumericValue;
                }

                _labels[startIndex].Baseline = "text-after-edge";
                _labels[_labels.Count - 1].Baseline = "text-before-edge";
            }

            StateHasChanged();
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
                Axis = axis;

                if (axis.ScalesAutomatically == false)
                {
                    Min = axis.Min;
                    Max = axis.Max;
                }
            }

            public IYAxis Axis { get; private set; }
            public Double Min { get; private set; } = 0.0;
            public Double Max { get; private set; } = Double.MinValue;
            public Double StartTickValue { get; private set; } = 0.0;
            public Double TickNumericValue { get; private set; } = 0.0;
            public Int32 TickAmount { get; private set; } = 1;
            private Boolean _hasValues = false;

            internal void ProcessDataSet(BarDataSet set)
            {
                if (Axis.ScalesAutomatically == false) { return; }

                _hasValues = true;

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

            internal void CalculateTicks()
            {
                if (Axis.MajorTickValue == 0) { return; }
                if (Axis.ScalesAutomatically == false) { return; }
                if (_hasValues == false) { return; }

                Double initialDelta = Max - Min;
                Int32 scalingFactor = 0;
                Double valuePerTick = initialDelta / ((Int32)Axis.MajorTickValue - 1);

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

                TickNumericValue = valuePerTick;
                Int32 steps = (Int32)Math.Ceiling((Max - Min) / TickNumericValue);
                TickAmount = 1 + steps;
                Max = steps * TickNumericValue;
            }
        }
    }
}
